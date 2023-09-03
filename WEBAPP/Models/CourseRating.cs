using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class CourseRating
    {
        [Key]
        public int Id { get; set; }
        public int Id_cource { get; set; }
        public string Id_User { get; set; }
        public int course_rating { get; set; }
    }
}
