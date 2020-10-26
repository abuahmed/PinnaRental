using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PinnaRent.Core;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.DAL.Interfaces;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

namespace PinnaRent.Service
{
    public class RenteeService : IRenteeService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<RenteeDTO> _renteeRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public RenteeService()
        {
            InitializeDbContext();
        }

        public RenteeService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _renteeRepository = new Repository<RenteeDTO>(_iDbContext);
            //_renteeSubscriptionRepository = new Repository<RenteeSubscriptionDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<RenteeDTO> Get()
        {
            var piList = _renteeRepository
                .Query()
                .Include(a => a.Address).Include(a => a.Title)
                .Include(a => a.Representee).Include(a => a.Representee.Title).Include(a => a.Representee.Address)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(q => q.OrderByDescending(c => c.Id));
            return piList;
        }

        public IEnumerable<RenteeDTO> GetAll(SearchCriteria<RenteeDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<RenteeDTO> GetAll(SearchCriteria<RenteeDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<RenteeDTO> piList = new List<RenteeDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<RenteeDTO> pdtoList;
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
                //   var subscriptioneDtos = _renteeSubscriptionRepository
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

        public RenteeDTO Find(string renteeId)
        {
            var bpId = Convert.ToInt32(renteeId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public RenteeDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.DisplayName == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(RenteeDTO rentee)
        {
            try
            {
                var validate = Validate(rentee);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(rentee))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same name exists";

                _renteeRepository.InsertUpdate(rentee);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(RenteeDTO rentee)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.RenteeId == Rentee.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.RenteeId == Rentee.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (rentee == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _renteeRepository.Update(rentee);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string renteeId)
        {
            try
            {
                int renId = Convert.ToInt16(renteeId);
                _renteeRepository.Delete(renId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(RenteeDTO rentee)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<RenteeDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (!string.IsNullOrEmpty(bp.TinNumber) && bp.TinNumber == rentee.TinNumber) &&
                                  bp.Id != rentee.Id)
                    .Get()
                    .FirstOrDefault();


                if (catExists != null)
                    objectExists = true;
            }
            finally
            {
                iDbContext.Dispose();
            }

            return objectExists;
        }

        public string Validate(RenteeDTO rentee)
        {
            if (null == rentee)
                return GenericMessages.ObjectIsNull;

            if (rentee.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(rentee.DisplayName))
                return rentee.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (rentee.DisplayName.Length > 255)
                return rentee.DisplayName + " can not be more than 255 characters ";

            if (!string.IsNullOrEmpty(rentee.Number) && rentee.Number.Length > 50)
                return rentee.Number + " can not be more than 50 characters ";


            return string.Empty;
        }

        #endregion

        #region Private Methods
        //public string GetRenteeNumber(int renteeId)
        //{
        //    var pref = renteeId.ToString();
        //    if (renteeId < 1000)
        //    {
        //        var id = renteeId + 10000;
        //        pref = id.ToString();
        //        pref = pref.Substring(1);
        //    }
        //    var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
        //    return "DRN/" + pref + "/" + amhCalender.Substring(6);
        //}
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