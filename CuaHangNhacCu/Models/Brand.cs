using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.Models;

public class Brand
{ 
        public int Id { get; set; }
        [Required, MaxLength(100)]
    [Display(Name = "Tên thương hiệu")]
    public string? Name { get; set; }
    [Display(Name = "Mô tả")]
    [MaxLength(250)]
       
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
}
