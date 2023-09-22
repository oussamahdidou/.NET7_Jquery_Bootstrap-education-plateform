using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
           // ViewData["map"]= database.Users.GroupBy(u => u.Nationality).Select(g => new { Nationality = g.Key, Count = g.Count() }).ToList();

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
        public async Task<IActionResult> DashboardData()
        {
           var gender= database.Users.GroupBy(u => u.Gender).Select(g => new { Gender = g.Key, Count = g.Count() }).ToList();
            var monthlyCounts = await database.Applications
                 .GroupBy(a => a.Created.Value.Month)
                 .Select(group => new
                 {
                     Month = group.Key,
                     RowCount = group.Count()
                 })
                 .ToListAsync();

            // Create a dictionary to store the results with all months (including zero counts)
            var result = Enumerable.Range(1, 12)
                .Select(monthNumber => new
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber),
                    RowCount = monthlyCounts.FirstOrDefault(mc => mc.Month == monthNumber)?.RowCount ?? 0
                })
                .ToList();

            var courses = await database.Courses.ToListAsync(); // Retrieve courses asynchronously
            var ratings = await database.CoursesRating.ToListAsync(); // Retrieve ratings asynchronously

            var response = courses
                .GroupJoin(
                    ratings,
                    c => c.Id,
                    l => l.Id_cource,
                    (course, ratingGroup) => new 
                    {
                        id = course.Id,
                        Name=course.Name,
                        likesnumber = ratingGroup.Count(),// Count the likes

                    }).OrderByDescending(x=>x.likesnumber)
                .ToList(); // Materialize the result

            var usersWithApplications = database.Users
         .Select(user => new
         {
             User = user.UserName,
             Applicationcount = database.Applications
                 .Where(app => app.Author_id == user.Id)
                 .Count()
         })
         .OrderByDescending(u => u.Applicationcount)
         .Take(6)
         .ToList();

            return Json(new {gender=gender,result=result,response=response,usersapp=usersWithApplications});       
        }

        public async Task<IActionResult> DashboardMap()
        {
            var result = database.Users
            .GroupBy(u => u.Nationality)
            .Select(g => new
            {
                Nationality = g.Key,
                Count = g.Count()
            })
            .ToList();
            return Json(result);
        }



    }
}
