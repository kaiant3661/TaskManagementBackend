using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly AuditLogService _auditLogService;

        public AuditLogController(AuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        // Get all audit logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> Get()
        {
            var logs = await _auditLogService.GetAuditLogsAsync();
            return Ok(logs); // Return the list of audit logs
        }

        // Create a new audit log
        [HttpPost]
        public async Task<ActionResult<AuditLog>> Create([FromBody] AuditLog auditLog)
        {
            var createdLog = await _auditLogService.LogActionAsync(
                auditLog.Action,
                auditLog.PerformedByUserId
            );
            return CreatedAtAction(nameof(Get), new { id = createdLog.AuditLogId }, createdLog); // Return the created log
        }

        // Get a specific audit log by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLog>> GetAuditLog(int id)
        {
            var log = await _auditLogService.GetAuditLogByIdAsync(id);
            if (log == null) return NotFound(); // Return NotFound if log doesn't exist
            return Ok(log); // Return the found audit log
        }

        // Clear all audit logs
        [HttpDelete]
        public async Task<IActionResult> ClearAllLogs()
        {
            await _auditLogService.ClearAllLogsAsync();
            return NoContent(); // No content response when all logs are deleted
        }

        // Clear audit logs of a specific user
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> ClearLogsByUser(int userId)
        {
            await _auditLogService.ClearLogsByUserAsync(userId);
            return NoContent();
        }

        // Clear audit logs based on specific data (Action)
        [HttpDelete("action/{action}")]
        public async Task<IActionResult> ClearLogsByAction(string action)
        {
            await _auditLogService.ClearLogsByActionAsync(action);
            return NoContent();
        }

        // Search logs by specific date
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogsByDate(DateTime date)
        {
            var logs = await _auditLogService.GetLogsByDateAsync(date);
            return Ok(logs);
        }

        // Search logs by specific user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogsByUser(int userId)
        {
            var logs = await _auditLogService.GetLogsByUserAsync(userId);
            return Ok(logs);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogsByAction([FromQuery] string action)
        {
            var logs = await _auditLogService.GetLogsByActionAsync(action);
            return Ok(logs);
        }

    }
}
