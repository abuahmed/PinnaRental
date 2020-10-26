using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IRoomService : IDisposable
    {
        IEnumerable<RoomDTO> GetAll(SearchCriteria<RoomDTO> criteria = null);
        RoomDTO Find(string roomId);
        RoomDTO GetByName(string displayName);
        string InsertOrUpdate(RoomDTO room);
        string Disable(RoomDTO room);
        int Delete(string roomId);
        string GetItemCode();
        string GetItemNumber(int roomId);
    }
}