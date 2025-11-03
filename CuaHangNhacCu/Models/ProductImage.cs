using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.Models;

public class ProductImage
{
    public int Id { get; set; }
    [Required, MaxLength(500)]
    public string? Url { get; set; }
    public string? AltText { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public bool IsPrimary { get; set; }
}
