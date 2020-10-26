using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRentalPaymentRemarkService : IDisposable
    {
        IEnumerable<RentalPaymentRemarkDTO> GetAll(SearchCriteria<RentalPaymentRemarkDTO> criteria = null);
        RentalPaymentRemarkDTO Find(string rentalPaymentRemarkId);
        RentalPaymentRemarkDTO GetByName(string displayName);
        string InsertOrUpdate(RentalPaymentRemarkDTO rentalPaymentRemark);
        string Disable(RentalPaymentRemarkDTO rentalPaymentRemark);
        int Delete(string rentalPaymentRemarkId);
        string GetItemCode();
        string GetItemNumber(int rentalPaymentRemarkId);
    }
}