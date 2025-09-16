using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MistGoApi.Data;
using MistGoApi.DTOs;
using MistGoApi.Models;
using System.Security.Claims;

namespace MistGoApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var userId = GetUserId();
            var items = await _context.Items
                .Where(i => i.UserId == userId)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var userId = GetUserId();
            var item = await _context.Items
                .Where(i => i.Id == id && i.UserId == userId)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    CreatedAt = i.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var itemDto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Quantity = item.Quantity,
                CreatedAt = item.CreatedAt
            };

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, itemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (item == null)
                return NotFound();

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Price = dto.Price;
            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            var itemDto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Quantity = item.Quantity,
                CreatedAt = item.CreatedAt
            };

            return Ok(itemDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var userId = GetUserId();
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (item == null)
                return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}