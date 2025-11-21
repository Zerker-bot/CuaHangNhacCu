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

    [Display(Name = "Mô tả")]
    public string Description { get; set; }
    [Precision(18, 2)]

    [Display(Name = "Giá bán ra")]
    public decimal Price { get; set; }
    [Precision(18, 2)]

    [Display(Name = "Giá gốc")]
    public decimal Cost { get; set; }

    [Display(Name = "Công khai")]
    public bool IsPublished { get; set; } = true;

    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    [Display(Name = "Ngày tạo sản phẩm")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Ngày cập nhật sản phẩm")]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "Loại sản phẩm")]
    public int CategoryId { get; set; }

    
    public Category Category { get; set; }

    [Display(Name = "Hãng sản xuất")]
    public int BrandId { get; set; }
    
    public Brand Brand { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
