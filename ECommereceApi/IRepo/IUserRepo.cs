using ECommereceApi.DTOs.Account;
using ECommereceApi.Models;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IUserRepo
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();

        Task<IEnumerable<UserDTO>> GetCustomersAsync();
        Task<IEnumerable<UserDTO>> GetAdminsAsync();

        Task<IEnumerable<UserDTO>> SearchUserByEmailAsync(string email);
        Task<IEnumerable<UserDTO>> SearchUserByNameAsync(string name);
        Task<IEnumerable<UserDTO>> GetUserPaginationAsync(int pageNumber, int pageSize);
        Task<IEnumerable<UserDTO>> GetUserPaginationAsync(int pageNumber, int pageSize, string email);

        Task<IEnumerable<UserDTO>> SortUsersAsync(UserOrderBy userOrderBy, SortType sortType = SortType.ASC);
        Task<UserDTO> GetUserAsync(int id);
        Task<Status> AddUserAsync(UserDTOUi userDto);
        Task<Status> UpdateUserAsync(int userId, UserDTOUi userDtoUi);
        Task<Status> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<Status> SaveAsync();
        Task<Status> SoftDeleteAsync(int id);
    }

}
