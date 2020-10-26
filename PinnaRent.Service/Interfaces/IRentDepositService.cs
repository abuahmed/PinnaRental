using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRentDepositService : IDisposable
    {
        IEnumerable<RentDepositDTO> GetAll(SearchCriteria<RentDepositDTO> criteria = null);
        IEnumerable<RentDepositDTO> GetAll(SearchCriteria<RentDepositDTO> criteria, out int totalCount);
        RentDepositDTO Find(string rentDepositId);
        RentDepositDTO GetByName(string displayName);
        string InsertOrUpdate(RentDepositDTO rentDeposit);
        string Disable(RentDepositDTO rentDeposit);
        int Delete(string rentDepositId);
        string GetRentDepositNumber(int rentDepositId);
    }
}