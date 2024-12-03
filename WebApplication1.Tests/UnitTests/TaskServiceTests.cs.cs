using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Services;  // Make sure this is here
using Xunit;
using TaskModel = WebApplication1.Models.Task;

namespace UnitTests
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task CreateTaskAsync_ShouldAddTaskToDb()
        {
            // Arrange: Setup InMemory DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // In-memory database for testing
                .Options;

            using (var context = new AppDbContext(options))
            {
                var taskService = new TaskService(context);  // Now TaskService will be recognized

                // Create a new task with all required properties filled
                var newTask = new TaskDTO
                {
                    TaskName = "Test Task",// Fill Description
                    Status = "Pending",  // Fill Status (choose one: Pending, In Progress, or Completed)
                    Priority = "Medium", // Fill Priority (choose one: Low, Medium, or High)
                    CreatedByUserId = 1
                };

                // Act: Call the method that adds the task
                TaskModel createdTask = await taskService.CreateTaskAsync(newTask);

                // Assert: Verify that the task was added
                var savedTask = await context.Tasks.FindAsync(createdTask.TaskId);
                Assert.NotNull(savedTask);
                Assert.Equal("Test Task", savedTask.TaskName); // Verify the task's name
                Assert.Equal("Test Task Description", savedTask.Description); // Verify the task's description
                Assert.Equal("Pending", savedTask.Status); // Verify the task's status
                Assert.Equal("Medium", savedTask.Priority); // Verify the task's priority
            }
        }
    }
}
