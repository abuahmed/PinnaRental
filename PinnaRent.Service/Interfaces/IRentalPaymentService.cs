using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRentalPaymentService : IDisposable
    {
        IEnumerable<RentalPaymentDTO> GetAll(SearchCriteria<RentalPaymentDTO> criteria = null);
        IEnumerable<RentalPaymentDTO> GetAll(SearchCriteria<RentalPaymentDTO> criteria, out int totalCount);
        RentalPaymentDTO Find(string rentalPaymentId);
        RentalPaymentDTO GetByName(string displayName);
        string InsertOrUpdate(RentalPaymentDTO rentalPayment);
        string InsertOrUpdateWithPayment(RentalPaymentDTO rentalPayment, IEnumerable<PaymentDTO> paymentsDtos);
        string Disable(RentalPaymentDTO rentalPayment);
        int Delete(string rentalPaymentId);
        string GetRentalPaymentNumber(int rentalPaymentId);
    }
}