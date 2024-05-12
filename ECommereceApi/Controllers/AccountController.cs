using ECommereceApi.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace ECommereceApi.Controllers
{

    [ApiController]
    [Route("/api/[Controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserManagementRepo _userManagementRepo;
        private readonly IConfiguration _configuration;
        public AccountController(IUserManagementRepo userManagementRepo ,IConfiguration configuration)
        {
            _userManagementRepo = userManagementRepo;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO dto)
        {
            if (dto == null)
                return BadRequest("Register Model Can't be null!");

            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage).ToList();  
                
                return BadRequest("No");
            }

            bool isRegistered = await _userManagementRepo.TryRegisterUser(dto);



            if (! isRegistered)
                return BadRequest("Invalid Registry");

            // generate jwt token
            string token = await GenerateToken(dto.Email);
            return Created("",token);

        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(LoginUserDTO dto)
        {
            if (dto == null)
                return BadRequest("User credentials can't be null");

            User? user =await _userManagementRepo.GetUserByEmail(dto.Email);

            if (user is null || user.Password != dto.Password)
                return BadRequest("User name or password is incorrect");

            string token =await GenerateToken(dto.Email);

            HttpContext.Response.Headers.Add("Bearer-Token", token);
            
            return Ok();
        }

        [Authorize]
        [HttpPatch("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            if (dto is null)
                return BadRequest("Model Can't be null");

            if(! ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage)
                                                        .ToList();

                return BadRequest(errors);
            }

            User? user =await _userManagementRepo.GetUserByEmail(dto.Email);
            
            if (user is null || user.Password != dto.Password)
                return BadRequest("Invalid email or password!");

            bool isPasswordReset =await _userManagementRepo.TryResetPassword(dto.Email, dto.NewPassword);
            if (!isPasswordReset)
                return BadRequest("invalid Password Reset!");

            return Ok();
        }


        [Authorize]
        [HttpGet("userInfo")]
        public async Task<IActionResult> GetUserData(string email)
        {
            User? user =await _userManagementRepo.GetUserByEmail(email);

            if (user is null)
                return BadRequest("Invalid Email");


            AlterUserDataDTO data = new AlterUserDataDTO()
            {
                Email = email,
                FName = user.FName,
                LName = user.LName,
                City = user.City,
                Governorate = user.Governorate,
                Phone = user.Phone,
                PostalCode = user.PostalCode,
                Street = user.PostalCode
            };

            return Ok(data); 
        }

        [Authorize]
        [HttpPatch("edit")]
        public async Task<IActionResult> EditUserDate(AlterUserDataDTO dto)
        {
            if (dto is null)
                return BadRequest("Model Can't be null");

            if (dto.Email is null)
                return BadRequest("Email can't be null");


            User? user = await _userManagementRepo.GetUserByEmail(dto.Email);

            if (user is null)
                return BadRequest("Incorrect Email!");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isUserDataSaved =await _userManagementRepo.TryAlterUserData(dto);
            if (!isUserDataSaved)
                return BadRequest("Failed to edit user data!!");

            return Created();
        }

        private async Task<string> GenerateToken(string email)
        {
            User? user =await _userManagementRepo.GetUserByEmail(email);
            if (user is null)
               throw new ArgumentException("email is incorrect");

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role.ToString(), user.Role.ToString())
            };

            DateTime expireDate = DateTime.Now.AddDays(_configuration.GetValue<int>("JWT:DurationInDays"));

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:secretkey")));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtToken = new JwtSecurityToken(claims: claims, expires: expireDate,signingCredentials: credentials);
        
            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            
            return token;
        }

    }
}
