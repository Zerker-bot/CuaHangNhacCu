using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.Models;

public class Category
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    [Display(Name = "Tên danh mục")]
    public string? Name { get; set; }
    [MaxLength(250)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
