using ECommereceApi.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ECommereceApi.Controllers
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserManagementRepo _userManagementRepo;
        private readonly IConfiguration _configuration;
        private readonly IMailRepo _mailRepo;

        public AccountController(IUserManagementRepo userManagementRepo, IConfiguration configuration, IMailRepo mailRepo)
        {
            _userManagementRepo = userManagementRepo;
            _configuration = configuration;
            _mailRepo = mailRepo;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="dto">The user registration details.</param>
        /// <returns>Created if successful, otherwise BadRequest.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO dto)
        {
            if (dto == null)
                return BadRequest("Register Model Can't be null!");

            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage).ToList();

                return BadRequest(errors);
            }

            string code = GenerateCode();

            bool isValidEmail = _mailRepo.TrySendEmail(dto.Email, code, "Email verification");

            if (!isValidEmail)
                return BadRequest("Invalid Email");

            bool isRegistered = await _userManagementRepo.TryRegisterUser(dto, code);

            if (!isRegistered)
                return BadRequest("Invalid Registry");

            return Created();
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="dto">The user login details.</param>
        /// <returns>OK with a token if successful, otherwise BadRequest.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginUserDTO dto)
        {
            if (dto == null)
                return BadRequest("User credentials can't be null");

            User? user = await _userManagementRepo.GetUserByEmail(dto.Email);

            if (user is null || user.Password != dto.Password)
                return BadRequest("User name or password is incorrect");

            bool isVerified = user?.IsVerified ?? false;
            if (!isVerified)
                return BadRequest("Email should be verified first!");

            string token = await GenerateToken(dto.Email);

            HttpContext.Response.Headers.Add("Bearer-Token", token);

            return Ok();
        }

        /// <summary>
        /// Verifies a user's email.
        /// </summary>
        /// <param name="verifyModel">The email verification details.</param>
        /// <returns>OK if successful, otherwise BadRequest.</returns>
        [HttpPost("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Verify(VerifyEmail verifyModel)
        {
            if (await _userManagementRepo.ConfirmEmail(verifyModel))
                return Ok();
            else
                return BadRequest("Email or code is incorrect");
        }

        /// <summary>
        /// Initiates the forgot password process by sending a code to the user email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>OK if successful, otherwise BadRequest.</returns>
        [HttpGet("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            User? user = await _userManagementRepo.GetUserByEmail(email);
            if (user is null)
                return BadRequest("Email isn't correct");

            if (!(user.IsVerified ?? false))
                return BadRequest("Email not verified! You should verify it first.");

            string code = GenerateCode();

            bool codeSent = _mailRepo.TrySendEmail(email, code, "Email verification to change password");
            if (!codeSent)
                return BadRequest();

            bool verificationCodeChanged = await _userManagementRepo.TryChangeVerificationCode(email, code);
            if (!verificationCodeChanged)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Completes the forgot password process by sending code ,email and the new password .
        /// </summary>
        /// <param name="dto">The forgot password details.</param>
        /// <returns>OK if successful, otherwise BadRequest.</returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(forgotPasswordDTO dto)
        {
            if (dto is null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(error => error.ErrorMessage)
                                                .ToList();

                return BadRequest(errors);
            }

            User? user = await _userManagementRepo.GetUserByEmail(dto.Email);
            if (user is null)
                return BadRequest("Email isn't correct");

            if (!(user.IsVerified ?? false))
                return BadRequest("Email not verified! You should verify it first.");

            if (!user.VertificationCode.Equals(dto.Code))
                return BadRequest("Email or verification code is not correct!");

            bool passwordChanged = await _userManagementRepo.TryResetPassword(dto.Email, dto.NewPassword);
            if (!passwordChanged)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Resets a user's password by sending email ,password and the new passowd.
        /// </summary>
        /// <param name="dto">The reset password details.</param>
        /// <returns>OK if successful, otherwise BadRequest.</returns>
        [Authorize]
        [HttpPatch("resetpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            if (dto is null)
                return BadRequest("Model can't be null");

            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage)
                                                        .ToList();

                return BadRequest(errors);
            }

            User? user = await _userManagementRepo.GetUserByEmail(dto.Email);

            if (user is null || user.Password != dto.Password)
                return BadRequest("Invalid email or password!");

            bool isPasswordReset = await _userManagementRepo.TryResetPassword(dto.Email, dto.NewPassword);
            if (!isPasswordReset)
                return BadRequest("Invalid password reset!");

            return Ok();
        }

        /// <summary>
        /// Gets user information.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>User information if successful, otherwise BadRequest.</returns>
        [Authorize]
        [HttpGet("userInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserData(string email)
        {
            User? user = await _userManagementRepo.GetUserByEmail(email);

            if (user is null)
                return BadRequest("Invalid email");

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

        /// <summary>
        /// Edits user data.
        /// </summary>
        /// <param name="dto">The user data to edit.</param>
        /// <returns>Created if successful, otherwise BadRequest.</returns>
        [Authorize]
        [HttpPatch("edit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditUserDate(AlterUserDataDTO dto)
        {
            if (dto is null)
                return BadRequest("Model can't be null");

            if (dto.Email is null)
                return BadRequest("Email can't be null");

            User? user = await _userManagementRepo.GetUserByEmail(dto.Email);

            if (user is null)
                return BadRequest("Incorrect email!");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isUserDataSaved = await _userManagementRepo.TryAlterUserData(dto);
            if (!isUserDataSaved)
                return BadRequest("Failed to edit user data!");

            return Created();
        }

        private async Task<string> GenerateToken(string email)
        {
            User? user = await _userManagementRepo.GetUserByEmail(email);
            if (user is null)
                throw new ArgumentException("Email is incorrect");

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            DateTime expireDate = DateTime.Now.AddDays(_configuration.GetValue<int>("JWT:DurationInDays"));

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:secretkey")));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtToken = new JwtSecurityToken(claims: claims, expires: expireDate, signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }

        private string GenerateCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
