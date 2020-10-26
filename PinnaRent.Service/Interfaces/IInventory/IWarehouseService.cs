using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IWarehouseService : IDisposable
    {
        IEnumerable<WarehouseDTO> GetAll(SearchCriteria<WarehouseDTO> criteria = null);
        WarehouseDTO Find(string warehouseId);
        WarehouseDTO GetByName(string displayName);
        string InsertOrUpdate(WarehouseDTO warehouse);
        string Disable(WarehouseDTO warehouse);
        int Delete(string warehouseId);
        IEnumerable<WarehouseDTO> GetWarehousesPrevilegedToUser(int userId);
        string GetWarehouseNumber(int warehouseId);
    }
}