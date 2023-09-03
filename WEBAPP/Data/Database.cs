using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WEBAPP.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace WEBAPP.Data
{
    public class Database:IdentityDbContext<User>
    {
    
    
           public Database(DbContextOptions<Database> options) : base(options)
        {
            
        }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<CourseRating> CoursesRating { get; set;}
        public DbSet<ProjectRating> ProjectsRating { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }
        public DbSet<Notification> notifications { get; set; }

    }
}

