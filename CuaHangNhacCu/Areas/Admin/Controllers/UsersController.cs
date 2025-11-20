using CuaHangNhacCu.Areas.Admin.Models;
using CuaHangNhacCu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CuaHangNhacCu.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles
            .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
            .ToListAsync();

            var viewModel = new CreateUserViewModel
            {
                AllRoles = roles
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["SuccessMessage"] = "Tạo tài khoản mới thành công!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AllRoles = await _roleManager.Roles
                .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                .ToListAsync();

            return View(model);
        }
        //GET: Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var allRoles = await _roleManager.Roles
                .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                .ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(user);
            var currentRole = userRoles.FirstOrDefault();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CurrentRole = currentRole,
                AllRoles = allRoles
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllRoles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                    .ToListAsync();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                model.AllRoles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                    .ToListAsync();
                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(model.CurrentRole))
            {
                await _userManager.AddToRoleAsync(user, model.CurrentRole);
            }

            var currentAdminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isEditingSelf = (user.Id == currentAdminUserId);

            bool isNoLongerAdmin = (model.CurrentRole != "Admin");

            if (isEditingSelf && isNoLongerAdmin)
            {
                TempData["SuccessMessage"] = "Bạn đã tự thay đổi vai trò của mình và đã được đăng xuất.";

                await _signInManager.SignOutAsync();

                return RedirectToAction("Index", "Home", new { area = "" });
            }

            TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentAdminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isDeletingSelf = (user.Id == currentAdminUserId);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Đã xóa tài khoản {user.UserName} thành công.";

                if (isDeletingSelf)
                {
                    await _signInManager.SignOutAsync();

                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Xóa tài khoản thất bại.";
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction(nameof(Index)); 
        }
    }
}
