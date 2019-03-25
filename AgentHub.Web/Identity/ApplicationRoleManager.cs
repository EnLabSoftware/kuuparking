using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace AgentHub.Web.Identity
{
    /// <summary>
    /// ApplicationRoleManager class.
    /// </summary>
    public class ApplicationRoleManager : RoleManager<ApplicationRole, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRoleManager"/> class.
        /// </summary>
        /// <param name="roleStore">The role store.</param>
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        /// <summary>
        /// Creates the specified application role.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(
                new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }
}