using System;
using System.Collections.Generic;
using System.Linq;
using PinnaRent.Core;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Repository;
using PinnaRent.Repository.Interfaces;
using PinnaRent.Service.Interfaces;

namespace PinnaRent.Service
{
    public class BankAccountService : IBankAccountService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<BankAccountDTO> _bankAccountRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor

        public BankAccountService()
        {
            InitializeDbContext();
        }
        public BankAccountService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _bankAccountRepository = new Repository<BankAccountDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<BankAccountDTO> Get()
        {
            var piList = _bankAccountRepository
                .Query()
                .Include(a => a.Company)
                .Filter(a => !string.IsNullOrEmpty(a.AccountNumber))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }
        
        public IEnumerable<BankAccountDTO> GetAll(SearchCriteria<BankAccountDTO> criteria = null)
        {
            IEnumerable<BankAccountDTO> accountList = new List<BankAccountDTO>();
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

                        //#region By Warehouse
                        //var warehouse1 = warehouse;
                        //pdto.FilterList(p => p.CompanyId == warehouse1.Id);
                        //#endregion
                        
                        IList<BankAccountDTO> pdtoList;
                        if (criteria.Page != 0 && criteria.PageSize != 0)
                        {
                            int totalCount;
                            pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                        }
                        else
                            pdtoList = pdto.GetList().ToList();

                        accountList = accountList.Concat(pdtoList).ToList();
                    }
                }
                else
                    accountList = Get().Get().ToList();
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return accountList;
        }
        
        public BankAccountDTO Find(string bankAccountId)
        {
            return _bankAccountRepository.FindById(Convert.ToInt32(bankAccountId));
        }

        public BankAccountDTO GetByName(string accountNumber)
        {
            var cat = _bankAccountRepository
                .Query()
                .Filter(c => c.AccountNumber == accountNumber)
                .Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(BankAccountDTO bankAccount)
        {
            try
            {
                var validate = Validate(bankAccount);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(bankAccount))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _bankAccountRepository.InsertUpdate(bankAccount);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(BankAccountDTO bankAccount)
        {
            if (bankAccount == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _bankAccountRepository.Update(bankAccount);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string bankAccountId)
        {
            try
            {
                _bankAccountRepository.Delete(bankAccountId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(BankAccountDTO bankAccount)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<BankAccountDTO>(iDbContext);
                var catExists = catRepository
                    .Query()
                    .Filter(bp => bp.BankName == bankAccount.BankName && bp.AccountNumber == bankAccount.AccountNumber && bp.Id != bankAccount.Id)
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

        public string Validate(BankAccountDTO bankAccount)
        {
            if (null == bankAccount)
                return GenericMessages.ObjectIsNull;

            //if (bankAccount.WarehouseId != 0 && new WarehouseService(true).Find(bankAccount.WarehouseId.ToString()) == null)
            //    return "Warehouse is null/disabled ";

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