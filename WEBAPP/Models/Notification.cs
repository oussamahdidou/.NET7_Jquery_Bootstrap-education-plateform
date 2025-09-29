﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace WEBAPP.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public DateTime EventTime { get; set; }
        public bool IsRead { get; set; }

    }
}
