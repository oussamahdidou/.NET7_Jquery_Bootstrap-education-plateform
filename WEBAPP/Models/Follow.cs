using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }
        public string id_follower { get; set; }
        public string id_following { get; set; }
    
    
    }
  
}
