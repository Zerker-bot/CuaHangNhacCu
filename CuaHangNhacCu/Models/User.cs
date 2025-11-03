using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CuaHangNhacCu.Models;

public class User: IdentityUser
{
        [MaxLength(100)]
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
}
