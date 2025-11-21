using CuaHangNhacCu.Data;
using CuaHangNhacCu.Dto.Cart;
using CuaHangNhacCu.Models;
using CuaHangNhacCu.ViewModels.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CuaHangNhacCu.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var selectedItemsJson = TempData["SelectedCartItems"] as string;

            if (string.IsNullOrEmpty(selectedItemsJson))
            {
                return RedirectToAction("Index", "Cart");
            }

            var selectedItemIds = JsonSerializer.Deserialize<int[]>(selectedItemsJson);
            if (selectedItemIds == null || !selectedItemIds.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            var userAddresses = await _context.Addresses
                                              .Where(a => a.UserId == user.Id)
                                              .ToListAsync();

            var defaultAddress = userAddresses.FirstOrDefault(a => a.IsDefault);
            var temporaryAddress = userAddresses.FirstOrDefault(a => !a.IsDefault);

            var itemsToPurchase = new List<CartItemDto>();
            decimal subtotal = 0;

            foreach (var itemId in selectedItemIds)
            {
                var cartItem = await _context.CartItems
                                             .Include(ci => ci.Product)
                                             .FirstOrDefaultAsync(ci => ci.Id == itemId);

                if (cartItem != null)
                {
                    var primaryImage = await _context.ProductImages
                        .FirstOrDefaultAsync(pi => pi.ProductId == cartItem.ProductId && pi.IsPrimary);

                    var itemDto = new CartItemDto
                    {
                        CartItemId = cartItem.Id,
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.Product.Name,
                        ProductImageUrl = primaryImage?.Url ?? "https://placehold.co/100x100/eee/ccc?text=No+Image",
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    };

                    itemsToPurchase.Add(itemDto);
                    subtotal += itemDto.TotalItemPrice;
                }
            }

            decimal shippingFee = 15000;

            var viewModel = new CheckoutViewModel
            {
                ItemsToPurchase = itemsToPurchase,
                DefaultAddress = defaultAddress,
                TemporaryAddress = temporaryAddress,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                Total = subtotal + shippingFee
            };

            TempData.Keep("SelectedCartItems");

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPurchase(CheckoutViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var selectedItemsJson = TempData["SelectedCartItems"] as string;

            if (userId == null || string.IsNullOrEmpty(selectedItemsJson))
            {
                return RedirectToAction("Index", "Cart");
            }

            var selectedItemIds = JsonSerializer.Deserialize<int[]>(selectedItemsJson);
            if (selectedItemIds == null || !selectedItemIds.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var itemsToPurchase = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => selectedItemIds.Contains(ci.Id))
                .ToListAsync();

            if (!itemsToPurchase.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            decimal subtotal = 0;
            foreach (var item in itemsToPurchase)
            {
                subtotal += item.Product.Price * item.Quantity;
            }

            decimal shippingFee = 15000;
            decimal total = subtotal + shippingFee;

            var newOrder = new Order
            {
                UserId = userId,
                ShippingAddressId = model.SelectedAddressId, 
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ShippingFee = shippingFee,
                Discount = 0, 
            };

            foreach (var item in itemsToPurchase)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };
                newOrder.Items.Add(orderItem);
            }

            _context.Orders.Add(newOrder);

            _context.CartItems.RemoveRange(itemsToPurchase);

            await _context.SaveChangesAsync();

            TempData.Remove("SelectedCartItems");

            return RedirectToAction(nameof(ThankYou), new { orderId = newOrder.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ThankYou(int orderId)
        {
            var order = await _context.Orders
                                      .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["OrderId"] = order.Id;
            return View();
        }
    }
}
