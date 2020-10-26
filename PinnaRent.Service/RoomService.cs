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
    public class RoomService : IRoomService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<RoomDTO> _roomRepository;
        private IRepository<RentalContratDTO> _contractRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public RoomService()
        {
            InitializeDbContext();
        }

        public RoomService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _roomRepository = new Repository<RoomDTO>(iDbContext);
            _contractRepository = new Repository<RentalContratDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<RoomDTO> Get()
        {
            var piList = _roomRepository
              .Query()
              .Include(a => a.Floor)
              .Include(a => a.LastRentee).Include(a => a.LastRentee.Address)
              .Include(a => a.LastRentalPayment)
              .Include(a => a.LastRentalPayment.PaymentRemarks)
              .Include(a => a.RoomContrats)
              .Include(a => a.LastRentDeposit)
              .Include(a => a.LastServicePayment)
              .Filter(a => !string.IsNullOrEmpty(a.Number))
              .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<RoomDTO> GetAll(SearchCriteria<RoomDTO> criteria = null)
        {
            IEnumerable<RoomDTO> piList;// = new List<RoomDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<RoomDTO> pdtoList;
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

        public RoomDTO Find(string roomId)
        {
            var orgDto = _roomRepository.FindById(Convert.ToInt32(roomId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public RoomDTO GetByName(string displayName)
        {
            var cat = _roomRepository.Query().Filter(c => c.Number == displayName).Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(RoomDTO room)
        {
            try
            {
                var validate = Validate(room);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(room))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                if (room.ContractDiscontinuedId != 0)
                {
                    var contrat = _contractRepository.FindById(room.ContractDiscontinuedId);
                    contrat.Discontinued = true;
                    contrat.LastContractDiscontinuedDate = room.LastRoomReleasedDate;
                    _contractRepository.InsertUpdate(contrat);
                }

                _roomRepository.InsertUpdate(room);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(RoomDTO room)
        {
            if (room == null)
                return GenericMessages.ObjectIsNull;
            
            string stat;
            try
            {
                _roomRepository.Update(room);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string roomId)
        {
            try
            {
                _roomRepository.Delete(roomId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(RoomDTO room)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<RoomDTO>(iDbContext);
                var catExists = catRepository.Query().Filter(bp => bp.Number == room.Number && bp.Id != room.Id)
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

        public string Validate(RoomDTO room)
        {
            if (null == room || room.RentalFee==null)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(room.Number))
                return room.Number + " " + GenericMessages.StringIsNullOrEmpty;

            if (room.Number.Length > 50)
                return room.Number + " can not be more than 50 characters ";

            if (room.RentalFee<=0)
                return room.Number + " fee can not be less than 0";

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

                var lastItem = new RoomService(true).GetLastItemIncludingDeleted();

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
                //MessageBox.Show("Problem getting room number, try again...");
                return null;// prefix + (SelectedWarehouse.WarehouseNumber * 100000).ToString() + "0001";
            }
        }
        public string GetItemNumber(int roomId)
        {
            var pref = roomId.ToString();
            if (roomId < 1000)
            {
                var id = roomId + 10000;
                pref = id.ToString();
                pref = pref.Substring(1);
            }
            //var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
            return "DI" + pref;// + "/" + amhCalender.Substring(6);
        }
        public RoomDTO GetLastItemIncludingDeleted()
        {
            RoomDTO piList;
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