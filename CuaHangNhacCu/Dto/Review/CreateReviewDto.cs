using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.Dto.Review;

public class CreateReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }
    [MaxLength(2000)]
    public string? Content { get; set; }
}
