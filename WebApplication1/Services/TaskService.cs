using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class TaskService
    {
        private readonly AppDbContext _dbContext;

        public TaskService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Models.Task>> GetTasksByUserIdAsync(int userId)
        {
            // Fetch tasks where the user is either assigned or created the task
            var tasks = await _dbContext.Tasks
                .Where(t => t.AssignedToUserId == userId )
                .ToListAsync();

            return tasks;
        }
        public async Task<List<Models.Task>> GetAllTasksAsync()
        {
            return await _dbContext.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .ToListAsync();
        }

        public async Task<Models.Task?> GetTaskByIdAsync(int id)
        {
            return await _dbContext.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.TaskId == id);
        }

        public async Task<Models.Task> CreateTaskAsync(TaskDTO taskDto)
        {
            // Validate if AssignedToUserId exists
            var assignedUser = await _dbContext.Users.FindAsync(taskDto.AssignedToUserId);
            if (assignedUser == null)
            {
                throw new ArgumentException("AssignedToUserId does not exist.");
            }

            // Validate if CreatedByUserId exists
            var createdByUser = await _dbContext.Users.FindAsync(taskDto.CreatedByUserId);
            if (createdByUser == null)
            {
                throw new ArgumentException("CreatedByUserId does not exist.");
            }

            // Map DTO to Task entity
            var task = new Models.Task
            {
                TaskName = taskDto.TaskName,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                AssignedToUserId = taskDto.AssignedToUserId,
                CreatedByUserId = taskDto.CreatedByUserId,
                DueDate = taskDto.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AssignedToUser = assignedUser,
                CreatedByUser = createdByUser
            };

            // Add the task to the database
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return task;
        }

        public async Task<Models.Task?> UpdateTaskAsync(int id, TaskDTO taskDto)
        {
            // Fetch the task from the database
            var existingTask = await _dbContext.Tasks.FindAsync(id);

            if (existingTask == null)
                return null;

            // Map DTO fields to the existing task
            existingTask.TaskName = taskDto.TaskName;
            existingTask.Description = taskDto.Description;
            existingTask.Status = taskDto.Status;
            existingTask.Priority = taskDto.Priority;
            existingTask.AssignedToUserId = taskDto.AssignedToUserId;
            existingTask.CreatedByUserId = taskDto.CreatedByUserId;
            existingTask.DueDate = taskDto.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return existingTask;
        }



        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);
            if (task == null) return false;

            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();
            return true;
        }




        public async Task<(List<Models.Task>, int)> GetPaginatedTasksAsync(int page = 1, int pageSize = 10)
        {
            // Fetch the total number of tasks for pagination purposes
            var totalTasks = await _dbContext.Tasks.CountAsync();

            // Fetch the tasks for the current page with pagination
            var tasks = await _dbContext.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Skip((page - 1) * pageSize)  // Skip tasks based on the page and page size
                .Take(pageSize)  // Take the number of tasks for the current page
                .ToListAsync();

            // Return both the tasks and the total count
            return (tasks, totalTasks);
        }

    }
}
