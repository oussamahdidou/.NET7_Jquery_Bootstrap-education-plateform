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
    [Authorize]
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
            string userId = currentUser.Id;

            var courses = await database.Courses.ToListAsync(); // Retrieve courses asynchronously
            var ratings = await database.CoursesRating.ToListAsync(); // Retrieve ratings asynchronously

            var response = courses
                .GroupJoin(
                    ratings,
                    c => c.Id,
                    l => l.Id_cource,
                    (course, ratingGroup) => new IndexCourseVM
                    {
                        id = course.Id,
                        Name = course.Name,
                        Description = course.Description,
                        CoursePath = course.Url,
                        IsLiked = ratingGroup.Any(item => item.Id_cource == course.Id && item.Id_User == userId),
                        likesnumber = ratingGroup.Count() // Count the likes
                    })
                .ToList(); // Materialize the result

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Like(int number)
        {
            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            var existingItem = database.CoursesRating
              .FirstOrDefault(item => item.Id_cource == number && item.Id_User == userId);

            if (existingItem != null)
            {
                // Item exists, so delete it
                database.CoursesRating.Remove(existingItem);
            }

            else
            {
                // Create a new item and add it
                var newItem = new CourseRating
                {
                    Id_cource = number,
                    Id_User = userId,
                    // Other properties of the item...
                };

                database.CoursesRating.Add(newItem);
            }

            // Save changes to the database
            database.SaveChanges();
            int count = database.CoursesRating
           .Count(item => item.Id_cource == number);

            return Json(count);
        }
        [HttpPost]
        public IActionResult Delete(int number) {
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
                database.SaveChanges() ;
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


            var courses = new Courses()
            {
                
                Url= "~/courses/" + uniqueFileName,
                Name = model.Name,
                Description = model.Description,
                Author_id = userId
            };

            database.Courses.Add(courses);
            await database.SaveChangesAsync();
          

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await model.file.CopyToAsync(stream);
            }

            return RedirectToAction("Index","Document");

        }
    }
}
