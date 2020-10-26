using System;
using System.Collections.Generic;
using System.Linq;
using PinnaRent.Core;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.DAL.Interfaces;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

namespace PinnaRent.Service
{
    public class RepresenteeService : IRepresenteeService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<RepresenteeDTO> _representeeRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public RepresenteeService()
        {
            InitializeDbContext();
        }

        public RepresenteeService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _representeeRepository = new Repository<RepresenteeDTO>(_iDbContext);
            //_representeeSubscriptionRepository = new Repository<RepresenteeSubscriptionDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<RepresenteeDTO> Get()
        {
            var piList = _representeeRepository
                .Query()
                .Include(a => a.Address)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser);
            return piList;
        }

        public IEnumerable<RepresenteeDTO> GetAll(SearchCriteria<RepresenteeDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<RepresenteeDTO> GetAll(SearchCriteria<RepresenteeDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<RepresenteeDTO> piList = new List<RepresenteeDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<RepresenteeDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0 && criteria.PaymentListType == -1)
                    {
                        int totalCount2;
                        pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount2).ToList();
                        totalCount = totalCount2;
                    }
                    else
                    {
                        pdtoList = pdto.GetList().ToList();
                        totalCount = pdtoList.Count;
                    }

                    piList = pdtoList.ToList();
                }
                else
                    piList = Get().Get().OrderBy(i => i.Id).ToList();

                #region For Eager Loading Childs
                ////foreach (var transactionHeaderDTO in piList)
                ////{
                //   var subscriptioneDtos = _representeeSubscriptionRepository
                //        .Query()
                //        .Include(a => a.FacilitySubscription, a => a.FacilitySubscription.Subscription,
                //            a => a.FacilitySubscription.Facility)
                //        .Get()
                //        .OrderBy(i => i.Id)
                //        .ToList();
                //    //var transactionLineDtos =
                //    //(ICollection<TransactionLineDTO>)GetChilds(transactionHeaderDTO.Id, false);
                ////}
                #endregion

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return piList;
        }

        public RepresenteeDTO Find(string representeeId)
        {
            var bpId = Convert.ToInt32(representeeId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public RepresenteeDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.FullName == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(RepresenteeDTO representee)
        {
            try
            {
                var validate = Validate(representee);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(representee))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same name exists";

                _representeeRepository.InsertUpdate(representee);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(RepresenteeDTO representee)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.RepresenteeId == Representee.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.RepresenteeId == Representee.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (representee == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _representeeRepository.Update(representee);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string representeeId)
        {
            try
            {
                _representeeRepository.Delete(representeeId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(RepresenteeDTO Representee)
        {
            return false;
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<RepresenteeDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => (!string.IsNullOrEmpty(bp.TinNumber) && bp.TinNumber == Representee.TinNumber) &&
            //                      bp.Id != Representee.Id)
            //        .Get()
            //        .FirstOrDefault();


            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            //return objectExists;
        }

        public string Validate(RepresenteeDTO Representee)
        {
            if (null == Representee)
                return GenericMessages.ObjectIsNull;

            if (Representee.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(Representee.FullName))
                return Representee.FullName + " " + GenericMessages.StringIsNullOrEmpty;

            if (Representee.FullName.Length > 255)
                return Representee.FullName + " can not be more than 255 characters ";

            if (!string.IsNullOrEmpty(Representee.AuthorizationNumber) && Representee.AuthorizationNumber.Length > 50)
                return Representee.AuthorizationNumber + " can not be more than 50 characters ";


            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetRepresenteeNumber(int representeeId)
        {
            var pref = representeeId.ToString();
            if (representeeId < 1000)
            {
                var id = representeeId + 10000;
                pref = id.ToString();
                pref = pref.Substring(1);
            }
            var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
            return "DRN/" + pref + "/" + amhCalender.Substring(6);
        }
        #endregion

        #region Disposing
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}