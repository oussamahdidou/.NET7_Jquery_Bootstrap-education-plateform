using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using WEBAPP.Models;
using WEBAPP.VModels;
using WEBAPP.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
namespace WEBAPP.Controllers
{

    public class DocumentController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Data.Database database;
        public DocumentController(IWebHostEnvironment _webHostEnvironment, UserManager<User> userManager, SignInManager<User> signInManager, Data.Database database)
        {
            this._webHostEnvironment = _webHostEnvironment;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.database = database;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            string? userId = currentUser?.Id;


            var courses = await database.Courses.Include(x => x.CourseRatings).Select(x => new IndexCourseVM
            {
                id = x.Id,
                CoursePath = x.Url,
                Description = x.Description,
                IsLiked = x.CourseRatings.Any(x => x.UserId == userId),
                likesnumber = x.CourseRatings.Count(),
                Name = x.Name

            }).ToListAsync();


            return View(courses);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like(int number)
        {
            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            var existingItem = database.CoursesRating
              .FirstOrDefault(item => item.CourseId == number && item.UserId == userId);

            if (existingItem != null)
            {
                // Item exists, so delete it
                database.CoursesRating.Remove(existingItem);
            }

            else
            {
                var newItem = new CourseRating
                {
                    CourseId = number,
                    UserId = userId,
                };

                database.CoursesRating.Add(newItem);
            }

            database.SaveChanges();
            int count = database.CoursesRating
           .Count(item => item.CourseId == number);

            return Json(count);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Delete(int number)
        {
            var existingItem = database.Courses
             .FirstOrDefault(item => item.Id == number);
            if (existingItem != null)
            {
                var FilePath = existingItem.Url;


                // Get just the file name
                string fileName = Path.GetFileName(FilePath);
                var deletePath = Path.Combine(_webHostEnvironment.WebRootPath, "courses", fileName);
                // Item exists, so delete it
                database.Courses.Remove(existingItem);
                database.SaveChanges();
                System.IO.File.Delete(deletePath);
                return Json("item deleted successfully");
            }
            else
            {
                return Json("item not deleted successfully");

            }

        }
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            var reponse = new CreateCourseVM();
            return View(reponse);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseVM model)
        {
            var currentUser = await userManager.GetUserAsync(User);



            // Access the user's ID
            string userId = currentUser.Id;

            // Your logic here

            if (model.file == null || model.file.Length == 0)
            {
                return BadRequest("Invalid file");
            }
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.file.FileName);
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "courses", uniqueFileName);


            var courses = new Course()
            {

                Url = "~/courses/" + uniqueFileName,
                Name = model.Name,
                Description = model.Description,
                UserId = userId
            };

            database.Courses.Add(courses);
            await database.SaveChangesAsync();


            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await model.file.CopyToAsync(stream);
            }
            var friends = await userManager.GetUsersInRoleAsync("student");
            foreach (var friend in friends)
            {
                var notification = new Notification()
                {
                    UserId = friend.Id,
                    IsRead = false,
                    EventTime = DateTime.Now,
                    Message = "A new course has been added",
                    Title = "New Course",
                };
                database.notifications.Add(notification);

            }
            database.SaveChanges();
            return RedirectToAction("Index", "Document");

        }
    }
}
