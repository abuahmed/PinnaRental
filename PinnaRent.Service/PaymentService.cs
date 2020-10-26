using System;
using System.Collections.Generic;
using System.Linq;
using PinnaRent.Core;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.DAL.Interfaces;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

namespace PinnaRent.Service
{
    public class PaymentService : IPaymentService
    {
        #region Fields

        private IDbContext _iDbContext;
        private IUnitOfWork _unitOfWork;
        private IRepository<PaymentDTO> _paymentRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public PaymentService()
        {
            InitializeDbContext();
        }
        public PaymentService(IDbContext iDbContext)
        {
            _iDbContext = iDbContext;
            _unitOfWork = new UnitOfWork(_iDbContext);
            _paymentRepository = _unitOfWork.Repository<PaymentDTO>();
        }
        public PaymentService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _unitOfWork = new UnitOfWork(_iDbContext);
            _paymentRepository = _unitOfWork.Repository<PaymentDTO>();
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<PaymentDTO> Get()
        {
            var piList = _paymentRepository
                .Query()
                .Include(c => c.RentDeposit,
                         c => c.RentalPayment,
                         c => c.RentalPayment.Contrat,
                         c => c.RentalPayment.Contrat.Rentee,
                         c => c.RentalPayment.Contrat.Room
                         //c => c.Check,
                         //c => c.Check.CustomerBankAccount,
                         //c => c.Clearance
                         )
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;

            //c => c.Clearance.DepositedBy,
            //c => c.Clearance.ClearedBy
        }

        public IEnumerable<PaymentDTO> GetAll(SearchCriteria<PaymentDTO> criteria = null)
        {
            IEnumerable<PaymentDTO> piList = new List<PaymentDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {
                    //var warehouseList = new WarehouseService(true)
                    //    .GetWarehousesPrevilegedToUser(criteria.CurrentUserId).ToList();
                    //if (criteria.SelectedWarehouseId != null)
                    //    warehouseList = warehouseList.Where(w => w.Id == criteria.SelectedWarehouseId).ToList();

                    //foreach (var warehouse in warehouseList.Where(w => w.Id != -1))
                    //{
                        var pdto = Get();

                        foreach (var cri in criteria.FiList)
                        {
                            pdto.FilterList(cri);
                        }

                        //#region By Warehouse
                        //var warehouse1 = warehouse;
                        //pdto.FilterList(p => p.WarehouseId == warehouse1.Id);
                        //#endregion

                        #region By Duration

                        if (criteria.BeginingDate != null)
                        {
                            var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                                criteria.BeginingDate.Value.Day, 0, 0, 0);
                            pdto.FilterList(p => p.PaymentDate >= beginDate);
                        }

                        if (criteria.EndingDate != null)
                        {
                            var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                                criteria.EndingDate.Value.Day, 23, 59, 59);
                            pdto.FilterList(p => p.PaymentDate <= endDate);
                        }

                        #endregion

                        //#region By Transaction Type
                        //if (criteria.TransactionType != -1)
                        //{
                        //    switch ((TransactionTypes)criteria.TransactionType)
                        //    {
                        //        case TransactionTypes.Sale:
                        //            {
                        //                pdto.FilterList(p => p.PaymentType == PaymentTypes.Sale);
                        //            }
                        //            break;
                        //        case TransactionTypes.Purchase:
                        //            {
                        //                pdto.FilterList(p => p.PaymentType == PaymentTypes.Purchase);
                        //            }
                        //            break;
                        //    }
                        //}
                        //#endregion

