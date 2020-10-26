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
    public class ItemQuantityService : IItemQuantityService
    {
        #region Fields

        private IDbContext _iDbContext;
        private IUnitOfWork _unitOfWork;
        private IRepository<ItemQuantityDTO> _itemQuantityRepository;
        //private IRepository<PhysicalInventoryLineDTO> _piRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public ItemQuantityService()
        {
            InitializeDbContext();
        }
        public ItemQuantityService(IDbContext iDbContext)
        {
            _iDbContext = iDbContext;
            _unitOfWork = new UnitOfWork(_iDbContext);
            _itemQuantityRepository = _unitOfWork.Repository<ItemQuantityDTO>();
            //_piRepository = _unitOfWork.Repository<PhysicalInventoryLineDTO>();

        }
        public ItemQuantityService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _unitOfWork = new UnitOfWork(_iDbContext);
            _itemQuantityRepository = _unitOfWork.Repository<ItemQuantityDTO>();
            //_piRepository = _unitOfWork.Repository<PhysicalInventoryLineDTO>();
        }

        #endregion

        #region Common Methods

        public IRepositoryQuery<ItemQuantityDTO> Get()
        {
            var piList = _itemQuantityRepository
                .Query()
                .Include(i => i.Item, i => i.Item.Category, i => i.Item.UnitOfMeasure, i => i.Warehouse)
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<ItemQuantityDTO> GetAll(SearchCriteria<ItemQuantityDTO> criteria = null)
        {
            int totalCount = 0;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<ItemQuantityDTO> GetAll(SearchCriteria<ItemQuantityDTO> criteria, out int totalCount)
        {
            IEnumerable<ItemQuantityDTO> catList = new List<ItemQuantityDTO>();
            totalCount = 0;
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {
                    var warehouseList = new WarehouseService(true)
                        .GetWarehousesPrevilegedToUser(criteria.CurrentUserId).ToList();
                    if (criteria.SelectedWarehouseId != null)
                        warehouseList = warehouseList.Where(w => w.Id == criteria.SelectedWarehouseId).ToList();

                    foreach (var warehouse in warehouseList.Where(w => w.Id != -1))
                    {
                        var pdto = Get();

                        foreach (var cri in criteria.FiList)
                        {
                            pdto.FilterList(cri);
                        }

                        #region By Warehouse
                        var warehouse1 = warehouse;
                        pdto.FilterList(p => p.WarehouseId == warehouse1.Id);
                        #endregion

                        IList<ItemQuantityDTO> pdtoList;
                        if (criteria.Page != 0 && criteria.PageSize != 0)
                        {
                            int totalCount2;
                            pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount2).ToList();
                            totalCount = totalCount2;
                        }
                        else
                            pdtoList = pdto.GetList().ToList();

                        catList = catList.Concat(pdtoList).ToList();
                    }
                }
                else
                    catList = Get().Get().ToList();
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catList;
        }

        public ItemQuantityDTO Find(string itemQuantityId)
        {
            var orgDto = _itemQuantityRepository.FindById(Convert.ToInt32(itemQuantityId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public ItemQuantityDTO GetByName(string displayName)
        {
            return null;
        }

        public ItemQuantityDTO GetByCriteria(int warehouseId, int itemId)
        {
            var itQuant = Get()
                .Filter(c => c.WarehouseId == warehouseId && c.ItemId == itemId)
                .Get()
                .FirstOrDefault();
            return itQuant;
        }

        public ItemQuantityDTO GetByIq(ItemQuantityDTO itemQuantity)
        {
            var iq = GetByCriteria(itemQuantity.WarehouseId, itemQuantity.ItemId);
            if (iq == null)
            {
                iq = itemQuantity;
                iq.Item = _unitOfWork.Repository<ItemDTO>().FindById(iq.ItemId);
                iq.Warehouse = _unitOfWork.Repository<WarehouseDTO>().FindById(iq.WarehouseId);
            }
            else
                iq.QuantityOnHand = itemQuantity.QuantityOnHand;

            return iq;
        }

        public string InsertOrUpdate(ItemQuantityDTO itemQuantity, bool insertPiLine)
        {
            try
            {
                var itemQty = GetByIq(itemQuantity);

                var validate = Validate(itemQty);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(itemQty))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _itemQuantityRepository.InsertUpdate(itemQty);


                //if (insertPiLine)
                //{
                //    var line = new PiHeaderService().GetNewPiLine(itemQty.WarehouseId, itemQty.ItemId,
                //        itemQty.QuantityOnHand, PhysicalInventoryLineTypes.ItemEntry);

                //    _piRepository.InsertUpdate(line);
                //}

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public ItemQuantityDTO InsertOrUpdate(ItemQuantityDTO itemQuantity)
        {
            try
            {
                var itemQty = GetByIq(itemQuantity);

                //var validate = Validate(itemQty);
                //if (!string.IsNullOrEmpty(validate))
                //    return null;

                //if (ObjectExists(itemQty))
                //    return null;

                return itemQuantity;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<ItemQuantityDTO> UpdateInventoryByTransaction(TransactionHeaderDTO transaction)
        {
            var itemsQuantityRepository = new List<ItemQuantityDTO>();
            try
            {
                foreach (var line in transaction.TransactionLines.Where(t => t.Enabled))
                {
                    var iQty = GetByCriteria(transaction.WarehouseId, line.ItemId);
                    if (transaction.TransactionType == TransactionTypes.SellStock || transaction.TransactionType == TransactionTypes.UseStock)
                    {
                        if (iQty == null) //if item not found in stock //make sure it should never become null
                        {
                            return null; // "Item not found in store";
                        }

                        if (iQty.QuantityOnHand < line.Unit) //If PostWithLessStock is not allowed
                        {
                            return null;
                            // "The Store has less than " + itemQty.QuantityOnHand.ToString() + "  '" + line.Item.DisplayName + "'";
                        }
                        iQty.QuantityOnHand = iQty.QuantityOnHand - line.Unit;
                        itemsQuantityRepository.Add(iQty);
                    }
                    else if (transaction.TransactionType == TransactionTypes.RecieveStock)
                    {
                        if (iQty == null)
                        {
                            iQty = new ItemQuantityDTO()
                            {
                                ItemId = line.ItemId,
                                WarehouseId = transaction.WarehouseId,
                                QuantityOnHand = 0
                            };
                        }
                        iQty.QuantityOnHand = iQty.QuantityOnHand + line.Unit;
                        itemsQuantityRepository.Add(iQty);
                    }
                    else if (transaction.TransactionType == TransactionTypes.TransferStock)
                    {
                        if (transaction.ToWarehouseId == null)
                            return null;

                        if (transaction.Status == TransactionStatus.Requested)
                        {
                            if (iQty == null)
                            {
                                iQty = new ItemQuantityDTO()
                                {
                                    ItemId = line.ItemId,
                                    WarehouseId = transaction.WarehouseId,
                                    QuantityOnHand = 0
                                };
                            }
                            if (iQty.QuantityOnHand < line.Unit) //If PostWithLessStock is not allowed
                            {
                                return null;
                            }
                            iQty.QuantityOnHand = iQty.QuantityOnHand - line.Unit;
                            itemsQuantityRepository.Add(iQty);
                        }
                        else if (transaction.Status == TransactionStatus.Sent)
                        {
                            var destWarehouse = (int) transaction.ToWarehouseId;
                            var iQtyDest = GetByCriteria(destWarehouse, line.ItemId);
                            if (iQtyDest == null)
                            {
                                iQtyDest = new ItemQuantityDTO()
                                {
                                    ItemId = line.ItemId,
                                    WarehouseId = destWarehouse,
                                    QuantityOnHand = 0
                                };
                            }
                            iQtyDest.QuantityOnHand = iQtyDest.QuantityOnHand + line.Unit;
                            itemsQuantityRepository.Add(iQtyDest);
                        }
                    }

                    
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                //Dispose(true);
            }

            return itemsQuantityRepository;
        }

        public string Disable(ItemQuantityDTO itemQuantity)
        {
            if (itemQuantity == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _itemQuantityRepository.Update(itemQuantity);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string itemQuantityId)
        {
            try
            {
                _itemQuantityRepository.Delete(itemQuantityId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(ItemQuantityDTO itemQuantity)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<ItemQuantityDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.WarehouseId == itemQuantity.WarehouseId && bp.ItemId == itemQuantity.ItemId) && bp.Id != itemQuantity.Id)
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

        public string Validate(ItemQuantityDTO itemQuantity)
        {
            if (null == itemQuantity)
                return GenericMessages.ObjectIsNull;

            if (itemQuantity.WarehouseId == 0)
                return "Warehouse Id " + GenericMessages.ObjectIsNull;

            if (itemQuantity.ItemId == 0)
                return "Item Id " + GenericMessages.ObjectIsNull;

            return string.Empty;
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