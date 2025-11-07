using CuaHangNhacCu.Models;
using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.Areas.Admin.Models
{
    public class AdminProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ Tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }
        
        public Address? HomeAddress { get; set; }
    }
}
