using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AgentHub.Web.Identity
{
    /// <summary>
    /// ApplicationDbInitializer class.
    /// </summary>
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        public ApplicationDbInitializer()
        {
            //AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = false;
        }

        /// <summary>
        /// Override the Seeds method.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentity(context);
            base.Seed(context);
        }

        /// <summary>
        /// Initializes the identity.
        /// </summary>
        /// <param name="db">The database.</param>
        public static void InitializeIdentity(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            const string name = "admin@kuuparking.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName);
                roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name };
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
    }
}
