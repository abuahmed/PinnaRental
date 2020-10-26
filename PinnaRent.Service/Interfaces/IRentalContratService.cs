using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRentalContratService : IDisposable
    {
        IEnumerable<RentalContratDTO> GetAll(SearchCriteria<RentalContratDTO> criteria = null);
        IEnumerable<RentalContratDTO> GetAll(SearchCriteria<RentalContratDTO> criteria, out int totalCount);
        RentalContratDTO Find(string rentalContratId);
        RentalContratDTO GetByName(string displayName);
        string InsertOrUpdate(RentalContratDTO rentalContrat);
        string Disable(RentalContratDTO rentalContrat);
        int Delete(string rentalContratId);
       // string GetRentalContratNumber(int rentalContratId);
    }
}