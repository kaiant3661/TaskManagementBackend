using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace UnitTests
{
    public class UserrServiceTests
    {
        [Fact]
        public async Task AddUser_ShouldAddUserToDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("UserrTestDb")
                .Options;

            using (var context = new AppDbContext(options))
            {
                var service = new UserrService(context);
                var role = new Role { RoleName = "Admin" };
                context.Roles.Add(role);
                await context.SaveChangesAsync();

                var user = new Userr
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    RoleId = role.RoleId
                };

                // Corrected: passing the plain password string here
                var createdUser = await service.AddUser(user, "password");

                var savedUser = await context.Users.FindAsync(createdUser.UserId);
                Assert.NotNull(savedUser);
                Assert.Equal("John", savedUser.FirstName);
            }
        }


        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserFromDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("UserrTestDb")
                .Options;

            using (var context = new AppDbContext(options))
            {
                var service = new UserrService(context);
                var role = new Role { RoleName = "Admin" };
                context.Roles.Add(role);

                var user = new Userr
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@example.com",
                    PasswordHash = "password",
                    RoleId = role.RoleId
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                // Act
                var result = await service.DeleteUserAsync(user.UserId);
                var deletedUser = await context.Users.FindAsync(user.UserId);

                // Assert
                Assert.Equal("User and all related data have been successfully deleted.", result); // Check the response string
                Assert.Null(deletedUser); // Ensure the user is removed from the database
            }
        }
    }
}
