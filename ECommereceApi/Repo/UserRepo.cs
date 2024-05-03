using ECommereceApi.Data;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class UserRepo:IUserRepo
    {
        private readonly ECommerceContext _context;
        public UserRepo(ECommerceContext context)
        {
            _context = context;
        }
        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }
        public User GetUser(int id)
        {
            return _context.Users.Find(id);
        }
        public Status AddUser(User user)
        {
            _context.Users.Add(user);
            return Save();
        }
        public Status UpdateUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            return Save();
        }
        public Status DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return Status.NotFound;
            }
            _context.Users.Remove(user);
            return Save();
        }
        public bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
        public Status Save()
        {
            try
            {
                _context.SaveChanges();
                return Status.Success;
            }
            catch (Exception ex)
            {
                return Status.Failed;
            }
        }
    }
}
