using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class Courses
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author_id { get; set; }
        [MaxLength(100)]
        public string Url { get; set; }
    }
}
