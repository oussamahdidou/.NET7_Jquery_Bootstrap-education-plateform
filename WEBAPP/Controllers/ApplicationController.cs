using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEBAPP.Models;
using WEBAPP.VModels;

namespace WEBAPP.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly Data.Database database;
        public ApplicationController(UserManager<User> userManager, SignInManager<User> signInManager, Data.Database database)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.database = database;
        }
        public IActionResult Index()
        {
            return View();
        }
 
        public IActionResult Create()
        {
            var reponse = new CreateProjectVM();
            return View(reponse);
        }
        [HttpPost]
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
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Projects", uniqueFileName);


            var apps = new Application()
            {

                Url = uploadPath,
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

            return RedirectToAction("Index", "Document");

        }
    }
}
