using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class ProjectRating
    {
        public Application Application { get; set; }
        public int ApplicationId { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
