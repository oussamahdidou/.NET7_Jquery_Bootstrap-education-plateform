using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace WEBAPP.Models
{
    public class User:IdentityUser
    {
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        [MaxLength(100)]
        public string Image_Path { get; set; }
        public List<Application> Applications { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
        public List<CourseRating> CourseRatings { get; set; } = new();
        public List<ProjectRating> ProjectRatings { get; set; } = new();
        public List<Follow> Followers { get; set; } = new();
        public List<Follow> Followings { get; set; } = new();
        public List<Notification> Notifications { get; set; }= new();


    }
}

