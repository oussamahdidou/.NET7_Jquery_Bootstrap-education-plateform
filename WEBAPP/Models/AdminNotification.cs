using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class AdminNotification
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime EventTime { get; set; }
        public bool IsRead { get; set; }
    }
}
