using System.ComponentModel.DataAnnotations;

namespace MistGoApi.DTOs
{
    public class ItemDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
    
    public class CreateItemDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
    
    public class UpdateItemDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}