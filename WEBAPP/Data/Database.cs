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
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseRating> CoursesRating { get; set;}
        public DbSet<ProjectRating> ProjectsRating { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }
        public DbSet<Notification> notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CourseRating>(x => x.HasKey(p => new { p.UserId, p.CourseId }));
            builder.Entity<CourseRating>()
            .HasOne(u => u.User)
            .WithMany(u => u.CourseRatings)
            .HasForeignKey(p => p.UserId);
            builder.Entity<CourseRating>()
            .HasOne(u => u.Course)
            .WithMany(u => u.CourseRatings)
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
            /////////////////////////////////////////////////////
            builder.Entity<ProjectRating>(x => x.HasKey(p => new { p.UserId, p.ApplicationId }));
            builder.Entity<ProjectRating>()
            .HasOne(u => u.User)
            .WithMany(u => u.ProjectRatings)
            .HasForeignKey(p => p.UserId);
            builder.Entity<ProjectRating>()
            .HasOne(u => u.Application)
            .WithMany(u => u.ProjectRatings)
            .HasForeignKey(p => p.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
            //////////////////////////////////////////////////////
            builder.Entity<Follow>(x => x.HasKey(p => new { p.FollowerId, p.FollowedId }));
            builder.Entity<Follow>()
            .HasOne(u => u.Follower)
            .WithMany(u => u.Followings)
            .HasForeignKey(p => p.FollowerId);
            builder.Entity<Follow>()
            .HasOne(u => u.Followed)
            .WithMany(u => u.Followers)
            .HasForeignKey(p => p.FollowedId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

