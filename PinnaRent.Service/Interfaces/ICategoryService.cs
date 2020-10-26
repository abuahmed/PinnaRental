using System;
using System.Collections.Generic;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface ICategoryService : IDisposable
    {
        IEnumerable<CategoryDTO> GetAll(SearchCriteria<CategoryDTO> criteria = null);
        CategoryDTO Find(string categoryId);
        CategoryDTO GetByName(string displayName);
        string InsertOrUpdate(CategoryDTO category);
        string Disable(CategoryDTO category);
        int Delete(string categoryId);
    }
}