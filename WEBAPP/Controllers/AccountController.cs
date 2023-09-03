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
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, Database database)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.database = database;
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
                        return RedirectToAction("Index","home");
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
                return View(register);
            }
            var user= await userManager.FindByEmailAsync(register.Email);
            if(user != null)
            {
                ViewData["error"] = "user exist deja";
                return View(register);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(register.image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "profiles", uniqueFileName);
            var newuser = new User()
            {
                Email = register.Email,
                UserName=register.Username,
                Gender=register.Gender,
                Nationality=register.Nationality,
                Image_Path=uploadPath
            };
            var newuserregister= await userManager.CreateAsync(newuser,register.Password);
            if (newuserregister.Succeeded)
            {
                await userManager.AddToRoleAsync(newuser, UserRoles.Student);
            }
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await register.image.CopyToAsync(stream);
            }
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
