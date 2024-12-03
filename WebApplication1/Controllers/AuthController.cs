using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AuditLogService _logService;
        public AuthController(AuthService authService, AuditLogService logService)
        {
            _authService = authService;
            _logService = logService;
        }

        // Login action
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Authenticate user and get the token
            var token = await _authService.AuthenticateUserAsync(request.Email, request.Password);

            if (token == null)
                return Unauthorized("Invalid email or password.");

            // Fetch the user ID by email
            var userId = await _authService.GetUserIdByEmailAsync(request.Email);

            if (userId == null)
                return Unauthorized("Invalid email or password.");

            // Return the token and user ID
            return Ok(new { Token = token, UserId = userId });
        }


        // Register action
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest model)
        {
            var result = await _authService.RegisterUserAsync(model.Email, model.Password, model.FirstName, model.LastName);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return StatusCode(result.StatusCode, new { Message = result.Message, Data = result.Data });
        }


    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
    }
}
