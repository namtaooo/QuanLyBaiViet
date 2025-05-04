using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiViet.Models
{
    public class ArticleModel
    {
        [Required(ErrorMessage = "Bài viết chưa có tiêu đề.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không quá 200 ký tự.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Chưa viết tóm tắt kìa.")]
        [StringLength(500, ErrorMessage = "Độ dài không quá 500 ký tự.")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Bài viết gì không có nội dung gì vậy.")]
        [MinLength(50, ErrorMessage = "Nội dung ít thế (ít nhất 50 ký tự).")]
        public string Content { get; set; }

        [Required]
        [Display(Name ="Chủ đề")]
        public int TopicID { get; set; }
    }
    public class AuthorStat
    {
        public string AuthorName { get; set; }
        public int ArticleCount { get; set; }
    }
    public class TopicStat
    {
        public string TopicName { get; set; }
        public int TopicCount { get; set; }
    }
    public class StatusStat
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
    public class ViewModels
    {
        public List<AuthorStat> AuthorStats { get; set; }
        public List<TopicStat> TopicStats { get; set; }
        public StatusStat StatusStats { get; set; }
    }
}
