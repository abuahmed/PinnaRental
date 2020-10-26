using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IBusinessPartnerService : IDisposable
    {
        IEnumerable<BusinessPartnerDTO> GetAll(SearchCriteria<BusinessPartnerDTO> criteria = null);
        IEnumerable<BusinessPartnerDTO> GetAll(SearchCriteria<BusinessPartnerDTO> criteria, out int totalCount);
        BusinessPartnerDTO Find(string businessPartnerId);
        BusinessPartnerDTO GetByName(string displayName);
        string InsertOrUpdate(BusinessPartnerDTO businessPartner);
        string Disable(BusinessPartnerDTO businessPartner);
        int Delete(string businessPartnerId);
        string GetBusinessPartnerNumber(int businessPartnerId);
    }
}