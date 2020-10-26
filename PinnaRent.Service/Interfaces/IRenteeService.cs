using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRenteeService : IDisposable
    {
        IEnumerable<RenteeDTO> GetAll(SearchCriteria<RenteeDTO> criteria = null);
        IEnumerable<RenteeDTO> GetAll(SearchCriteria<RenteeDTO> criteria, out int totalCount);
        RenteeDTO Find(string renteeId);
        RenteeDTO GetByName(string displayName);
        string InsertOrUpdate(RenteeDTO rentee);
        string Disable(RenteeDTO rentee);
        int Delete(string renteeId);
        //string GetRenteeNumber(int renteeId);
    }
}