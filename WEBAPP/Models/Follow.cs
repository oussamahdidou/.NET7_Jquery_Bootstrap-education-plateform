using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class Follow
    {
        public string FollowerId { get; set; }
        public User Follower { get; set; }

        public string FollowedId { get; set; }
        public User Followed { get; set; }

        public DateTime FollowedOn { get; set; } = DateTime.UtcNow;


    }
  
}
