using System.ComponentModel.DataAnnotations;

namespace CuaHangNhacCu.ViewModels.Profile
{
    public class AddressFormViewModel
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đường/số nhà")]
        [Display(Name = "Đường/Số nhà")]
        public required string Line1 { get; set; }

        [Display(Name = "Dòng 2")]
        public string? Line2 { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thành phố")]
        [Display(Name = "Thành phố")]
        public required string City { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public string? Province { get; set; }

        public bool IsDefault { get; set; }
    }
}
