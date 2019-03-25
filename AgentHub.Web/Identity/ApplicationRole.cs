using Microsoft.AspNet.Identity.EntityFramework;

namespace AgentHub.Web.Identity
{
    /// <summary>
    /// ApplicationRole class.
    /// </summary>
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRole"/> class.
        /// </summary>
        public ApplicationRole()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRole"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ApplicationRole(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// ApplicationRoleStore class.
    /// </summary>
    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRoleStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ApplicationRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    } 
}
