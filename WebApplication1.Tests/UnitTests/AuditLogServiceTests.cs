using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;
using Task = System.Threading.Tasks.Task;

public class AuditLogServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly AuditLogService _auditLogService;

    public AuditLogServiceTests()
    {
        // Create in-memory database with a unique name for each test
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _auditLogService = new AuditLogService(_dbContext);

        // Add a user to the in-memory database with a valid password hash
        var user = new Userr
        {
            UserId = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@example.com",
            PasswordHash = Convert.ToBase64String(new byte[64]) // Example password hash (dummy value)
        };
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();  // Save the user to the database

        // Add audit logs linked to the user
        _dbContext.AuditLogs.Add(new AuditLog { Action = "Test Action 1", PerformedByUserId = 1, Timestamp = DateTime.Now, PerformedByUser = user });
        _dbContext.AuditLogs.Add(new AuditLog { Action = "Test Action 2", PerformedByUserId = 1, Timestamp = DateTime.Now, PerformedByUser = user });

        _dbContext.SaveChangesAsync().Wait(); // Save the audit logs asynchronously
    }

    [Fact]
    public async Task GetAuditLogsAsync_ShouldReturnAllLogs()
    {
        // Act
        var auditLogs = await _auditLogService.GetAuditLogsAsync();

        // Assert
        Assert.Equal(2, auditLogs.Count); // Expecting 2 logs to be returned
    }
}
