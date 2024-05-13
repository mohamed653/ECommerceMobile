using ECommereceApi.DTOs;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // Some Considerations:
        // Server-side validation 
        // Localization through routing 
        // Customizing error messages (Localized)
 


        private readonly IUserRepo _userRepo;
        public AdminController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Get all Users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_userRepo.GetUsers());
        }



        /// <summary>
        /// Get User by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public IActionResult GetUser(int id)
        {
            var user = _userRepo.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        /// <summary>
        /// AddUser
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddUser([FromBody] UserDTOUi userDto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var status = _userRepo.AddUser(userDto);

            if (status == Status.Success)
            {
                //return CreatedAtAction("GetUser", new { id = userDto.UserId }, userDto);
                return Ok(userDto);
            }
            else if (status == Status.EmailExistsBefore)
            {
                return BadRequest("Email Exists Before");
            }
            return BadRequest("An Error Has Occured");
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
        public IActionResult SortUsers(UserOrderBy userOrderBy, SortType sortType = SortType.ASC)
        {
            return Ok(_userRepo.SortUsers(userOrderBy, sortType));
        }

        /// <summary>
        /// SearchUserByName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name:alpha}")]
        public IActionResult SearchUserByName(string name)
        {
            var user = _userRepo.SearchUserByName(name);
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
        public IActionResult SearchUserByEmail(string email)
        {
            var user = _userRepo.SearchUserByEmail(email);
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
        public IActionResult GetUserPagination(int pageNumber, int pageSize,string? email)
        {
            IEnumerable<UserDTO> users;
            if (email == null)
            {
                users = _userRepo.GetUserPagination(pageNumber, pageSize);
            }
            else
            {
                users = _userRepo.GetUserPagination(pageNumber, pageSize,email);
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
        public IActionResult UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (id != userDto.UserId)
            {
                return BadRequest();
            }
            var status = _userRepo.UpdateUser(userDto);
            if (status == Status.Success)
            {
                return Ok("Updated Successfuly");
            }
            return BadRequest();
        }

        /// <summary>
        /// DeleteUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var status = _userRepo.DeleteUser(id);
            if (status == Status.Success)
            {
                return Ok("Deleted Successfuly");
            }
            return NotFound();
        }



    }
}
