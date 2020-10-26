using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRoomResourceService : IDisposable
    {
        IEnumerable<RoomResourceDTO> GetAll(SearchCriteria<RoomResourceDTO> criteria = null);
        RoomResourceDTO Find(string roomResourceId);
        RoomResourceDTO GetByName(string displayName);
        string InsertOrUpdate(RoomResourceDTO roomResource);
        string Disable(RoomResourceDTO roomResource);
        int Delete(string roomResourceId);
        string GetItemCode();
        string GetItemNumber(int roomResourceId);
    }
}