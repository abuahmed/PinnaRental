using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaRent.Core;
using PinnaRent.Core.Models;

namespace PinnaRent.Service.Interfaces
{
    public interface IUserService : IDisposable
    {
        IEnumerable<UserDTO> GetAll();
        IEnumerable<RoleDTO> GetAllRoles();
        UserDTO GetUser(int userId);
        UserDTO GetByName(string displayName);
        string InsertOrUpdate(UserDTO user);
        string Disable(UserDTO user);
        int Delete(UserDTO user);
    }
}