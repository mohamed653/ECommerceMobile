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
        // API Documentation
        // Search User by Name
        // Pagination in GetUsers
        // Sorting in GetUsers


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
        [HttpGet("{id}")]
        /// <summary>
        /// Get all User
        /// </summary>
        /// <returns></returns>
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
            return BadRequest();
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
