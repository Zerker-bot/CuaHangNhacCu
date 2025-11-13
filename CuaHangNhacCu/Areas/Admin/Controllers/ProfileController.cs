using CuaHangNhacCu.Areas.Admin.Models;
using CuaHangNhacCu.Data;
using CuaHangNhacCu.Models;
using CuaHangNhacCu.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangNhacCu.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); 
            }

            var homeAddress = await _context.Addresses
                                            .Where(a => a.UserId == user.Id)
                                            .FirstOrDefaultAsync();

            var viewModel = new AdminProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                HomeAddress = homeAddress 
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdminProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            async Task LoadHomeAddress(AdminProfileViewModel viewModel, User currentUser)
            {
                viewModel.HomeAddress = await _context.Addresses
                                                    .Where(a => a.UserId == currentUser.Id)
                                                    .FirstOrDefaultAsync();
            }

            if (!ModelState.IsValid)
            {
                await LoadHomeAddress(model, user); 
                return View(model);
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email; 
            }

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

            await LoadHomeAddress(model, user);
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
                    City = model.City,
                    IsDefault = true
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
                existingAddress.City = model.City;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật địa chỉ thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

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
