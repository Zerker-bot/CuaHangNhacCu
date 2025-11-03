using System.ComponentModel.DataAnnotations;
namespace CuaHangNhacCu.Models;

public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    [MaxLength(2000)]
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false;
}
