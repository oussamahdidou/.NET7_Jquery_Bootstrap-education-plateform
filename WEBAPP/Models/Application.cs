using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        [MaxLength(100)]
        public string? Url { get; set; }
        public DateTime? Created { get; set; } 
        public List<ProjectRating> ProjectRatings { get; set; } = new List<ProjectRating>();

    }
}
