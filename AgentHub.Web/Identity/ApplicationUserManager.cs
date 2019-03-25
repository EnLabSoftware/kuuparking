using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AgentHub.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace AgentHub.Web.Identity
{
    /// <summary>
    /// ApplicationUserManager class.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserManager"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }

        /// <summary>
        /// Creates the specified application user.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The application user created
        /// </returns>
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>()))
            {
                EmailService = new EmailService(),
                SmsService = new SmsService()
            };
            // Register two factor authentication providers. This application 
            // uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider(
                "PhoneCode",
                new PhoneNumberTokenProvider<ApplicationUser>
                {
                    MessageFormat = StringTable.YourSecurityCodeIs
                });

            manager.RegisterTwoFactorProvider(
                "EmailCode",
                new EmailTokenProvider<ApplicationUser>
                {
                    Subject = StringTable.SecurityCode,
                    BodyFormat = StringTable.YourSecurityCodeIs
                });

            // Configure validation logic for usernames 
            manager.UserValidator = new UserValidator<ApplicationUser, string>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            // Configure validation logic for passwords 
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, string>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}
