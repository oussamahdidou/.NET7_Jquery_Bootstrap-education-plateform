using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBAPP.Models;
using WEBAPP.VModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WEBAPP.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Data.Database database;
        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, Data.Database database)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.database = database;
        }
        public async Task <IActionResult> Index()
        {
            var users = await userManager.GetUsersInRoleAsync("student");
            

            return View(users);
        }
        public async Task<IActionResult> Profile(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            var Response = new ProfileVM()
            {
                ProfileId = user.Id,
                Name = user.UserName,
                ProfileImage = user.Image_Path,
                Nationality = user.Nationality,
                followers = database.Follows.Count(x => x.id_following == id),
                following = database.Follows.Count(x => x.id_follower == id),
                postCount = database.Applications.Count(x => x.Author_id == id),
                HisAccount = id == userId,
                FollowStatus = database.Follows
              .FirstOrDefault(item => item.id_follower == id && item.id_following == userId) == null,
                NmbrMessages = database.notifications.Count(item => item.id_target_user == id && item.IsRead == false),
                Notifications = database.notifications.Where(x => x.id_target_user == id ).OrderByDescending(x => x.EventTime).ToList(),

            };
            return View(Response);
        }
        [HttpPost]
        public async Task <IActionResult> Follow(string number)
        {
            string ButtonStatus="";
            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            string username = currentUser.UserName;
            var existingItem = database.Follows
              .FirstOrDefault(item => item.id_follower == number && item.id_following == userId);

            if (existingItem != null)
            {
                // Item exists, so delete it
                database.Follows.Remove(existingItem);
                ButtonStatus = "Follow";
            }

            else
            {
                // Create a new item and add it
                var newItem = new Follow
                {
                    id_follower= number,
                    id_following = userId,
                    // Other properties of the item...
                };
                ButtonStatus = "Unfollow";
                database.Follows.Add(newItem);
                var notification = new Notification()
                {
                    Title = "New Follower",
                    Message = username + " Started following you",
                    EventTime = DateTime.Now,
                    id_target_user = number,
                    IsRead = false,

                };
                database.notifications.Add(notification);
            }

            // Save changes to the database
            database.SaveChanges();
            

            return Json(new
            {
                followers = database.Follows.Count(x => x.id_following == number),
                following = database.Follows.Count(x => x.id_follower == number),
                postCount = database.Applications.Count(x => x.Author_id == number),
                buttonStatus = ButtonStatus,

            });

        }
        [HttpGet]
        public async Task<IActionResult> Notification() 
        { 


            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            var notifications = database.notifications.Where(x => x.id_target_user == userId);
            foreach(var notification in notifications)
            {
                notification.IsRead= true;
               
            }
            database.SaveChanges();
             var list = database.notifications.Where(x=> x.id_target_user == userId)
                .OrderByDescending(x => x.EventTime)
                .Take(10)
                .ToList();
            return Json(list);
        }
        [HttpGet]
        public async Task<IActionResult> Checknotificationsupdate()
        {
            var currentUser = await userManager.GetUserAsync(User);
            string userId = currentUser.Id;

           var notificationcount= database.notifications.Count(x=>x.IsRead==false && x.id_target_user==userId);
            database.SaveChanges();
            return Json(notificationcount);
        }
    }
}
