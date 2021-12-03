using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Identity.Data
{
    public class RoleInitializer
    {
        readonly public static string RoleAdministrator = "admin";
        readonly public static string RoleDepartmentManager = "manager";
        readonly public static string RoleVerifiedUser = "user";

        public static async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            if(!(await roleManager.RoleExistsAsync(RoleAdministrator)))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleAdministrator));
            }
            if (!(await roleManager.RoleExistsAsync(RoleDepartmentManager)))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleDepartmentManager));
            }
            if (!(await roleManager.RoleExistsAsync(RoleVerifiedUser)))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleVerifiedUser));
            }
        }
    }
}
