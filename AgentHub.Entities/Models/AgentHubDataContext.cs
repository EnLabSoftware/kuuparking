using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using AgentHub.Entities.Models.Application;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Models.KuuParking;

namespace AgentHub.Entities.Models
{
    public partial class AgentHubDataContext : DataContext.DataContext
    {
        static AgentHubDataContext()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AgentHubDataContext, Migrations.Configuration>("AgentHubDataContext"));
        }

        public AgentHubDataContext()
            : base("Name=AgentHubDataContext")
        {
        }

        public DbSet<Application.Application> Applications { get; set; }
        public DbSet<ApplicationService> ApplicationServices { get; set; }
        public DbSet<ApplicationUserAudit> ApplicationUserAudits { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<PageItem> PageItems { get; set; }
        public DbSet<UserProfile> Users { get; set; }

        public DbSet<Slot> Slots { get; set; }
        public DbSet<SlotProvider> SlotProviders { get; set; }
        public DbSet<SlotBooking> SlotBookings { get; set; }
        public DbSet<SlotPayment> SlotPayments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //
            // App tables
            modelBuilder.Entity<Application.Application>().ToTable("Applications", "auth");
            modelBuilder.Entity<ApplicationService>().ToTable("ApplicationServices", "auth");
            modelBuilder.Entity<ApplicationUserAudit>().ToTable("ApplicationUserAudits", "auth");
            //
            // Common tables
            modelBuilder.Entity<Address>().ToTable("Addresses", "common");
            modelBuilder.Entity<City>().ToTable("Cities", "common");
            modelBuilder.Entity<Country>().ToTable("Countries", "common");
            modelBuilder.Entity<District>().ToTable("Districts", "common");
            modelBuilder.Entity<State>().ToTable("States", "common");
            modelBuilder.Entity<UserProfile>().ToTable("UserProfiles", "common");
            modelBuilder.Entity<Comment>().ToTable("Comments", "common");
            modelBuilder.Entity<Lookup>().ToTable("Lookups", "common");
            modelBuilder.Entity<PageItem>().ToTable("PageItems", "common");

            modelBuilder.Entity<City>().HasOptional(c => c.State).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<District>().HasOptional(c => c.City).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<UserProfile>().HasOptional(c => c.BillingAddress).WithMany().WillCascadeOnDelete(false);
            //
            // Parking slot tables
            modelBuilder.Entity<Slot>().ToTable("Slots", "park");
            modelBuilder.Entity<SlotProvider>().ToTable("SlotProviders", "park");
            modelBuilder.Entity<SlotProvider>()
                .Property(e => e.Price)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Price", 1)));
            modelBuilder.Entity<SlotProvider>()
                .Property(e => e.Price)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Price", 1)));

            modelBuilder.Entity<SlotBooking>().ToTable("SlotBookings", "park");
            modelBuilder.Entity<SlotPayment>().ToTable("SlotPayments", "park");

            modelBuilder.Entity<SlotBooking>().HasRequired(c => c.Slot).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<SlotPayment>().HasRequired(c => c.Slot).WithMany().WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
