using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MistGoApi.Data;
using MistGoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port from environment or default
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://+:{port}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add PostgreSQL Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Host=postgres;Database=mistgo;Username=mistgouser;Password=mistgopass";
    options.UseNpgsql(connectionString);
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyForDevelopmentOnly12345678";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MistGoApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MistGoApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Register JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Wait a bit for PostgreSQL to be fully ready
        Thread.Sleep(2000);
        
        // Only drop and recreate in development environment
        if (app.Environment.IsDevelopment())
        {
            // Drop existing database and recreate (for development)
            dbContext.Database.EnsureDeleted();
            Console.WriteLine("Old database deleted (if existed)");
        }
        
        // Create database and all tables if they don't exist
        var created = dbContext.Database.EnsureCreated();
        Console.WriteLine($"Database created: {created}");
        
        // Verify tables exist
        var hasUsersTable = dbContext.Database.CanConnect();
        Console.WriteLine($"Database connection test: {hasUsersTable}");
        
        // Test query to verify Users table
        var userCount = dbContext.Users.Count();
        Console.WriteLine($"Users table exists with {userCount} users");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add CORS
app.UseCors("AllowAll");

// Add Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => "Healthy");

app.Run();