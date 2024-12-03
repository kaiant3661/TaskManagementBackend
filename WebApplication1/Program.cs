using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication1.Middlewares;  // Add this using directive for the middleware

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()   // Allow any origin
              .AllowAnyMethod()   // Allow any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();  // Allow any headers
    });
});
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization to avoid issues with circular references in entity relationships
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// JWT Configuration Validation
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];

if (string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(jwtSecretKey))
{
    throw new ArgumentNullException("JWT configuration values (Issuer, Audience, SecretKey) are missing in appsettings.json.");
}

// Add Authentication using JWT Bearer tokens
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Use the default scheme for JWT
    .AddJwtBearer(options =>
    {
        // Configure JWT options
        options.RequireHttpsMetadata = false; // Disable HTTPS metadata requirement for testing purposes
        options.SaveToken = true; // Save the token in the request for later use

        // Define token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Ensure the token's issuer matches the expected issuer
            ValidateAudience = true, // Ensure the token's audience matches the expected audience
            ValidateLifetime = true, // Ensure the token is not expired
            ValidIssuer = jwtIssuer, // Use the valid issuer from the configuration
            ValidAudience = jwtAudience, // Use the valid audience from the configuration
            IssuerSigningKey = new SymmetricSecurityKey( // Define the signing key used to validate the token
                Encoding.UTF8.GetBytes(jwtSecretKey) // Convert the secret key to bytes
            )
        };
    });

// Add other services to the container
builder.Services.AddEndpointsApiExplorer(); // Enable endpoint documentation
builder.Services.AddSwaggerGen(); // Enable Swagger for API documentation

// Configure database context with dependency injection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services for dependency injection
builder.Services.AddScoped<UserrService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<AuthService>();


var app = builder.Build();
app.UseCors("AllowAllOrigins");

// Register the exception handling middleware here
app.UseMiddleware<ExceptionMiddleware>();  // Add this line

// Configure the middleware pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger only in development for API testing
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDeveloperExceptionPage(); // This will show detailed error information during development

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware
app.MapControllers(); // Map controller routes
app.Run(); // Start the application



//Client ID and Domain:c5TrJuiPzwC9G13JtWOvPcUPyIAled66, dev-ee8n18ff4hpee35r.us.auth0.com
//identifier:https://api.taskmanager.com