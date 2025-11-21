using CuaHangNhacCu.Data;
using CuaHangNhacCu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;

namespace CuaHangNhacCu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExportFileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExportFileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // =================================================================
        // CHỨC NĂNG 1: XUẤT FILE DANH SÁCH ĐƠN HÀNG ĐANG XỬ LÝ (SHIP) 📦
        // =================================================================

        public async Task<IActionResult> ExportPendingOrders()
        {
            var orders = await GetPendingShipmentDetails();
            var csvContent = CreateShippingCsvContent(orders);

            // Thêm BOM UTF-8 vào đầu
            var bom = Encoding.UTF8.GetPreamble(); // EF BB BF
            var csvBytes = Encoding.UTF8.GetBytes(csvContent);

            // Ghép BOM + dữ liệu
            var fileBytes = bom.Concat(csvBytes).ToArray();

            return File(
                fileContents: fileBytes,
                contentType: "text/csv",
                fileDownloadName: $"DanhSachShip_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            );
        }

        // --- Hàm hỗ trợ Truy vấn ---
        private async Task<List<Order>> GetPendingShipmentDetails()
        {
            // Đã kiểm tra lại cú pháp Include, cú pháp này là chính xác.
            // Nếu vẫn lỗi, hãy kiểm tra lại các khóa ngoại trong DbContext và Model.
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Processing)
                .Include(o => o.ShippingAddress)
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();
        }

        // --- Hàm hỗ trợ Tạo CSV (Đã Tăng Cường Kiểm Tra Null) ---
        private string CreateShippingCsvContent(List<Order> orders)
        {
            var sb = new StringBuilder();

            // ⚠️ HEADER MỚI: Chỉ có 1 dòng cho mỗi đơn hàng. Chi tiết sản phẩm được gộp vào 1 cột.
            sb.AppendLine("Mã Đơn Hàng,Tên Khách Hàng,Địa Chỉ Giao Hàng,Tổng Số Lượng,Tổng Tiền Đơn Hàng,Chi Tiết Sản Phẩm");

            foreach (var order in orders)
            {
                var customerName = order.User?.FullName ?? "Khách lẻ (ID: " + order.UserId + ")";
                var totalAmount = order.Total.ToString("F2", CultureInfo.InvariantCulture);

                var shippingAddress = "Không có địa chỉ";
                if (order.ShippingAddress != null)
                {
                    var address = order.ShippingAddress;
                    // Nối các thành phần địa chỉ
                    shippingAddress = $"{address.Line1}";
                    if (!string.IsNullOrEmpty(address.Line2))
                        shippingAddress += $", {address.Line2}";
                    shippingAddress += $", {address.City}";
                    if (!string.IsNullOrEmpty(address.Province))
                        shippingAddress += $", {address.Province}";
                }

                // TÍNH TOÁN VÀ GỘP CHI TIẾT SẢN PHẨM
                var totalQuantity = 0;
                var productDetails = "Không có sản phẩm";

                if (order.Items != null && order.Items.Any())
                {
                    // 1. Gộp chi tiết từng sản phẩm thành một chuỗi duy nhất
                    productDetails = string.Join(" | ", order.Items.Select(item =>
                    {
                        var name = item.Product?.Name ?? "Sản phẩm không tên";
                        return $"{name} x {item.Quantity}";
                    }));

                    // 2. Tính tổng số lượng
                    totalQuantity = order.Items.Sum(item => item.Quantity);
                }

                // 🚀 XUẤT DỮ LIỆU CHỈ MỘT DÒNG CHO MỖI ĐƠN HÀNG
                sb.AppendLine(
                    $"{order.Id}," +
                    $"\"{customerName}\"," +
                    $"\"{shippingAddress}\"," +
                    $"{totalQuantity}," +
                    $"{totalAmount}," +
                    // Chi tiết sản phẩm được bọc trong dấu ngoặc kép để tránh bị cắt khi có dấu phẩy/pipe
                    $"\"{productDetails}\""
                );
            }
            return sb.ToString();
        }

        // =================================================================
        // CHỨC NĂNG 2: THỐNG KÊ CÁC ĐƠN HÀNG TRONG TUẦN, THÁNG 📊
        // =================================================================

        public async Task<IActionResult> GetSalesSummary(string type = "month", int lookback = 3)
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate;

            if (type.ToLower() == "week")
            {
                startDate = endDate.AddDays(-7 * lookback);
                ViewData["Title"] = $"Thống kê Doanh thu {lookback} tuần gần nhất";
            }
            else
            {
                startDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-(lookback - 1));
                ViewData["Title"] = $"Thống kê Doanh thu {lookback} tháng gần nhất";
            }

            var baseQuery = _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt >= startDate && o.CreatedAt <= endDate);

            var summaryData = new List<object>();

            if (type.ToLower() == "week")
            {
                var dataInMem = await baseQuery.ToListAsync(); // Load vào bộ nhớ

                summaryData = dataInMem
                    .GroupBy(o => new
                    {
                        Year = o.CreatedAt.Year,
                        Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.CreatedAt, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                    })
                    .Select(g => new
                    {
                        Key = $"Tuần {g.Key.Week} - {g.Key.Year}",
                        TotalOrders = g.Count(),
                        TotalRevenue = g.Sum(o => o.Total)
                    })
                    .OrderBy(r => r.Key)
                    .ToList<object>();
            }
            else
            {
                // 1. FIX LỖI: Tải dữ liệu từ DB vào bộ nhớ trước.
                // Chỉ có WHERE clause (bộ lọc) được thực hiện trên Server. 
                var dataInMem = await baseQuery.ToListAsync();

                // 2. Thực hiện Grouping và Sum trên bộ nhớ (Client-side)
                summaryData = dataInMem
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new
                    {
                        Key = $"Tháng {g.Key.Month}/{g.Key.Year}",
                        TotalOrders = g.Count(),
                        TotalRevenue = g.Sum(o => o.Total) // Đã an toàn vì chạy trên bộ nhớ
                    })
                    .OrderBy(r => r.Key)
                    .ToList<object>(); // Dùng ToList() vì dataInMem đã là List trong bộ nhớ
            }

            // GỌI VIEW: Đảm bảo tên View ở đây khớp với tên tệp View của bạn (ví dụ: Index)
            // Nếu tệp View là Index.cshtml:
            return PartialView("_SalesSummaryTable", summaryData);
        }
    }
}