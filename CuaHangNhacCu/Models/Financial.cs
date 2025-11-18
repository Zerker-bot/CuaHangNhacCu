using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace CuaHangNhacCu.Models
{
    public class Financial
    {
        public int Id { get; set; }

        [Display(Name = "Ngày giao dịch")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Display(Name = "Số tiền")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Loại giao dịch")]
        public TransactionType Type { get; set; } // Enum: Income (Thu) hoặc Expense (Chi)

        [Display(Name = "Mô tả")]
        public string Description { get; set; } // Ví dụ: "Bán đàn Guitar Fender", "Trả tiền điện", "Nhập hàng"

        // (Tùy chọn) Liên kết với Đơn hàng nếu là khoản thu từ bán hàng
        public int? OrderId { get; set; }
    }
}

public enum TransactionType
{
    Income = 1, // Thu (Bán hàng)
    Expense = 2 // Chi (Nhập hàng, điện nước, lương...)
}