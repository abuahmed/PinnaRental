#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PinnaRent.Core;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

#endregion

namespace PinnaRent.Service
{
    public class RentDepositService : IRentDepositService
    {
        #region Fields

        private IUnitOfWork _unitOfWork;
        private IRepository<RentDepositDTO> _rentDepositRepository;
        private IRepository<RentalContratDTO> _rentalContratRepository;
        private IRepository<RoomDTO> _roomRepository;
        private readonly bool _disposeWhenDone;

        #endregion

        #region Constructor

        public RentDepositService()
        {
            InitializeDbContext();
        }

        public RentDepositService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _rentDepositRepository = new Repository<RentDepositDTO>(iDbContext);
            _rentalContratRepository = new Repository<RentalContratDTO>(iDbContext);
            _roomRepository = new Repository<RoomDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods

        public IRepositoryQuery<RentDepositDTO> Get()
        {
            var piList = _rentDepositRepository
                .Query()
                .Include(a => a.Contrat.Room).Include(a => a.Contrat.Room.Floor)
                .Include(a => a.Contrat)
                .Include(a => a.Contrat.Rentee).Include(a => a.Contrat.Rentee.Address)
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<RentDepositDTO> GetAll(SearchCriteria<RentDepositDTO> criteria = null)
        {
            int totalCount = 0;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<RentDepositDTO> GetAll(SearchCriteria<RentDepositDTO> criteria, out int totalCount)
        {
            IEnumerable<RentDepositDTO> piList; // = new List<RentDepositDTO>();
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
                        if (!criteria.IncludePhoto)
                            pdto.FilterList(p => p.DepositedDate >= beginDate);
                        else if (criteria.IncludePhoto)
                            pdto.FilterList(p => p.ReturnedDate >= beginDate);
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        if (!criteria.IncludePhoto)
                            pdto.FilterList(p => p.DepositedDate <= endDate);
                        if (criteria.IncludePhoto)
                            pdto.FilterList(p => p.ReturnedDate <= endDate);
                    }

                    IList<RentDepositDTO> pdtoList;
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
                    piList = Get().Get().OrderBy(i => i.Id).ToList();//.AsNoTracking()
                }
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return piList;
        }

        public RentDepositDTO Find(string rentDepositId)
        {
            var orgDto = _rentDepositRepository.FindById(Convert.ToInt32(rentDepositId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public RentDepositDTO GetByName(string displayName)
        {
            var cat = _rentDepositRepository
                .Query()
                //.Filter(c => c.PaymentNumber == displayName)
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(RentDepositDTO rentDeposit)
        {
            try
            {
                var validate = Validate(rentDeposit);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(rentDeposit))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                var contract = _rentalContratRepository.FindById(rentDeposit.ContratId);
                if (rentDeposit.ReturnedDate == null && rentDeposit.UsedDate == null)
                    contract.LastRentDeposit = rentDeposit;
                else contract.LastRentDeposit = null;
                _rentalContratRepository.InsertUpdate(contract);

                var room = _roomRepository.FindById(contract.RoomId);
                if (rentDeposit.ReturnedDate == null && rentDeposit.UsedDate == null)
                    room.LastRentDeposit = rentDeposit;
                else room.LastRentDeposit = null;
                _roomRepository.InsertUpdate(room);

                _rentDepositRepository.InsertUpdate(rentDeposit);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        //public string InsertOrUpdateRange(IEnumerable<RentDepositDTO> rentDeposits)
        //{
        //    try
        //    {
        //        var validate = Validate(rentDeposits);
        //        if (!string.IsNullOrEmpty(validate))
        //            return validate;

        //        if (ObjectExists(rentDeposits))
        //            return GenericMessages.DatabaseErrorRecordAlreadyExists;

        //        var contrat = _rentalContratRepository.FindById(rentDeposits.ContratId);
        //        contrat.LastRentDeposit = rentDeposits;
        //        _rentalContratRepository.InsertUpdate(contrat);

        //        _rentDepositRepository.InsertUpdate(rentDeposits);

        //        _unitOfWork.Commit();
        //        return string.Empty;
        //    }
        //    catch (Exception exception)
        //    {
        //        return exception.Message;
        //    }
        //}

        public string Disable(RentDepositDTO rentDeposit)
        {
            if (rentDeposit == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _rentDepositRepository.Update(rentDeposit);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string rentDepositId)
        {
            try
            {
                _rentDepositRepository.Delete(rentDepositId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(RentDepositDTO rentDeposit)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<RentDepositDTO>(iDbContext);
            //    var catExists = catRepository.Query().Filter(bp => bp.ReceiptNumber == rentDeposit.ReceiptNumber && bp.Id != rentDeposit.Id)
            //        .Get()
            //        .FirstOrDefault();

            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            return objectExists;
        }

        public string Validate(RentDepositDTO rentDeposit)
        {
            if (null == rentDeposit)
                return GenericMessages.ObjectIsNull;

            return string.Empty;
        }

        #endregion

        #region Private Methods

        public string GetRentDepositNumber(int rentDepositId)
        {
            var pref = rentDepositId.ToString();
            if (rentDepositId < 1000)
            {
                var id = rentDepositId + 10000;
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