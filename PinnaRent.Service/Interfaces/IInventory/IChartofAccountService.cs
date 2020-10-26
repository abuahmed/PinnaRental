using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IChartofAccountService : IDisposable
    {
        IEnumerable<ChartofAccountDTO> GetAll(SearchCriteria<ChartofAccountDTO> criteria = null);
        ChartofAccountDTO Find(string chartofAccountId);
        ChartofAccountDTO GetByName(string displayName);
        string InsertOrUpdate(ChartofAccountDTO chartofAccount);
        string Disable(ChartofAccountDTO chartofAccount);
        int Delete(string chartofAccountId);
    }
}