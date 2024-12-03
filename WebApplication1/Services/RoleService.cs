using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class RoleService
    {
        private readonly AppDbContext _dbContext;

        public RoleService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all roles
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        // Get a role by ID
        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _dbContext.Roles.FindAsync(id);
        }

        // Create a new role
        public async Task<Role> CreateRoleAsync(Role role)
        {
            // Check if a role with the same name already exists
            var existingRole = await _dbContext.Roles
                                               .FirstOrDefaultAsync(r => r.RoleName == role.RoleName);
            if (existingRole != null)
            {
                throw new ArgumentException("A role with this name already exists.");
            }

            // Add the new role to the database
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();
            return role;
        }


        // Update an existing role
        public async Task<Role?> UpdateRoleAsync(Role role)
        {
            var existingRole = await _dbContext.Roles.FindAsync(role.RoleId);
            if (existingRole == null) return null;

            existingRole.RoleName = role.RoleName;
            await _dbContext.SaveChangesAsync();
            return existingRole;
        }

        // Delete a role
        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _dbContext.Roles.FindAsync(roleId);
            if (role == null) return false;

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<Role> GetDefaultRoleAsync()
        {
            // Fetch the default role, e.g., "User"
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "User")
                   ?? throw new Exception("Default role not found");
        }
    }
}
