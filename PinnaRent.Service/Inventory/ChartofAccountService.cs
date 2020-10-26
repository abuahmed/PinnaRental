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
    public class ChartofAccountService : IChartofAccountService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<ChartofAccountDTO> _chartofAccountRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor

        public ChartofAccountService()
        {
            InitializeDbContext();
        }
        public ChartofAccountService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _chartofAccountRepository = new Repository<ChartofAccountDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<ChartofAccountDTO> Get()
        {
            var piList = _chartofAccountRepository
                .Query()
                .Include(a => a.AccountType)
                .Filter(a => !string.IsNullOrEmpty(a.AccountId))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<ChartofAccountDTO> GetAll(SearchCriteria<ChartofAccountDTO> criteria = null)
        {
            IEnumerable<ChartofAccountDTO> accountList = new List<ChartofAccountDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {
                        var pdto = Get();

                        foreach (var cri in criteria.FiList)
                        {
                            pdto.FilterList(cri);
                        }
                    
                        IList<ChartofAccountDTO> pdtoList;
                        if (criteria.Page != 0 && criteria.PageSize != 0)
                        {
                            int totalCount;
                            pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                        }
                        else
                            pdtoList = pdto.GetList().ToList();

                        accountList = accountList.Concat(pdtoList).ToList();
                   
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

        public ChartofAccountDTO Find(string chartofAccountId)
        {
            return _chartofAccountRepository.FindById(Convert.ToInt32(chartofAccountId));
        }

        public ChartofAccountDTO GetByName(string accountNumber)
        {
            var cat = _chartofAccountRepository
                .Query()
                .Filter(c => c.AccountId == accountNumber)
                .Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(ChartofAccountDTO chartofAccount)
        {
            try
            {
                var validate = Validate(chartofAccount);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(chartofAccount))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _chartofAccountRepository.InsertUpdate(chartofAccount);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(ChartofAccountDTO chartofAccount)
        {
            if (chartofAccount == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _chartofAccountRepository.Update(chartofAccount);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string chartofAccountId)
        {
            try
            {
                _chartofAccountRepository.Delete(chartofAccountId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(ChartofAccountDTO chartofAccount)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<ChartofAccountDTO>(iDbContext);
                var catExists = catRepository
                    .Query()
                    .Filter(bp => bp.AccountId == chartofAccount.AccountId && bp.Id != chartofAccount.Id)
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

        public string Validate(ChartofAccountDTO chartofAccount)
        {
            if (null == chartofAccount)
                return GenericMessages.ObjectIsNull;
            
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