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
        private readonly IUserRepo _userRepo;
        public AdminController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_userRepo.GetUsers());
        }
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _userRepo.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            var status = _userRepo.AddUser(user);
            if (status == Status.Success)
            {
                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            return BadRequest();
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }
            var status = _userRepo.UpdateUser(user);
            if (status == Status.Success)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var status = _userRepo.DeleteUser(id);
            if (status == Status.Success)
            {
                return NoContent();
            }
            return NotFound();
        }


    }
}
