using WEBAPP.Models;

namespace WEBAPP.VModels
{
    public class ProfileVM
    {
        public string Name { get; set; }
        public string ProfileId { get; set; }
        public string Nationality { get; set; }
        public string ProfileImage { get; set; }
        public int postCount { get; set; }
        public int followers { get; set; }  
        public int following { get; set; }
        public bool HisAccount { get; set; }
        public bool FollowStatus { get; set;}
        public int NmbrMessages { get; set;}
        public List<Notification> Notifications { get; set; }

    }
}
