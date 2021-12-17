using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Identity.Data
{
    public class SuperAdminInitializer
    {
        readonly public static string SuperAdminUserName = "admin@quickquestions";

        public static async Task InitializeSuperAdmin(IConfigurationRoot configuration,UserManager<QuickQuestionsUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            IConfigurationSection superAdminConfiguration = configuration.GetSection("SuperAdmin");

            var user = await userManager.FindByEmailAsync(SuperAdminUserName);

            if (Convert.ToBoolean(superAdminConfiguration["Enabled"]))
            {
                if (user == null)
                {
                    QuickQuestionsUser superAdmin = new QuickQuestionsUser()
                    {
                        UserName = SuperAdminUserName,
                        Email = SuperAdminUserName,
                        Surname = "System",
                        Name = "Administrator",
                        Patronymic = "Account",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(superAdmin, superAdminConfiguration["Password"]);

                    if(result == IdentityResult.Success)
                    {
                        await userManager.AddToRoleAsync(superAdmin, RoleInitializer.RoleAdministrator);
                    }

                    return;
                }

                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, superAdminConfiguration["Password"]);
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
