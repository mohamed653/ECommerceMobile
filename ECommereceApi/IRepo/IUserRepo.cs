
using ECommereceApi.Models;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IUserRepo
    {
        IEnumerable<User> GetUsers();
        User GetUser(int id);
        Status AddUser(User user);
        Status UpdateUser(User user);
        Status DeleteUser(int id);
        bool UserExists(int id);
        Status Save();

    }
}
