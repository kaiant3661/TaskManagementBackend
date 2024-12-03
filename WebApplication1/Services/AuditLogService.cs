using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using AuditLog = WebApplication1.Models.AuditLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class AuditLogService
    {
        private readonly AppDbContext _dbContext;

        public AuditLogService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Add an audit log entry
        public async Task<AuditLog> LogActionAsync(string action, int performedByUserId)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                PerformedByUserId = performedByUserId,
                Timestamp = DateTime.Now
            };

            _dbContext.AuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();
            return auditLog;
        }

        // Retrieve all audit logs
        public async Task<List<AuditLog>> GetAuditLogsAsync()
        {
            return await _dbContext.AuditLogs
                .Include(a => a.PerformedByUser) // Ensure the user is included
                .ToListAsync();
        }

        // Get a specific audit log by ID
        public async Task<AuditLog?> GetAuditLogByIdAsync(int id)
        {
            return await _dbContext.AuditLogs.FirstOrDefaultAsync(log => log.AuditLogId == id);
        }

        // Clear all audit logs
        public async Task ClearAllLogsAsync()
        {
            var logs = await _dbContext.AuditLogs.ToListAsync();
            _dbContext.AuditLogs.RemoveRange(logs);
            await _dbContext.SaveChangesAsync();
        }

        // Clear audit logs of a specific user
        public async Task ClearLogsByUserAsync(int userId)
        {
            var logs = await _dbContext.AuditLogs.Where(log => log.PerformedByUserId == userId).ToListAsync();
            _dbContext.AuditLogs.RemoveRange(logs);
            await _dbContext.SaveChangesAsync();
        }

        // Clear audit logs based on specific action
        public async Task ClearLogsByActionAsync(string action)
        {
            var logs = await _dbContext.AuditLogs.Where(log => log.Action == action).ToListAsync();
            _dbContext.AuditLogs.RemoveRange(logs);
            await _dbContext.SaveChangesAsync();
        }

        // Get logs by specific date
        public async Task<List<AuditLog>> GetLogsByDateAsync(DateTime date)
        {
            return await _dbContext.AuditLogs
                .Where(log => log.Timestamp.Date == date.Date)
                .ToListAsync();
        }

        // Get logs by specific user
        public async Task<List<AuditLog>> GetLogsByUserAsync(int userId)
        {
            return await _dbContext.AuditLogs
                .Where(log => log.PerformedByUserId == userId)
                .ToListAsync();
        }

        // Get logs by specific action
        public async Task<List<AuditLog>> GetLogsByActionAsync(string action)
        {
            return await _dbContext.AuditLogs
                .Where(log => log.Action.Contains(action)) // Search for logs with the given action name
                .ToListAsync();
        }
    }
}
