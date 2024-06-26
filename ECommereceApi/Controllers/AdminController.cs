using ECommereceApi.DTOs.Account;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    // [Authorize(Roles="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        public AdminController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Get all Users
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepo.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("GetAdmins")]
        public async Task<IActionResult> GetAdmins()
        {
            var users = await _userRepo.GetAdminsAsync();
            return Ok(users);
        }
        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            var users = await _userRepo.GetCustomersAsync();
            return Ok(users);
        }
        /// <summary>
        /// Get User by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepo.GetUserAsync(id);
            if (user == null)
            {
                return NotFound("User Doesn't Exist or Deleted ");
            }
         
            return Ok(user);
        }

        /// <summary>
        /// AddUser
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserDTOUi userDto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var status = await _userRepo.AddUserAsync(userDto);

            if (status == Status.Success)
            {
                return Ok(userDto);
            }
            else if (status == Status.EmailExistsBefore)
            {
                return BadRequest("Email Exists Before");
            }
            return BadRequest("An Error Has Occurred");
        }

        /// <summary>
        /// SortUsers by userOrderBy and sortType  
        /// UserOrderBy: Name, Email, Date
        /// SortType: ASC, DESC
        /// </summary>
        /// <param name="userOrderBy"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        [HttpGet("{userOrderBy:int}/{sortType:int}")]
        public async Task<IActionResult> SortUsers(UserOrderBy userOrderBy, SortType sortType = SortType.ASC)
        {
            var users = await _userRepo.SortUsersAsync(userOrderBy, sortType);
            return Ok(users);
        }

        /// <summary>
        /// SearchUserByName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name:alpha}")]
        public async Task<IActionResult> SearchUserByName(string name)
        {
            var user = await _userRepo.SearchUserByNameAsync(name);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// SearchUserByEmail
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("{email}")]
        public async Task<IActionResult> SearchUserByEmail(string email)
        {
            var user = await _userRepo.SearchUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// GetUserPagination
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetUserPagination(int pageNumber, int pageSize, string? email)
        {
            IEnumerable<UserDTO> users;
            if (email == null)
            {
                users = await _userRepo.GetUserPaginationAsync(pageNumber, pageSize);
            }
            else
            {
                users = await _userRepo.GetUserPaginationAsync(pageNumber, pageSize, email);
            }
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        /// <summary>
        /// UpdateUser
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            var _user = await _userRepo.GetUserAsync(id);
            if (_user is null || id != _user.UserId)
            {
                return BadRequest();
            }
            var status = await _userRepo.UpdateUserAsync(id, userUpdateDTO);
            if (status == Status.Success)
            {
                return Ok("Updated Successfully");
            }
            else if (status == Status.EmailExistsBefore)
            {
                return BadRequest("Email Exists Before");
            }
            else if (status == Status.NotFound)
            {
                return NotFound();
            }
            return BadRequest();
        }

        /// <summary>
        /// DeleteUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetUserAsync(id);
            if(user==null)
            {
                return BadRequest("User Doesn't Exist or Already Deleted");
            }
            var status = await _userRepo.SoftDeleteAsync(id);
            if (status == Status.Success)
            {
                return Ok("Deleted Successfully");
            }
            else if(status == Status.NotFound)
            {
                return NotFound();
            }
            else if(status == Status.SuperAdminConstraint)
            {
                return BadRequest("Super Admin Can't Be Deleted");
            }
            return NotFound();
        }


    }

}
