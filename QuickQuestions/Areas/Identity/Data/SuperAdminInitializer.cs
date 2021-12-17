using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Identity.Data
{
    public class SuperAdminInitializer
    {
        readonly public static string SuperAdminUserName = "admin@quickquestions.local";

        public static async Task InitializeSuperAdmin(UserManager<QuickQuestionsUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var user = await userManager.FindByEmailAsync(SuperAdminUserName);

            if (true)
            {
                if (user == null)
                {
                    QuickQuestionsUser superAdmin = new QuickQuestionsUser()
                    {
                        UserName = SuperAdminUserName,
                        Email = SuperAdminUserName,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(superAdmin, "password");
                    await userManager.AddToRoleAsync(superAdmin, RoleInitializer.RoleAdministrator);

                    return;
                }

                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, "password");
            }
            else
            {
                if (user != null)
                {
                    await userManager.DeleteAsync(user);
                }
            }
        }
    }
}
