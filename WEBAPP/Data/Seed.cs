using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WEBAPP.Models;

namespace WEBAPP.Data
{
    public class Seed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new Database(serviceProvider.GetRequiredService<DbContextOptions<Database>>()))
            {
                //
            }

        }
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var RoleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (! await RoleManager.RoleExistsAsync(UserRoles.Admin)) 
                {
                    await RoleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                }
                if (!await RoleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await RoleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                }
                //Users
                var usermanager=serviceScope.ServiceProvider.GetService<UserManager<Models.User>>();
                                            //Admins
                string adminuserEmail = "oussamahdidou15@gmail.com";
                var adminUser = await usermanager.FindByEmailAsync(adminuserEmail);
                if (adminUser == null)
                {
                    var newadminUser = new User()
                    {
                        UserName = "oussamahdidou",
                        Email = adminuserEmail,
                        EmailConfirmed=true,
                        Gender="Male",
                        Nationality = "Morocco",
                        Image_Path= "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-profiles/avatar-1.webp"

                    };
                    await usermanager.CreateAsync(newadminUser,"Coding@1234?");
                    await usermanager.AddToRoleAsync(newadminUser, UserRoles.Admin);
                }
                                             //Students
                string studentuserEmail = "oussamahdidou223@gmail.com";
                var studentUser = await usermanager.FindByEmailAsync(studentuserEmail);
                if (studentUser == null)
                {
                    var newstudentUser = new User()
                    {
                        UserName = "oussamahdidou2",
                        Email = studentuserEmail,
                        EmailConfirmed = true,
                        Gender = "Male",
                        Nationality = "Morocco",
                        Image_Path = "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-profiles/avatar-1.webp"


                    };
                    await usermanager.CreateAsync(newstudentUser, "Coding@1234?");
                    await usermanager.AddToRoleAsync(newstudentUser, UserRoles.Student);
                }
            }
        }
    }
}
