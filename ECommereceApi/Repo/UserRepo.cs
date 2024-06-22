using AutoMapper;
using ECommereceApi.Data;
using ECommereceApi.DTOs.Account;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class UserRepo : IUserRepo
    {
        private readonly ECommerceContext _context;
        private readonly IMapper _mapper;
        public UserRepo(ECommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> GetAdminsAsync()
        {
            var users = await _context.Users.Where(x=>x.Role==RoleType.Admin).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }
        public async Task<IEnumerable<UserDTO>> GetCustomersAsync()
        {
            var users = await _context.Users.Where(x => x.Role == RoleType.Customer).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<Status> AddUserAsync(UserDTOUi userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                return Status.EmailExistsBefore;
            }
            userDto.Email = userDto.Email.ToLower().Trim();
            var user = _mapper.Map<User>(userDto);
            user.VertificationCode = "adminver"; //  Generate Random Code
          
            if (userDto.Role == RoleType.Customer)
            {
                await _context.Users.AddAsync(user);
                var status = await SaveAsync();
                if (status != Status.Success)
                {
                    return status;
                }
                await _context.Customers.AddAsync(new Customer { UserId = user.UserId});
                return await SaveAsync();
            }
            else
            {
                await _context.Users.AddAsync(user);
                var status = await SaveAsync();
                if (status != Status.Success)
                {
                    return status;
                }
                await _context.Admins.AddAsync(new Admin { UserId = user.UserId });
                return await SaveAsync();
            }
        }

        public async Task<Status> UpdateUserAsync(UserDTO userDto)
        {
            userDto.Email = userDto.Email.ToLower().Trim();
            var user = _mapper.Map<User>(userDto);
            _context.Entry(user).State = EntityState.Modified;
            return await SaveAsync();
        }

        public async Task<Status> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Status.NotFound;
            }
            _context.Users.Remove(user);
            return await SaveAsync();
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(e => e.UserId == id);
        }

        public async Task<Status> SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return Status.Success;
            }
            catch (Exception ex)
            {
                return Status.Failed;
            }
        }

        public async Task<IEnumerable<UserDTO>> SearchUserByEmailAsync(string email)
        {
            var users = await _context.Users.Where(u => u.Email.Contains(email.ToLower())).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> SearchUserByNameAsync(string name)
        {
            var users = await _context.Users.Where(u => u.FName.Contains(name) || u.LName.Contains(name)).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> GetUserPaginationAsync(int pageNumber, int pageSize)
        {
            var users = await _context.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> GetUserPaginationAsync(int pageNumber, int pageSize, string email)
        {
            var users = await _context.Users.Where(u => u.Email.Contains(email)).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> SortUsersAsync(UserOrderBy userOrderBy, SortType sortType = SortType.ASC)
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
            var sortedUsers = await users.ToListAsync();
            return _mapper.Map<List<UserDTO>>(sortedUsers);
        }

        public async Task<Status> SoftDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Status.NotFound;
            }
            user.IsDeleted = true;
            return await SaveAsync();
        }

    }
}
