using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.Data;
using WebApplication1.Models;

public class DatabaseFixture : IDisposable
{
    public AppDbContext DbContext { get; private set; }

    public DatabaseFixture()
    {
        // Configure in-memory database with a unique name for isolation
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique name for each test run
            .EnableSensitiveDataLogging() // Enable detailed error messages
            .Options;

        // Initialize DbContext
        DbContext = new AppDbContext(options);

        // Optionally seed data for tests
        SeedData();
    }

    private void SeedData()
    {
        // Add users and audit logs for tests
        DbContext.Users.Add(new Userr { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" });
        DbContext.AuditLogs.Add(new AuditLog { Action = "Initial Log", PerformedByUserId = 1, Timestamp = DateTime.UtcNow });
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        // Clean up resources after tests
        DbContext.Dispose();
    }
}
