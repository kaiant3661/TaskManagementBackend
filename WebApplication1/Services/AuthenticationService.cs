using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;
using System.Security.Cryptography;

namespace WebApplication1.Services
{
    public class AuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Authenticate a user with email and password
        public async Task<string?> AuthenticateUserAsync(string email, string password)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role) // Include role for role-based claims
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return null; // User does not exist

            // Decode the password hash and salt stored in Base64, then verify the password
            var storedPasswordHash = Convert.FromBase64String(user.PasswordHash);
            var storedPasswordSalt = user.PasswordSalt;
            var isPasswordValid = VerifyPasswordHash(password, storedPasswordHash, storedPasswordSalt);
            if (!isPasswordValid)
            {
                return null; // Invalid credentials
            }

            return GenerateJwtToken(user); // Return JWT if authentication is successful
        }
        // In your AuthService or UserService

        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user?.UserId;  // Return the user ID if found, otherwise null
        }

        // Register a new user with email, password, first name, and last name
        public async Task<ServiceResponse<Userr>> RegisterUserAsync(string email, string password, string firstName, string lastName)
        {
            var response = new ServiceResponse<Userr>();

            // Check if the email already exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "A user with this email already exists.";
                response.StatusCode = 400; // Bad Request
                return response;
            }

            // Create password hash and salt before storing them
            CreatePasswordHash(password, out byte[] passwordHash, out string passwordSalt);

            // Create a new user
            var user = new Userr
            {
                Email = email,
                PasswordHash = Convert.ToBase64String(passwordHash), // Store the hashed password as a string
                PasswordSalt = passwordSalt, // Store the salt
                FirstName = firstName,       // Set the FirstName
                LastName = lastName,         // Set the LastName
                Role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "New") // Assign a default role
            };

            // Save the user to the database
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            response.Data = user;
            response.Success = true;
            response.Message = "User registered successfully.";
            response.StatusCode = 201; // Created
            return response;
        }


        // Generate JWT token with claims
        private string GenerateJwtToken(Userr user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            // Add role claim if the user has a role
            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleName)); // Use RoleName instead of Name
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Create a password hash and salt
        private void CreatePasswordHash(string password, out byte[] passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                // Generate a new salt for each password
                passwordSalt = Convert.ToBase64String(hmac.Key); // Key is used as the salt
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Compute the hash
            }
        }

        // Verify the password hash using stored hash and salt
        private bool VerifyPasswordHash(string password, byte[] storedHash, string storedSalt)
        {
            using (var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt)))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash); // Compare the hashes
            }
        }
    }
}
