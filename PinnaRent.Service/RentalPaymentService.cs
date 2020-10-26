#region

//using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using PinnaRent.Core;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

#endregion

namespace PinnaRent.Service
{
    public class RentalPaymentService : IRentalPaymentService
    {
        #region Fields

        private IUnitOfWork _unitOfWork;
        private IRepository<RentalPaymentDTO> _rentalPaymentRepository;
        private IRepository<RentalContratDTO> _rentalContratRepository;
        private IRepository<RoomDTO> _roomRepository;
        private IRepository<PaymentDTO> _paymentRepository;
        private readonly bool _disposeWhenDone;

        #endregion

        #region Constructor

        public RentalPaymentService()
        {
            InitializeDbContext();
        }

        public RentalPaymentService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _rentalPaymentRepository = new Repository<RentalPaymentDTO>(iDbContext);
            _rentalContratRepository = new Repository<RentalContratDTO>(iDbContext);
            _roomRepository = new Repository<RoomDTO>(iDbContext);
            _paymentRepository=new Repository<PaymentDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods

        public IRepositoryQuery<RentalPaymentDTO> Get()
        {
            var piList = _rentalPaymentRepository
                .Query()
                .Include(a => a.Contrat)
                .Include(a => a.Contrat.Room)
                .Include(a => a.Contrat.Room.Floor)
                .Include(a => a.Contrat.Room.RoomContrats)
                .Include(a => a.Contrat.Rentee)
                .Include(a => a.Contrat.Rentee.Address)
                .Include(a => a.Payments)
                .Include(a => a.ServicePayment)
                .Include(a => a.PaymentRemarks)
                .OrderBy(q => q.OrderByDescending(c => c.EndDate));
            return piList;
        }

        public IEnumerable<RentalPaymentDTO> GetAll(SearchCriteria<RentalPaymentDTO> criteria = null)
        {
            int totalCount = 0;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<RentalPaymentDTO> GetAll(SearchCriteria<RentalPaymentDTO> criteria, out int totalCount)
        {
            IEnumerable<RentalPaymentDTO> piList; // = new List<RentalPaymentDTO>();
            totalCount = 0;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

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

                    IList<RentalPaymentDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount2;
                        pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount2).ToList();
                        totalCount = totalCount2;
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

        public RentalPaymentDTO Find(string rentalPaymentId)
        {
            var orgDto = _rentalPaymentRepository.FindById(Convert.ToInt32(rentalPaymentId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public RentalPaymentDTO GetByName(string displayName)
        {
            var cat =
                _rentalPaymentRepository.Query().Filter(c => c.PaymentNumber == displayName).Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(RentalPaymentDTO rentalPayment)
        {
            try
            {

                var validate = Validate(rentalPayment);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(rentalPayment))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                if (!rentalPayment.IsArchived)
                {
                    var contract = _rentalContratRepository.FindById(rentalPayment.ContratId);
                    contract.LastRentalPayment = rentalPayment;
                    _rentalContratRepository.InsertUpdate(contract);
                    var room = _roomRepository.FindById(contract.RoomId);
                    room.LastRentalPayment = rentalPayment;
                    room.LastRenteeId = contract.RenteeId;
                    if (rentalPayment.ServicePaymentId != null && rentalPayment.ServicePaymentId != 0)
                    room.LastServicePaymentId = rentalPayment.ServicePaymentId;
                    _roomRepository.InsertUpdate(room);
                }
                
                _rentalPaymentRepository.InsertUpdate(rentalPayment);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string InsertOrUpdateWithPayment(RentalPaymentDTO rentalPayment,IEnumerable<PaymentDTO> paymentsDtos )
        {
            try
            {
                var validate = Validate(rentalPayment);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(rentalPayment))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                if (!rentalPayment.IsArchived)
                {
                    var contract = _rentalContratRepository.FindById(rentalPayment.ContratId);
                    var room = _roomRepository.FindById(contract.RoomId);
                    if (rentalPayment.Type == PaymentTypes.Rent)
                    {
                        contract.LastRentalPayment = rentalPayment;
                        _rentalContratRepository.InsertUpdate(contract);
                        room.LastRentalPayment = rentalPayment;
                        room.LastRenteeId = contract.RenteeId;
                        if (rentalPayment.ServicePaymentId != null && rentalPayment.ServicePaymentId != 0)
                            room.LastServicePaymentId = rentalPayment.ServicePaymentId;
                        _roomRepository.InsertUpdate(room);
                    }
                    else if(rentalPayment.Type == PaymentTypes.Service)
                    {
                        room.LastServicePayment = rentalPayment;
                        _roomRepository.InsertUpdate(room);
                    }
                }

                if (paymentsDtos != null)
                    foreach (var paymentDTO in paymentsDtos)
                    {
                        if (paymentDTO.Id != 0)
                            _paymentRepository.Delete(paymentDTO.Id);
                    }

                _rentalPaymentRepository.InsertUpdate(rentalPayment);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
        
        public string Disable(RentalPaymentDTO rentalPayment)
        {
            if (rentalPayment == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _rentalPaymentRepository.Update(rentalPayment);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string rentalPaymentId)
        {
            try
            {
                var rentId = Convert.ToInt32(rentalPaymentId);
                _rentalPaymentRepository.Delete(rentId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(RentalPaymentDTO rentalPayment)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<RentalPaymentDTO>(iDbContext);
                var catExists =
                    catRepository.Query()
                        .Filter(bp => bp.ReceiptNumber == rentalPayment.ReceiptNumber && bp.Id != rentalPayment.Id)
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

        public string Validate(RentalPaymentDTO rentalPayment)
        {
            if (null == rentalPayment)
                return GenericMessages.ObjectIsNull;

            return string.Empty;
        }

        #endregion

        #region Private Methods

        public string GetRentalPaymentNumber(int rentalPaymentId)
        {
            var pref = rentalPaymentId.ToString();
            if (rentalPaymentId < 1000)
            {
                var id = rentalPaymentId + 10000;
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