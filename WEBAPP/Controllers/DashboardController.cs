using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBAPP.Data;
using WEBAPP.Models;
using WEBAPP.VModels;

namespace WEBAPP.Controllers
{
    [Authorize(Roles = "admin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Data.Database database;
        private readonly RoleManager<IdentityRole> roleManager;
        public DashboardController(RoleManager<IdentityRole> roleManager,UserManager<User> userManager, SignInManager<User> signInManager, Data.Database database)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.database = database;
            this.roleManager = roleManager;
        }
        public async Task <IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var studentRoleId = await roleManager.FindByNameAsync("student");
            var usersInCustomerRole = await userManager.GetUsersInRoleAsync(studentRoleId.Name);
            int customerCount = usersInCustomerRole.Count;
            ViewData["map"]= database.Users.GroupBy(u => u.Nationality).Select(g => new { Nationality = g.Key, Count = g.Count() }).ToList();

            var reponse = new DashboardVM()
            {
                courses = database.Courses.Count(),
                Projects = database.Applications.Count(),
                Students = customerCount,
                Relations = database.Follows.Count(),
                image = currentUser.Image_Path,
                username = currentUser.UserName 

        };
            return View(reponse);
        }





    }
}
