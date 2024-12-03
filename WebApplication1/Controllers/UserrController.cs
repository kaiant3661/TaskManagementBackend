using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserrController : ControllerBase
    {
        private readonly UserrService _service;
        private readonly AuditLogService _auditLogService;
        private readonly RoleService _roleService;

        public UserrController(UserrService userService, AuditLogService auditLogService, RoleService roleService)
        {
            _service = userService;
            _auditLogService = auditLogService;
            _roleService = roleService;
        }

        // GET: api/Userr
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Userr>>> GetUsers()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/Userr/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Userr>> GetUser(int id)
        {
            var user = await _service.GetUserrByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/Userr
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] Userr user)
        {
            // Validate if password is provided
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("Password is required.-----");
            }

            // Set the role if not provided (default to a valid role)
            if (user.RoleId == 0 && user.Role != null)
            {
                // Validate Role and set RoleId
                var validRole = await _roleService.GetRoleByIdAsync(user.Role.RoleId);
                if (validRole == null) return BadRequest("The provided Role is invalid.");
                user.RoleId = validRole.RoleId;
            }
            else if (user.RoleId != 0)
            {
                // Fetch Role by RoleId if Role is null
                user.Role ??= await _roleService.GetRoleByIdAsync(user.RoleId);
                if (user.Role == null) return BadRequest($"Role with ID {user.RoleId} does not exist.");
            }
            else
            {
                // Assign default Role if both Role and RoleId are missing
                user.Role ??= await _roleService.GetDefaultRoleAsync();
                user.RoleId = user.Role.RoleId;
            }

            // Validate mismatch between Role and RoleId
            if (user.Role != null && user.RoleId != user.Role.RoleId)
                return BadRequest("RoleId and Role do not match.");



            // Add user to the system
            var createdUser = await _service.AddUser(user, user.PasswordHash);
            await _auditLogService.LogActionAsync("User Created", createdUser.UserId);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        // PUT: api/Userr/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Userr updatedUser)
        {
            if (id != updatedUser.UserId)
                return BadRequest("User ID mismatch.");

            var user = await _service.GetUserrByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            // Check for unique email
            var existingUserWithEmail = await _service.GetUserByEmailAsync(updatedUser.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.UserId != id)
            {
                return Conflict("A user with this email already exists.");
            }
            if (user.PasswordHash==updatedUser.PasswordHash) { updatedUser.PasswordHash = null; }
            // Update basic information (name, email, role, etc.)
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.RoleId = updatedUser.RoleId;

            // If the user is updating the password, use the new password field (plaintext)
            var updated = await _service.UpdateUserAsync(user, updatedUser.PasswordHash); // Send the new password (if any)
            if (updated == null)
                return NotFound("User not found.");

            // Log the action
            await _auditLogService.LogActionAsync("User Updated", updated.UserId);

            return CreatedAtAction(nameof(GetUser), new { id = updated.UserId }, updated);
        }



        // DELETE: api/Userr/{id}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId,int performedBy)
        {
            var result = await _service.DeleteUserAsync(userId);

            if (result == "User not found.")
            {
                return NotFound(result);
            }
            if (result.StartsWith("Error"))
            {
                return StatusCode(500, result);
            }

            await _auditLogService.LogActionAsync("User Deleted", performedBy);
            return Ok(result);
        }



        [HttpPost("soft-delete/{userId}")]
        public async Task<IActionResult> SoftDeleteUser(int userId)
        {
            var response = await _service.SoftDeleteUserAsync(userId);

            if (response.Success) {await _auditLogService.LogActionAsync("User Disabled", userId); }
            // Use the status code and message from the service response
            
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPost("restore/{userId}")]
        public async Task<IActionResult> RestoreUser(int userId)
        {
            var response = await _service.RestoreUserAsync(userId);

            if (response.Success) { await _auditLogService.LogActionAsync("User Enabled", userId); }
            // Use the status code and message from the service response
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}
