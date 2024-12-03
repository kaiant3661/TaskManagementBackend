using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services;
using TaskModel = WebApplication1.Models.Task;

namespace WebApplication1.Controllers
{//[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly AuditLogService _auditLogService;

        public TaskController(TaskService taskService, AuditLogService auditLogService)
        {
            _taskService = taskService;
            _auditLogService = auditLogService;
        }
        [HttpGet("getPaginatedtask")]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetPaginatedTasks(int page = 1, int pageSize = 10)
        {
            var (tasks, totalTasks) = await _taskService.GetPaginatedTasksAsync(page, pageSize);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalTasks / (double)pageSize);

            // Prepare response with pagination info
            var response = new
            {
                TotalTasks = totalTasks,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Tasks = tasks
            };

            return Ok(response);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<IEnumerable<TaskModel>>> Get()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<TaskModel>> Get(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDTO taskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var task = await _taskService.CreateTaskAsync(taskDto);
                await _auditLogService.LogActionAsync($"Task created: {taskDto.TaskName}", taskDto.CreatedByUserId);
                return CreatedAtAction(nameof(Get), new { id = task.TaskId }, task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskDTO taskDto)
        {
            if (taskDto == null)
                return BadRequest("Invalid task data.");

            // Call the service method to update the task
            var updatedTask = await _taskService.UpdateTaskAsync(id, taskDto);

            if (updatedTask == null)
                return NotFound();

            // Log the action
            await _auditLogService.LogActionAsync($"Task Updated: {taskDto.TaskName}", taskDto.CreatedByUserId);

            return Ok(updatedTask); // Return updated task
        }




        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            var success = await _taskService.DeleteTaskAsync(id);
            if (!success) return NotFound();
            await _auditLogService.LogActionAsync($"Task Deleted: Task ID {id}", task.CreatedByUserId);
            return NoContent();
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            if (tasks == null || tasks.Count == 0)
            {
                return NotFound("No tasks found for this user.");
            }

            return Ok(tasks);
        }
    }
}
