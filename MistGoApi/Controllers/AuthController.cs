using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MistGoApi.Data;
using MistGoApi.DTOs;
using MistGoApi.Models;
using MistGoApi.Services;

namespace MistGoApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest(new { error = "Username already exists" });

            // Create new user
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return Ok(new 
            { 
                token,
                user = new { user.Id, user.Username }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { error = "Invalid username or password" });

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return Ok(new 
            { 
                token,
                user = new { user.Id, user.Username }
            });
        }
    }
}