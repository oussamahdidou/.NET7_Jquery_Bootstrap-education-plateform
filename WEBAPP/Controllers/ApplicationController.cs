using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBAPP.Models;
using WEBAPP.VModels;

namespace WEBAPP.Controllers
{
    
    public class ApplicationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Data.Database database;
        public ApplicationController(IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, SignInManager<User> signInManager, Data.Database database)
        {
            this._webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.database = database;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = User.Identity.IsAuthenticated ? await userManager.GetUserAsync(User) : null;
            string? userId = currentUser?.Id;

            var courses = await database.Applications.ToListAsync(); // Retrieve courses asynchronously
            var ratings = await database.ProjectsRating.ToListAsync(); // Retrieve ratings asynchronously

            var response = courses
                .GroupJoin(
                    ratings,
                    c => c.Id,
                    l => l.Id_project,
                    (course, ratingGroup) => new IndexProject
                    {
                        id = course.Id,
                        Name = course.Name,
                        Description = course.Description,
                        ProjectPath = course.Url,
                        IsLiked = ratingGroup.Any(item => item.Id_project == course.Id && item.Id_User == userId),
                        likesnumber = ratingGroup.Count() ,// Count the likes
                        Author = userManager.Users.FirstOrDefault(x => x.Id == course.Author_id),
                    })
                .ToList(); // Materialize the result

            return View(response);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like(int number)
        {
            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            string username = currentUser.UserName;
            var existingItem = database.ProjectsRating
              .FirstOrDefault(item => item.Id_project == number && item.Id_User == userId);

            if (existingItem != null)
            {
                // Item exists, so delete it
                database.ProjectsRating.Remove(existingItem);
            }

            else
            {
                // Create a new item and add it
                var newItem = new ProjectRating
                {
                    Id_project = number,
                    Id_User = userId,
                    // Other properties of the item...
                };
                var notification = new Notification()
                {
                    Title = "Post Like",
                    Message = username + " Liked your post",
                    EventTime = DateTime.Now,
                    id_target_user = database.Applications.FirstOrDefault(item => item.Id == number).Author_id,
                    IsRead = false,

                };
                database.notifications.Add(notification);
                database.ProjectsRating.Add(newItem);
            }

            // Save changes to the database
            database.SaveChanges();
            int count = database.ProjectsRating
           .Count(item => item.Id_project == number);

            return Json(count);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Delete(int number)
        {
            var existingItem = database.Applications
             .FirstOrDefault(item => item.Id == number);
            if (existingItem != null)
            {
                var FilePath = existingItem.Url;


                // Get just the file name
                string fileName = Path.GetFileName(FilePath);
                var deletePath = Path.Combine(_webHostEnvironment.WebRootPath, "projects", fileName);
                // Item exists, so delete it
                database.Applications.Remove(existingItem);
                database.SaveChanges();
                System.IO.File.Delete(deletePath);
                return Json("item deleted successfully");
            }
            else
            {
                return Json("item not deleted successfully");

            }

        }
        [Authorize]
        public IActionResult Create()
        {
            var reponse = new CreateProjectVM();
            return View(reponse);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateProjectVM model)
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
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "projects", uniqueFileName);


            var apps = new Application()
            {

                Url = "~/projects/" + uniqueFileName,
                Name = model.Name,
                Description = model.Description,
                Author_id = userId,
               Created= DateTime.Now
        };

            database.Applications.Add(apps);
            await database.SaveChangesAsync();


            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await model.file.CopyToAsync(stream);
            }
            var friends = database.Follows.Where(x=>x.id_follower==currentUser.Id);
            foreach(var friend in friends)
            {
                var notification = new Notification()
                {
                    id_target_user = friend.id_following,
                    IsRead = false,
                    EventTime = DateTime.Now,
                    Message = currentUser.UserName + " added a new project",
                    Title = "New Project",
                };
                database.notifications.Add(notification);
               
            }
            database.SaveChanges();
            //var notification = new Notification()
            //{
            //    id_target_user = "",
            //    IsRead=false,
            //    EventTime = DateTime.Now,
            //    Message=currentUser.UserName+" added a new project",
            //    Title="New Project",


            //};
            return RedirectToAction("Index", "Application");

        }
    }
}
