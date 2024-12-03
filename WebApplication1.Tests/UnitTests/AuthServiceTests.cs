using Microsoft.Extensions.Configuration;
using Xunit;
using WebApplication1.Services;

public class JwtSettingsTests
{
    private readonly IConfiguration _configuration;

    public JwtSettingsTests()
    {
        // Load configuration from appsettings.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Set base path for the configuration
            .AddJsonFile("appsettings.json"); // Load the appsettings.json file

        _configuration = builder.Build(); // Build configuration
    }

    [Fact]
    public void JwtSettings_ShouldBeValid()
    {
        // Retrieve the JwtSettings from the configuration
        var jwtSettings = _configuration.GetSection("Jwt");

        // Assert that the Jwt settings are properly loaded
        Assert.NotNull(jwtSettings["Issuer"]);
        Assert.NotNull(jwtSettings["Audience"]);
        Assert.NotNull(jwtSettings["SecretKey"]);

        // Validate that they match expected values
        Assert.Equal("https://dev-ee8n18ff4hpee35r.us.auth0.com", jwtSettings["Issuer"]);
        Assert.Equal("https://api.taskmanager.com", jwtSettings["Audience"]);
        Assert.Equal("hJvJmi_pWWqBlCigwi7yT6YkiaJXHxST9SVs6IKPgiyXMdIQqQ1pWMPH4iIaIyd7", jwtSettings["SecretKey"]);
    }
}
