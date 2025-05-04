using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiViet.Models
{
    public enum ArticleStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
    public class Article
    {
        public int ArticleId { get; set; }

        [Required(ErrorMessage ="Bài viết chưa có tiêu đề.")]
        [StringLength(200,ErrorMessage ="Tiêu đề không quá 200 ký tự.")]
        [Display(Name = "Tiêu đề bài viết")]
        public string Title { get; set; }

        [Required]
        public int AuthorId {  get; set; }

        [Display(Name = "Tác giả")]
        public User Author { get; set; }

        [Required(ErrorMessage= "Lỗi")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày đăng")]
        public DateTime SubmitedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage ="Chưa viết tóm tắt kìa.")]
        [StringLength(500,ErrorMessage ="Độ dài không quá 500 ký tự.")]
        [Display(Name = "Tóm tắt nội dung")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Bài viết gì không có nội dung gì vậy.")]
        [MinLength(50, ErrorMessage ="Nội dung ít thế (ít nhất 50 ký tự).")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Chủ đề")]
        public int TopicID { get; set; }

        [Display(Name = "Chủ đề")]
        public Topic Topic { get; set; }
        [Required(ErrorMessage ="Trạng thái là bắt buộc")]
        [Display(Name = "Trạng thái duyệt")]
        [EnumDataType(typeof(ArticleStatus), ErrorMessage ="Trạng thái không hợp lệ.")]
        public ArticleStatus Status { get; set; } = ArticleStatus.Pending;

    }
}
