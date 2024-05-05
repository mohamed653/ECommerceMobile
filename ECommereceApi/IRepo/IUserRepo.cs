using ECommereceApi.DTOs;
using ECommereceApi.Models;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IUserRepo
    {
        IEnumerable<UserDTO> GetUsers();
        UserDTO GetUser(int id);
        Status AddUser(UserDTOUi userDto);
        Status UpdateUser(UserDTO userDto);
        Status DeleteUser(int id);
        bool UserExists(int id);
        Status Save();

    }
}