                        //#region By Payment List Types
                        //if (criteria.PaymentListType != -1)
                        //{
                        //    switch ((PaymentListTypes)criteria.PaymentListType)
                        //    {
                        //        case PaymentListTypes.All:
                        //            break;
                        //        case PaymentListTypes.Cleared:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.Cleared);
                        //            break;
                        //        case PaymentListTypes.NotCleared:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.NotCleared);
                        //            break;
                        //        case PaymentListTypes.NotClearedandOverdue:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.NotCleared && (p.DueDate != null && p.DueDate > DateTime.Now));
                        //            break;
                        //        case PaymentListTypes.NotDeposited:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.NotDeposited && p.PaymentMethod == PaymentMethods.Cash);
                        //            break;
                        //        case PaymentListTypes.DepositedNotCleared:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.NotCleared && p.PaymentMethod == PaymentMethods.Cash);
                        //            break;
                        //        case PaymentListTypes.DepositedCleared:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.Cleared && p.PaymentMethod == PaymentMethods.Cash);
                        //            break;
                        //        case PaymentListTypes.CreditNotCleared:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.NotCleared && p.PaymentMethod == PaymentMethods.Credit);
                        //            break;
                        //        case PaymentListTypes.CheckNotCleared:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.NotCleared && p.PaymentMethod == PaymentMethods.Check);
                        //            break;
                        //        case PaymentListTypes.CheckCleared:
                        //            pdto.FilterList(p => p.Status == PaymentStatus.Cleared && p.PaymentMethod == PaymentMethods.Check);
                        //            break;
                        //    }
                        //}
                        //#endregion

                        //#region By Payment Method
                        //if (criteria.PaymentMethodType != -1)
                        //{
                        //    switch ((PaymentMethods)criteria.PaymentMethodType)
                        //    {
                        //        case PaymentMethods.Cash:
                        //            pdto.FilterList(p => p.PaymentMethod == PaymentMethods.Cash);
                        //            break;
                        //        case PaymentMethods.Credit:
                        //            pdto.FilterList(p => p.PaymentMethod == PaymentMethods.Credit);
                        //            break;
                        //        case PaymentMethods.Check:
                        //            pdto.FilterList(p => p.PaymentMethod == PaymentMethods.Check);
                        //            break;
                        //    }
                        //}
                        //#endregion

                        //#region By Payment Type
                        //if (criteria.PaymentType != -1)
                        //{
                        //    switch (criteria.PaymentType)
                        //    {
                        //        case 2:
                        //            pdto.FilterList(p => p.PaymentType == PaymentTypes.CashOut);
                        //            break;
                        //        case 5:
                        //            pdto.FilterList(p => p.PaymentType == PaymentTypes.CashIn);
                        //            break;
                        //    }
                        //}
                        //#endregion

                        piList = piList.Concat(pdto.GetList().ToList());
                    //}
                }
                else
                {
                    piList = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;

        }

        public PaymentDTO Find(string paymentId)
        {
            var orgDto = _paymentRepository.FindById(Convert.ToInt32(paymentId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public PaymentDTO GetByName(string displayName)
        {
            var cat = _paymentRepository.Query().Filter(c => c.PersonName == displayName).Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(PaymentDTO payment)
        {
            try
            {
                var validate = Validate(payment);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(payment))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name/Tin No. Exists";

                _paymentRepository.InsertUpdate(payment);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(PaymentDTO payment)
        {

            if (payment == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _paymentRepository.Update(payment);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string paymentId)
        {
            try
            {
                _paymentRepository.Delete(paymentId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        //public bool DeleteRange(IEnumerable<PaymentDTO> payments)
        //{
        //    try
        //    {
        //        foreach (var paymentDTO in payments)
        //        {
        //            _paymentRepository.Delete(paymentDTO.Id);
        //        }
                
        //        _unitOfWork.Commit();
        //        if (_disposeWhenDone)
        //            Dispose();
        //        return true;
        //    }
        //    catch (Exception exception)
        //    {
        //        return false;
        //    }
        //}

        public bool ObjectExists(PaymentDTO payment)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<PaymentDTO>(iDbContext);
            //    var catExists = catRepository.GetAll()
            //        .FirstOrDefault(bp => (bp.DisplayName == payment.DisplayName ||
            //        (!string.IsNullOrEmpty(bp.TinNumber) && bp.TinNumber == payment.TinNumber)) &&
            //        bp.Id != payment.Id);


            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            return objectExists;
        }

        public string Validate(PaymentDTO payment)
        {
            if (null == payment)
                return GenericMessages.ObjectIsNull;
            
            if (String.IsNullOrEmpty(payment.Reason))
                return payment.Reason + " " + GenericMessages.StringIsNullOrEmpty;

            if (String.IsNullOrEmpty(payment.PersonName))
                return payment.PersonName + " " + GenericMessages.StringIsNullOrEmpty;

            return string.Empty;
        }

        #endregion

        #region Private Methods
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