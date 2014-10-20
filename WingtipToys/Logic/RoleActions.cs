using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WingtipToys.Logic
{
    internal class RoleActions
    {
        internal void createAdmin()
        {
            // Access the application context and create result variables
            Models.ApplicationDbContext context = new ApplicationDbContext();
            IdentityResult IdRoleResult;
            IdentityResult IdUserResult;

            // Create a RoleStore object by using the ApplicationDBContext object
            // The RoleManager will only be allowed to contain IdentityRole objects
            var roleStore = new RoleStore<IdentityRole>(context);

            // Create a RoleManager object that is only allowed to contain IdentityRole objects
            // When creating the RoleManager object, you pass in (as parameter) a new RoleStore object
            var roleMgr = new RoleManager<IdentityRole>(roleStore);

            // Then you create the "Administrator" role if it doesn't already exist
            if (!roleMgr.RoleExists("Administrator"))
            {
                IdRoleResult = roleMgr.Create(new IdentityRole("Administrator"));
                if (!IdRoleResult.Succeeded)
                {
                    // Handle the error condition if there's a problem creating the RoleManager object
                }
            }

            // Create a UserManager object based on the UserStore object and the ApplicationDBContext object
            // Note that you can create new objects and use them as parameters in a single line of code,
            // rather than as we did for the RoleManager
            var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var appUser = new ApplicationUser()
            {
                UserName = "Admin@gmail.com",
            };
            IdUserResult = userMgr.Create(appUser, "Pa$$word");

            // If the new "Admin" user was successfully created,
            // Add the "Admin" user to the "Administrator" Role
            if (IdUserResult.Succeeded)
            {
                IdUserResult = userMgr.AddToRole(appUser.Id, "Administrator");
                if (!IdUserResult.Succeeded)
                {
                    // Handle the error condition if there's a problem adding the user to the role
                }
            }
            else
            {
                // Handle the error condition if there's a problem creating the new user
            }
        }
    }
}