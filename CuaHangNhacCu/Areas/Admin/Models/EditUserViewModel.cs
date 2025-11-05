using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.Areas.Admin.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; } 

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ Tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Vai trò hiện tại")]
        public string? CurrentRole { get; set; }

        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
