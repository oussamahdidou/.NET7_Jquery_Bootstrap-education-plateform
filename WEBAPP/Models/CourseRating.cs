using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class CourseRating
    {
        

        public Course Course { get; set; }
        public int CourseId { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
