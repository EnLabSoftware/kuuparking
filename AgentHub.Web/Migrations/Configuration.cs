using System.Web;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Utilities;
using AgentHub.Web.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AgentHub.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            return;

            try
            {
                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

                const string name = "admin@kuuparking.com";
                const string password = "Admin@123456";
                const string roleName = "BusinessOwner";

                //Create Role Admin if it does not exist
                var role = roleManager.FindByName(roleName);
                if (role == null)
                {
                    role = new ApplicationRole(roleName) {Id = Guid.NewGuid().ToString()};
                    roleManager.Create(role);
                }

                var user = userManager.FindByName(name);
                if (user == null)
                {
                    user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = name, Email = name, UserProfileId = (int)UserProfileType.BusinessOwner, EmailConfirmed = false, PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0};
                    userManager.Create(user, password);
                    userManager.SetLockoutEnabled(user.Id, false);
                }

                // Add user admin to Role Admin if not already added
                var rolesForUser = userManager.GetRoles(user.Id);
                if (!rolesForUser.Contains(role.Name))
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }
        }
    }
}
