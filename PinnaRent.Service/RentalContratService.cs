using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
//using System.Windows.Forms;
using PinnaRent.Core;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

namespace PinnaRent.Service
{
    public class RentalContratService : IRentalContratService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<RentalContratDTO> _rentalContratRepository;
        private IRepository<RoomDTO> _roomRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public RentalContratService()
        {
            InitializeDbContext();
        }

        public RentalContratService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _rentalContratRepository = new Repository<RentalContratDTO>(iDbContext);
            _roomRepository = new Repository<RoomDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<RentalContratDTO> Get()
        {
            var piList = _rentalContratRepository
              .Query()
              .Include(a => a.Room).Include(a=>a.Room.Floor)//.Include(a=>a.Room.RoomContrats)
              .Include(a => a.Rentee).Include(a => a.Rentee.Address)//.Include(a => a.Payments)
              //.Filter(a => !string.IsNullOrEmpty(a.RentalContratNumber))
              .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<RentalContratDTO> GetAll(SearchCriteria<RentalContratDTO> criteria = null)
        {
            int totalCount = 0;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<RentalContratDTO> GetAll(SearchCriteria<RentalContratDTO> criteria, out int totalCount)
        {
            IEnumerable<RentalContratDTO> piList;// = new List<RentalContratDTO>();
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
                        pdto.FilterList(p =>p.LastContractDiscontinuedDate!=null && p.LastContractDiscontinuedDate >= beginDate);
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        pdto.FilterList(p => p.LastContractDiscontinuedDate != null && p.LastContractDiscontinuedDate <= endDate);
                    }

                    IList<RentalContratDTO> pdtoList;
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

        public RentalContratDTO Find(string rentalContratId)
        {
            var orgDto = _rentalContratRepository.FindById(Convert.ToInt32(rentalContratId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public RentalContratDTO GetByName(string displayName)
        {
            var cat = _rentalContratRepository.Query()
                //.Filter(c => c.RentalContratNumber == displayName)
                .Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(RentalContratDTO rentalContrat)
        {
            try
            {
                var validate = Validate(rentalContrat);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(rentalContrat))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                ////if the Contrat is the last one
                //var room = _roomRepository.FindById(rentalContrat.RoomId);
                //room.LastRenteeId = rentalContrat.RenteeId;
                //_roomRepository.InsertUpdate(room);

                _rentalContratRepository.InsertUpdate(rentalContrat);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(RentalContratDTO rentalContrat)
        {
            if (rentalContrat == null)
                return GenericMessages.ObjectIsNull;
            
            string stat;
            try
            {
                _rentalContratRepository.Update(rentalContrat);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string rentalContratId)
        {
            try
            {
                _rentalContratRepository.Delete(rentalContratId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(RentalContratDTO rentalContrat)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<RentalContratDTO>(iDbContext);
            //    var catExists = catRepository.Query().Filter(bp => bp.RentalContratNumber == rentalContrat.RentalContratNumber && bp.Id != rentalContrat.Id)
            //            .Get()
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

        public string Validate(RentalContratDTO rentalContrat)
        {
            if (null == rentalContrat)// || rentalContrat.Room == null || rentalContrat.Rentee == null)
                return GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(rentalContrat.RentalContratNumber))
            //    return rentalContrat.RentalContratNumber + " " + GenericMessages.StringIsNullOrEmpty;

            //if (rentalContrat.RentalContratNumber.Length > 50)
            //    return rentalContrat.RentalContratNumber + " can not be more than 50 characters ";

            //if (rentalContrat.RentalFee<=0)
            //    return rentalContrat.RentalContratNumber + " fee can not be less than 0";

            return string.Empty;
        }

        #endregion

        #region Private Methods
        //public string GetRentalContratNumber(int rentalContratId)
        //{
        //    var pref = rentalContratId.ToString();
        //    if (rentalContratId < 1000)
        //    {
        //        var id = rentalContratId + 10000;
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