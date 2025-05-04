using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiViet.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Tên đăng nhập độ đài 6-50")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập gồm chữ số, không dấu cách , không có ký tự đặc biệt")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Thiếu mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
