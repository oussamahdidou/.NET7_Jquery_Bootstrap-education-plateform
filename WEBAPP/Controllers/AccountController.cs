using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEBAPP.Data;
using WEBAPP.Models;
using WEBAPP.VModels;

namespace WEBAPP.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Database database;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(IWebHostEnvironment _webHostEnvironment,UserManager<User> userManager, SignInManager<User> signInManager, Database database)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.database = database;
            this._webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Login()
        {
            var reponse = new LoginVM();
            return View(reponse);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var user = await userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                var passwordCheck = await userManager.CheckPasswordAsync(user, login.Password);
                if (passwordCheck)
                {
                    var result = await signInManager.PasswordSignInAsync(user, login.Password, false, false);

                    if (result.Succeeded)
                    {
                       
                        return RedirectToAction("Profile", "User", new { id = user.Id });
                    }
                }
                ViewData["error"] = "wrong password";
                return View(login);
            }

            ViewData["error"] = "user not found";
            return View(login);
        }
        public IActionResult Register()
        {
            var reponse = new RegisterVM();
            return View(reponse);
        }
        [HttpPost]
        public async Task <IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "form not valid";
                return View(register);
            }
            var user= await userManager.FindByEmailAsync(register.Email);
            if(user != null)
            {
                ViewData["error"] = "user exist deja";
                return View(register);
            }
            //var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(register.image.FileName);
            //var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "profiles", uniqueFileName);
            //Image_Path="~/profiles/"+uniqueFileName
            var newuser = new User()
            {
                Email = register.Email,
                UserName=register.Username,
                Gender=register.Gender,
                Nationality=register.Nationality,
                Image_Path= "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png"
            };
            var newuserregister= await userManager.CreateAsync(newuser,register.Password);
            if (newuserregister.Succeeded)
            {
                await userManager.AddToRoleAsync(newuser, UserRoles.Student);
                var result = await signInManager.PasswordSignInAsync(newuser, register.Password, false, false);
                var notification = new Notification() 
                {
                    UserId = newuser.Id,
                    IsRead = false,
                    EventTime = DateTime.Now,
                    Message = "Welcome to our community",
                    Title = "Welcome",
                };
                database.notifications.Add(notification);
                database.SaveChanges();
                return RedirectToAction("Profile", "User", new { id = newuser.Id });
            }
            //using (var stream = new FileStream(uploadPath, FileMode.Create))
            //{
            //    await register.image.CopyToAsync(stream);
            //}
            ViewData["error"] = " that passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least six characters long.";
            return View(register);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
