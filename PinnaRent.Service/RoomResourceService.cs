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
    public class RoomResourceService : IRoomResourceService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<RoomResourceDTO> _roomResourceRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public RoomResourceService()
        {
            InitializeDbContext();
        }

        public RoomResourceService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _roomResourceRepository = new Repository<RoomResourceDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<RoomResourceDTO> Get()
        {
            var piList = _roomResourceRepository
              .Query()
              .Include(a => a.Room)
              .Include(a => a.Category)
              .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<RoomResourceDTO> GetAll(SearchCriteria<RoomResourceDTO> criteria = null)
        {
            IEnumerable<RoomResourceDTO> piList;// = new List<RoomResourceDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<RoomResourceDTO> pdtoList;
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

        public RoomResourceDTO Find(string roomResourceId)
        {
            var orgDto = _roomResourceRepository.FindById(Convert.ToInt32(roomResourceId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public RoomResourceDTO GetByName(string displayName)
        {
            //var cat = _roomResourceRepository.Query().Filter(c => c.Number == displayName).Get().FirstOrDefault();
            //return cat;
            return null;
        }

        public string InsertOrUpdate(RoomResourceDTO roomResource)
        {
            try
            {
                var validate = Validate(roomResource);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(roomResource))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _roomResourceRepository.InsertUpdate(roomResource);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(RoomResourceDTO roomResource)
        {
            if (roomResource == null)
                return GenericMessages.ObjectIsNull;

            //if (_unitOfWork.Repository<ItemQuantityDTO>().Query().Get().Any(i => i.ItemId == roomResource.Id) ||
            //    _unitOfWork.Repository<PhysicalInventoryLineDTO>().Query().Get().Any(i => i.ItemId == roomResource.Id) ||
            //    _unitOfWork.Repository<TransactionLineDTO>().Query().Get().Any(i => i.ItemId == roomResource.Id) ||
            //    _unitOfWork.Repository<ItemBorrowDTO>().Query().Get().Any(i => i.ItemId == roomResource.Id))
            //{
            //    return "Can't delete the roomResource, it is already in use...";
            //}

            string stat;
            try
            {
                _roomResourceRepository.Update(roomResource);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string roomResourceId)
        {
            try
            {
                var roomresId = Convert.ToInt32(roomResourceId);
                _roomResourceRepository.Delete(roomresId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(RoomResourceDTO roomResource)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<RoomResourceDTO>(iDbContext);
                var catExists = catRepository.Query().Filter(bp => bp.RoomId == roomResource.RoomId && bp.CategoryId == roomResource.CategoryId && bp.Id != roomResource.Id)
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

        public string Validate(RoomResourceDTO roomResource)
        {
            if (null == roomResource || roomResource.RoomId==0)
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

                var lastItem = new RoomResourceService(true).GetLastItemIncludingDeleted();

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
                //MessageBox.Show("Problem getting roomResource number, try again...");
                return null;// prefix + (SelectedWarehouse.WarehouseNumber * 100000).ToString() + "0001";
            }
        }
        public string GetItemNumber(int roomResourceId)
        {
            var pref = roomResourceId.ToString();
            if (roomResourceId < 1000)
            {
                var id = roomResourceId + 10000;
                pref = id.ToString();
                pref = pref.Substring(1);
            }
            //var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
            return "DI" + pref;// + "/" + amhCalender.Substring(6);
        }
        public RoomResourceDTO GetLastItemIncludingDeleted()
        {
            RoomResourceDTO piList;
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