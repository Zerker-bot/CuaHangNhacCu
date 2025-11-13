using CuaHangNhacCu.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CuaHangNhacCu.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public string? AvatarUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ Tên")]
        public string FullName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        public Address? DefaultAddress { get; set; }
        public Address? TemporaryAddress { get; set; }
    }
}
