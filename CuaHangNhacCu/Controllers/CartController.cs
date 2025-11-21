using CuaHangNhacCu.Data;
using CuaHangNhacCu.Dto.Cart;
using CuaHangNhacCu.Models;
using CuaHangNhacCu.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;
using System.Security.Claims;

namespace CuaHangNhacCu.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge();
            }

            var cartItems = await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .Select(ci => new CartItemDto
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,

                    ProductImageUrl = _context.ProductImages
                        .Where(pi => pi.ProductId == ci.ProductId && pi.IsPrimary)
                        .Select(pi => pi.Url)
                        .FirstOrDefault() ?? "https://placehold.co/100x100/eee/ccc?text=No+Image",

                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                })
                .ToListAsync();

            var viewModel = new CartViewModel
            {
                Items = cartItems
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyNow(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity; 
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();

            var selectedItems = new int[] { cartItem.Id };
            var selectedItemsJson = JsonSerializer.Serialize(selectedItems);
            TempData["SelectedCartItems"] = selectedItemsJson;

            return RedirectToAction("Index", "Checkout");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đã thêm vào giỏ hàng!" });
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (userCart == null || cartItem.CartId != userCart.Id)
            {
                return Forbid(); 
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                return await Remove(cartItemId);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            var cartItem = await _context.CartItems
                                         .Include(ci => ci.Product)
                                         .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == userCart.Id);

            if (cartItem == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            if (newQuantity > cartItem.Product.Quantity)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Số lượng tồn kho không đủ (Chỉ còn {cartItem.Product.Quantity})."
                });
            }

            cartItem.Quantity = newQuantity;
            await _context.SaveChangesAsync();

            decimal newItemTotal = cartItem.Quantity * cartItem.Product.Price;

            return Ok(new
            {
                success = true,
                newItemTotal = newItemTotal,
                newItemTotalFormatted = newItemTotal.ToString("N0") + " VNĐ"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PrepareCheckout(int[] selectedItems)
        {
            if (selectedItems == null || selectedItems.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm để mua.";
                return RedirectToAction(nameof(Index)); 
            }

            var selectedItemsJson = System.Text.Json.JsonSerializer.Serialize(selectedItems);

            TempData["SelectedCartItems"] = selectedItemsJson;

            return RedirectToAction("Index", "Checkout");
        }
    }
}
