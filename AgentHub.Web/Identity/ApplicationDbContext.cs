using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AgentHub.Web.Identity
{
    /// <summary>
    /// ApplicationDbContext class.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim> 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        public ApplicationDbContext()
            : base("AgentHubDataContext") 
        {

        }
        static ApplicationDbContext()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>("AgentHubDataContext"));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("auth");

            //
            // Mapping for ApiRole
            modelBuilder.Entity<ApplicationRole>().Map(c =>
            {
                c.ToTable("ApplicationRoles");
                c.Properties(p => new
                {
                    p.Id,
                    p.Name
                });
            }).HasKey(p => p.Id);
            modelBuilder.Entity<ApplicationRole>().HasMany(c => c.Users).WithRequired().HasForeignKey(c => c.RoleId);

            modelBuilder.Entity<ApplicationUser>().Map(c =>
            {
                c.ToTable("ApplicationUsers");
                c.Properties(p => new
                {
                    p.Id,
                    p.AccessFailedCount,
                    p.Email,
                    p.EmailConfirmed,
                    p.PasswordHash,
                    p.PhoneNumber,
                    p.PhoneNumberConfirmed,
                    p.TwoFactorEnabled,
                    p.SecurityStamp,
                    p.LockoutEnabled,
                    p.LockoutEndDateUtc,
                    p.UserName,
                    p.CreatedDate,
                    p.ModifiedDate,
                    p.ExpiredDate,
                    p.UserProfileId,
                    p.InternationalPhoneNumber,
                    p.ExternalProviderName,
                    p.IsAdministrator
                });
            }).HasKey(c => c.Id);

            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Logins).WithOptional().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Claims).WithOptional().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Roles).WithRequired().HasForeignKey(c => c.UserId);

            modelBuilder.Entity<ApplicationUserLogin>().Map(c =>
            {
                c.ToTable("ApplicationUserLogins");
                c.Properties(p => new
                {
                    p.LoginProvider,
                    p.ProviderKey,
                    p.UserId
                });
            }).HasKey(p => new { p.LoginProvider, p.ProviderKey, p.UserId });

            modelBuilder.Entity<ApplicationUserRole>().Map(c =>
            {
                c.ToTable("ApplicationUserRoles");
                c.Properties(p => new
                {
                    p.UserId,
                    p.RoleId
                });
            }).HasKey(c => new { c.UserId, c.RoleId });

            modelBuilder.Entity<ApplicationUserClaim>().Map(c =>
            {
                c.ToTable("ApplicationUserClaims");
                c.Property(p => p.Id).HasColumnName("UserClaimId");
                c.Properties(p => new
                {
                    p.UserId,
                    p.ClaimValue,
                    p.ClaimType
                });
            }).HasKey(c => c.Id);

        }
    }
}
