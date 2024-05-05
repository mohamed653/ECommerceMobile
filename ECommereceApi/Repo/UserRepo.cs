using AutoMapper;
using ECommereceApi.Data;
using ECommereceApi.DTOs;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
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
            var user = _mapper.Map<User>(userDto);
            _context.Users.Add(user);
            return Save();
        }
        public Status UpdateUser(UserDTO userDto)
        {
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
    }
}
