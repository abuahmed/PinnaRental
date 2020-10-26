using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PinnaRent.Core;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

namespace PinnaRent.Service
{
    public class RentalPaymentRemarkService : IRentalPaymentRemarkService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<RentalPaymentRemarkDTO> _rentalPaymentRemarkRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public RentalPaymentRemarkService()
        {
            InitializeDbContext();
        }

        public RentalPaymentRemarkService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _rentalPaymentRemarkRepository = new Repository<RentalPaymentRemarkDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<RentalPaymentRemarkDTO> Get()
        {
            var piList = _rentalPaymentRemarkRepository
                .Query()
                .Include(a => a.RentalPayment)
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<RentalPaymentRemarkDTO> GetAll(SearchCriteria<RentalPaymentRemarkDTO> criteria = null)
        {
            IEnumerable<RentalPaymentRemarkDTO> piList;// = new List<RentalPaymentRemarkDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<RentalPaymentRemarkDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoList = pdto.GetList().ToList();

                    piList = pdtoList.ToList();

                }
                else
                {
                    piList = Get().Get().OrderBy(i => i.Id).ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return piList;
        }

        public RentalPaymentRemarkDTO Find(string rentalPaymentRemarkId)
        {
            var orgDto = _rentalPaymentRemarkRepository.FindById(Convert.ToInt32(rentalPaymentRemarkId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public RentalPaymentRemarkDTO GetByName(string displayName)
        {
            //var cat = _rentalPaymentRemarkRepository.Query().Filter(c => c.Number == displayName).Get().FirstOrDefault();
            //return cat;
            return null;
        }

        public string InsertOrUpdate(RentalPaymentRemarkDTO rentalPaymentRemark)
        {
            try
            {
                var validate = Validate(rentalPaymentRemark);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(rentalPaymentRemark))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _rentalPaymentRemarkRepository.InsertUpdate(rentalPaymentRemark);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(RentalPaymentRemarkDTO rentalPaymentRemark)
        {
            if (rentalPaymentRemark == null)
                return GenericMessages.ObjectIsNull;
            
            string stat;
            try
            {
                _rentalPaymentRemarkRepository.Update(rentalPaymentRemark);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string rentalPaymentRemarkId)
        {
            try
            {
                var roomresId = Convert.ToInt32(rentalPaymentRemarkId);
                _rentalPaymentRemarkRepository.Delete(roomresId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(RentalPaymentRemarkDTO rentalPaymentRemark)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<RentalPaymentRemarkDTO>(iDbContext);
            //    var catExists = catRepository.Query().Filter(bp => bp.RentalPaymentId == rentalPaymentRemark.RentalPaymentId
            //        && bp.Id != rentalPaymentRemark.Id)
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
            return false;
        }

        public string Validate(RentalPaymentRemarkDTO rentalPaymentRemark)
        {
            if (null == rentalPaymentRemark || rentalPaymentRemark.RentalPaymentId == 0)
                return GenericMessages.ObjectIsNull;

            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetItemCode()
        {
            try
            {
                var prefix = "I";
                //if (new ClientService(true).GetClient().HasOnlineAccess)
                prefix = Singleton.Edition == PinnaRentEdition.OnlineEdition ? prefix + "S" : prefix + "L";//S=Server L=Local

                var lastNum = 0;

                var lastItem = new RentalPaymentRemarkService(true).GetLastItemIncludingDeleted();

                if (lastItem != null)
                {
                    //int.TryParse(lastItem.ItemCode.Substring(2), out lastNum);
                    lastNum = lastItem.Id;
                }

                lastNum = lastNum + 1 + 100000;
                return prefix + lastNum.ToString(CultureInfo.InvariantCulture).Substring(1);

            }
            catch
            {
                //MessageBox.Show("Problem getting rentalPaymentRemark number, try again...");
                return null;// prefix + (SelectedWarehouse.WarehouseNumber * 100000).ToString() + "0001";
            }
        }
        public string GetItemNumber(int rentalPaymentRemarkId)
        {
            var pref = rentalPaymentRemarkId.ToString();
            if (rentalPaymentRemarkId < 1000)
            {
                var id = rentalPaymentRemarkId + 10000;
                pref = id.ToString();
                pref = pref.Substring(1);
            }
            //var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
            return "DI" + pref;// + "/" + amhCalender.Substring(6);
        }
        public RentalPaymentRemarkDTO GetLastItemIncludingDeleted()
        {
            RentalPaymentRemarkDTO piList;
            try
            {
                piList = Get()
                    .Get(1)
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefault();
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;
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