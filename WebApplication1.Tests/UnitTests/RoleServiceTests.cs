using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
    public class RoleServiceTests
    {
        [Fact]
        public async Task CreateRoleAsync_ShouldAddRoleToDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("RoleTestDb")
                .Options;

            using (var context = new AppDbContext(options))
            {
                var service = new RoleService(context);
                var role = await service.CreateRoleAsync(new Role { RoleName = "Admin" });

                var savedRole = await context.Roles.FindAsync(role.RoleId);
                Assert.NotNull(savedRole);
                Assert.Equal("Admin", savedRole.RoleName);
            }
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldRemoveRoleFromDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("RoleTestDb")
                .Options;

            using (var context = new AppDbContext(options))
            {
                var service = new RoleService(context);
                var role = await service.CreateRoleAsync(new Role { RoleName = "Admin" });
                var result = await service.DeleteRoleAsync(role.RoleId);

                var deletedRole = await context.Roles.FindAsync(role.RoleId);
                Assert.True(result);
                Assert.Null(deletedRole);
            }
        }
    }
}
