using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiViet.Models
{
    public enum RoleStatus
    {
        Author = 0,
        Admin = 1
    }
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage ="Họ và tên bắt buộc phải nhập.")]
        [StringLength(100,ErrorMessage ="Độ dài không quá 100 ký tự.")]
        [RegularExpression(@"^[\p{L}\s]+$",ErrorMessage ="Họ tên chỉ gồm chữ và khoảng trắng")]
        public string FullName { get; set; }

        [Required(ErrorMessage ="Phần này chưa nhập nè.")]
        [DataType(DataType.DateTime)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage ="Bắt buộc phải có phần này.")]
        [EmailAddress(ErrorMessage ="Định dạng email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Tên đăng nhập độ đài 6-50")]
        [RegularExpression("^[a-zA-Z0-9]+$",ErrorMessage ="Tên đăng nhập gồm chữ số, không dấu cách , không có ký tự đặc biệt")]
        public string UserName {  get; set; }

        [Required(ErrorMessage ="Thiếu mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EnumDataType(typeof(RoleStatus), ErrorMessage ="Role không hợp lệ.")]
        public RoleStatus Role { get; set; } = RoleStatus.Author;

    }

}
