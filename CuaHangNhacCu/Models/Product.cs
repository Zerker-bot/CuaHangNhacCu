using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Models;

public class Product
{
    public int Id { get; set; }
    [Required, MaxLength(200)]
    [Display(Name = "Tên sản phẩm")]
    public string Name { get; set; }
    [MaxLength(1000)]
    public string Description { get; set; }
    [Precision(18, 2)]
    public decimal Price { get; set; }
    [Precision(18, 2)]
    public decimal Cost { get; set; }
    public bool IsPublished { get; set; } = true;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int BrandId { get; set; }
    public Brand Brand { get; set; }
    public int? SupplierId { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
