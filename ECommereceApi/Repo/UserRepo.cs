using AutoMapper;
using ECommereceApi.Data;
using ECommereceApi.DTOs;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class UserRepo:IUserRepo
    {
        private readonly ECommerceContext _context;
        private readonly IMapper _mapper;
        public UserRepo(ECommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IEnumerable<UserDTO> GetUsers()
        {
            return _mapper.Map<List<UserDTO>>(_context.Users.ToList());
        }

        public UserDTO GetUser(int id)
        {
            return _mapper.Map<UserDTO>( _context.Users.Find(id));
        }
        public Status AddUser(UserDTOUi userDto)
        {
            if (_context.Users.Any(u => u.Email == userDto.Email))
            {
                return Status.EmailExistsBefore;
            }
            userDto.Email = userDto.Email.ToLower().Trim();
            var user = _mapper.Map<User>(userDto);
            _context.Users.Add(user);
            return Save();
        }
        public Status UpdateUser(UserDTO userDto)
        {
            userDto.Email = userDto.Email.ToLower().Trim();
            var user = _mapper.Map<User>(userDto);
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

        public IEnumerable<UserDTO> SearchUserByEmail(string email)
        {
            return _mapper.Map<List<UserDTO>>(_context.Users.Where(u => u.Email.Contains(email.ToLower())).ToList());
        }
        public IEnumerable<UserDTO> SearchUserByName(string name)
        {
            return _mapper.Map<List<UserDTO>>(_context.Users.Where(u => u.FName.Contains(name)||u.LName.Contains(name)).ToList());
        }

        public IEnumerable<UserDTO> GetUserPagination(int pageNumber, int pageSize)
        {
            return _mapper.Map<List<UserDTO>>(_context.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
        }

        public IEnumerable<UserDTO> GetUserPagination(int pageNumber, int pageSize, string email)
        {
            return _mapper.Map<List<UserDTO>>(_context.Users.Where(u => u.Email.Contains(email)).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
        }

        public IEnumerable<UserDTO> SortUsers(UserOrderBy userOrderBy, SortType sortType = SortType.ASC)
        {

            var users = _context.Users.AsQueryable();
            switch (userOrderBy)
            {
                case UserOrderBy.Name:
                    users = sortType == SortType.ASC ? users.OrderBy(u => u.FName) : users.OrderByDescending(u => u.FName);
                    break;
                case UserOrderBy.Email:
                    users = sortType == SortType.ASC ? users.OrderBy(u => u.Email) : users.OrderByDescending(u => u.Email);
                    break;
            }
            return _mapper.Map<List<UserDTO>>(users.ToList());
        }

        public Status SoftDelete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return Status.NotFound;
            }
            user.IsDeleted = true;
            return Save();

        }
    }
}
