using ECommereceApi.DTOs;
using ECommereceApi.Models;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IUserRepo
    {
        IEnumerable<UserDTO> GetUsers();

        IEnumerable<UserDTO> SearchUserByEmail(string email);
        IEnumerable<UserDTO> SearchUserByName(string name);
        IEnumerable<UserDTO> GetUserPagination(int pageNumber, int pageSize);
        IEnumerable<UserDTO> GetUserPagination(int pageNumber, int pageSize,string email);

        IEnumerable<UserDTO> SortUsers(UserOrderBy userOrderBy, SortType sortType = SortType.ASC);
        UserDTO GetUser(int id);
        Status AddUser(UserDTOUi userDto);
        Status UpdateUser(UserDTO userDto);
        Status DeleteUser(int id);
        bool UserExists(int id);
        Status Save();
        Status SoftDelete(int id);
    }
}
