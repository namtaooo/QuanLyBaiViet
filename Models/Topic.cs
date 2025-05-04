using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiViet.Models
{
    public class Topic
    {
        public int TopicId { get; set; }

        [Required(ErrorMessage ="Phần này là bắt buộc")]
        [StringLength(50, ErrorMessage ="Tối đa 50 ký tự.")]
        public string TopicName { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}
