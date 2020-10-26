using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IBankAccountService : IDisposable
    {
        IEnumerable<BankAccountDTO> GetAll(SearchCriteria<BankAccountDTO> criteria = null);
        BankAccountDTO Find(string bankAccountId);
        BankAccountDTO GetByName(string displayName);
        string InsertOrUpdate(BankAccountDTO bankAccount);
        string Disable(BankAccountDTO bankAccount);
        int Delete(string bankAccountId);
    }
}