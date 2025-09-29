using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        [MaxLength(100)]
        public string? Url { get; set; }
        public ICollection<CourseRating> CourseRatings { get; set; }
    }
}
