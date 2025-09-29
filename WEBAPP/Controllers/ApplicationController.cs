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

            var courses = await database.Applications.Include(x=>x.ProjectRatings).Select(x=>new IndexProject
            {
                Author=x.User,
                Description=x.Description,
                IsLiked= x.ProjectRatings.Any(x=>x.UserId==userId),
                likesnumber=x.ProjectRatings.Count(),
               Name=x.Name,
               ProjectPath=x.Url,
               id=x.Id
            }).ToListAsync();

            return View(courses);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like(int number)
        {
            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            string username = currentUser.UserName;
            var existingItem = database.ProjectsRating
              .FirstOrDefault(item => item.ApplicationId == number && item.UserId== userId);

            if (existingItem != null)
            {
                database.ProjectsRating.Remove(existingItem);
            }

            else
            {
                var newItem = new ProjectRating
                {
                   ApplicationId = number,
                   UserId= userId,
                };
                var notification = new Notification()
                {
                    Title = "Post Like",
                    Message = username + " Liked your post",
                    EventTime = DateTime.Now,
                    UserId = database.Applications.Where(item => item.Id == number).Select(x=>x.UserId).FirstOrDefault(),
                    IsRead = false,

                };
                database.notifications.Add(notification);
                database.ProjectsRating.Add(newItem);
            }

            // Save changes to the database
            database.SaveChanges();
            int count = database.ProjectsRating
           .Count(item => item.ApplicationId == number);

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
                UserId = userId,
               Created= DateTime.Now,
               
        };

            database.Applications.Add(apps);
            await database.SaveChangesAsync();


            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await model.file.CopyToAsync(stream);
            }
            var friends = database.Follows.Where(x=>x.FollowedId==currentUser.Id);
            foreach(var friend in friends)
            {
                var notification = new Notification()
                {
                    UserId = friend.FollowerId,
                    IsRead = false,
                    EventTime = DateTime.Now,
                    Message = currentUser.UserName + " added a new project",
                    Title = "New Project",
                };
                database.notifications.Add(notification);
               
            }
            database.SaveChanges();
            return RedirectToAction("Index", "Application");

        }
    }
}
