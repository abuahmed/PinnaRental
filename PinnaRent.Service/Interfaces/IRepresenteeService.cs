using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRepresenteeService : IDisposable
    {
        IEnumerable<RepresenteeDTO> GetAll(SearchCriteria<RepresenteeDTO> criteria = null);
        IEnumerable<RepresenteeDTO> GetAll(SearchCriteria<RepresenteeDTO> criteria, out int totalCount);
        RepresenteeDTO Find(string representeeId);
        RepresenteeDTO GetByName(string displayName);
        string InsertOrUpdate(RepresenteeDTO representee);
        string Disable(RepresenteeDTO representee);
        int Delete(string representeeId);
        string GetRepresenteeNumber(int representeeId);
    }
}