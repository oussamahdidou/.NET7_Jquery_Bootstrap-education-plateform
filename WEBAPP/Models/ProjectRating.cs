using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class ProjectRating
    {
        [Key]
        public int Id { get; set; }
        public int Id_project { get; set; }
        public string Id_User { get; set; }
       public int Project_rating { get; set;}
    }
}
