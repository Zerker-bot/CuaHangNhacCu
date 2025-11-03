using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.Models;

public class Brand
{ 
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
}
