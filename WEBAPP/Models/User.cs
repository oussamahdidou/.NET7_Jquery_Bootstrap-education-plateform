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

    }
}

