using System;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface ICompanyService : IDisposable
    {
        CompanyDTO GetCompany();
        string InsertOrUpdate(CompanyDTO client);
        //string Disable(CompanyDTODTO client);
        //int Delete(string companyDTOId);
    }
}