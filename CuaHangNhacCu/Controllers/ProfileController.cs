
using CuaHangNhacCu.Data;
using CuaHangNhacCu.Models;
using CuaHangNhacCu.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public ProfileController(UserManager<User> userManager, ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var userAddresses = await _context.Addresses
                                              .Where(a => a.UserId == user.Id)
                                              .ToListAsync();

            var defaultAddress = userAddresses.FirstOrDefault(a => a.IsDefault);
            var temporaryAddress = userAddresses.FirstOrDefault(a => !a.IsDefault);


            var viewModel = new ProfileViewModel
            {
                AvatarUrl = "https://avatar.iran.liara.run/public/43",
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                DefaultAddress = defaultAddress,
                TemporaryAddress = temporaryAddress
            };

            return View(viewModel);
        }

        // POST: /Profile/Index (Cập nhật Tên và SĐT)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            async Task LoadAddressesAndAvatar(ProfileViewModel viewModel, User currentUser)
            {
                var userAddresses = await _context.Addresses
                                                  .Where(a => a.UserId == currentUser.Id)
                                                  .ToListAsync();
                viewModel.DefaultAddress = userAddresses.FirstOrDefault(a => a.IsDefault);
                viewModel.TemporaryAddress = userAddresses.FirstOrDefault(a => !a.IsDefault);
                viewModel.AvatarUrl = "https://avatar.iran.liara.run/public/43";
            }

            if (!ModelState.IsValid)
            {
                await LoadAddressesAndAvatar(model, user);
                return View(model);
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await LoadAddressesAndAvatar(model, user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(AddressFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Cập nhật địa chỉ thất bại. Vui lòng kiểm tra lại các trường bắt buộc.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (model.AddressId == 0)
            {
                var newAddress = new Address
                {
                    UserId = user.Id,
                    Line1 = model.Line1,
                    Line2 = model.Line2,
                    City = model.City,
                    Province = model.Province,
                    IsDefault = model.IsDefault
                };
                _context.Addresses.Add(newAddress);
            }
            else
            {
                var existingAddress = await _context.Addresses.FindAsync(model.AddressId);

                if (existingAddress == null || existingAddress.UserId != user.Id)
                {
                    TempData["ErrorMessage"] = "Địa chỉ không tồn tại hoặc bạn không có quyền sửa.";
                    return RedirectToAction(nameof(Index));
                }

                existingAddress.Line1 = model.Line1;
                existingAddress.Line2 = model.Line2;
                existingAddress.City = model.City;
                existingAddress.Province = model.Province;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật địa chỉ thành công!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}
