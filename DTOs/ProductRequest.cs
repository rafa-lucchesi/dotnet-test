using System.ComponentModel.DataAnnotations;

namespace ProductAPI.DTOs
{
    public class ProductRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }
    }
}
