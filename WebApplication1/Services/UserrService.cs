using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Services
{
    public class UserrService
    {
        private readonly AppDbContext _dbContext;

        public UserrService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all users, including role data
        public async Task<List<Userr>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .AsNoTracking()  // Prevents change tracking, which can save memory
                .Include(u => u.Role)  // Include role data
                .ToListAsync();
        }

        // Get a single user by ID, including role data
        public async Task<Userr?> GetUserrByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        // Create a new user
        public async Task<Userr> AddUser(Userr user, string password)
        {
            // Check if an email already exists in the database
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("A user with this email already exists---.");
            }

            // Ensure the role exists
            var role = await _dbContext.Roles.FindAsync(user.RoleId);
            if (role == null)
            {
                throw new ArgumentException("Role not found");
            }

            // Hash the password and salt before storing
            PasswordHelper.CreatePasswordHashAndSalt(password, out string passwordHash, out string passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = role;  // Assign the role to the user
            user.CreatedAt = DateTime.Now;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        // Update an existing user
        public async Task<Userr?> UpdateUserAsync(Userr updatedUser, string? newPassword = null)
        {
            var user = await _dbContext.Users.FindAsync(updatedUser.UserId);
            if (user == null) return null;

            var role = await _dbContext.Roles.FindAsync(updatedUser.RoleId);
            if (role == null)
            {
                throw new ArgumentException("Role not found");
            }

            // Update basic user details
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.RoleId = updatedUser.RoleId;
            user.Role = role;

            // If a new password is provided, hash it and update the password fields
            if (!string.IsNullOrEmpty(newPassword))
            {
                PasswordHelper.CreatePasswordHashAndSalt(newPassword, out string passwordHash, out string passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            user.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return user;
        }

        // Delete a user
        public async Task<string> DeleteUserAsync(int userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Fetch the user
                var user = await _dbContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return "User not found.";
                }

                // Remove related data
                _dbContext.AuditLogs.RemoveRange(_dbContext.AuditLogs.Where(al => al.PerformedByUserId == userId));
                _dbContext.Tasks.RemoveRange(_dbContext.Tasks.Where(t => t.AssignedToUserId == userId || t.CreatedByUserId == userId));
                _dbContext.Users.Remove(user);

                // Save changes and commit transaction
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return "User and all related data have been successfully deleted.";
            }
            catch (Exception ex)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                return $"Error deleting user: {ex.Message}";
            }
        }

        // Authenticate a user (login check)
        public async Task<Userr?> AuthenticateUser(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            // Verify password hash and salt
            bool isPasswordValid = PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid) return null;

            return user;
        }


        public async Task<ServiceResponse<string>> SoftDeleteUserAsync(int userId)
        {
            var user = await _dbContext.Users
                                       .Where(u => u.UserId == userId && u.IsDeleted == false)
                                       .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User not found or already deleted.",
                    StatusCode = 404
                };
            }

            // Mark the user as deleted
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User successfully soft-deleted.",
                StatusCode = 200
            };
        }


        public async Task<ServiceResponse<string>> RestoreUserAsync(int userId)
        {
            var user = await _dbContext.Users
                                       .Where(u => u.UserId == userId && u.IsDeleted == true)
                                       .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User not found or not deleted.",
                    StatusCode = 404
                };
            }

            // Restore the user
            user.IsDeleted = false;
            user.DeletedAt = null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User successfully restored.",
                StatusCode = 200
            };
        }


        public async Task<Userr> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
