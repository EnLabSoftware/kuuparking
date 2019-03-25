namespace AgentHub.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgentHub : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "common.Addresses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StreetNumber = c.String(maxLength: 20),
                        Street = c.String(nullable: false, maxLength: 80),
                        ZipCode = c.String(maxLength: 10),
                        AddressLine = c.String(maxLength: 250),
                        DistrictId = c.Int(),
                        CityId = c.Int(),
                        StateId = c.Int(),
                        CountryId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("common.Cities", t => t.CityId)
                .ForeignKey("common.Countries", t => t.CountryId)
                .ForeignKey("common.Districts", t => t.DistrictId)
                .ForeignKey("common.States", t => t.StateId)
                .Index(t => t.DistrictId)
                .Index(t => t.CityId)
                .Index(t => t.StateId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "common.Cities",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ReferenceCode = c.String(maxLength: 10),
                        StateId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("common.States", t => t.StateId)
                .Index(t => t.StateId);
            
            CreateTable(
                "common.States",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        PhoneCallCode = c.String(maxLength: 5),
                        ReferenceCode = c.String(maxLength: 10),
                        CountryId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("common.Countries", t => t.CountryId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "common.Countries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        PhoneCallCode = c.String(maxLength: 5),
                        ReferenceCode = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "common.Districts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ReferenceCode = c.String(maxLength: 10),
                        CityId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("common.Cities", t => t.CityId)
                .Index(t => t.CityId);
            
            CreateTable(
                "auth.Applications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ApplicationKey = c.String(nullable: false, maxLength: 50),
                        ApplicationName = c.String(maxLength: 100),
                        ApplicationDescription = c.String(maxLength: 250),
                        RequestedOn = c.DateTime(nullable: false),
                        ActivatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "auth.ApplicationServices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ServiceKey = c.String(nullable: false, maxLength: 50),
                        ServiceName = c.String(maxLength: 100),
                        ServiceDescription = c.String(maxLength: 250),
                        AllowedDomains = c.String(nullable: false, maxLength: 1024),
                        CultureInfoCode = c.String(),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("auth.Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "auth.ApplicationUserAudits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(),
                        ActionTypeId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "common.Comments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EntityType = c.Int(nullable: false),
                        EntityId = c.Int(nullable: false),
                        Rating = c.Decimal(precision: 18, scale: 2),
                        CommentDetail = c.String(nullable: false, maxLength: 2048),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "common.Lookups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LookupType = c.Int(nullable: false),
                        Key = c.String(maxLength: 50),
                        Value = c.String(),
                        ParentId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "common.PageItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 500),
                        Description = c.String(maxLength: 2048),
                        FriendlyUrl = c.String(nullable: false, maxLength: 500),
                        ControllerName = c.String(nullable: false, maxLength: 50),
                        ActionName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "park.SlotBookings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BookedByUserId = c.Int(nullable: false),
                        VehicleInfo = c.String(),
                        VehicleImageFileName = c.String(),
                        BookedOn = c.DateTime(nullable: false),
                        SlotId = c.Int(nullable: false),
                        Duration = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("common.UserProfiles", t => t.BookedByUserId, cascadeDelete: true)
                .ForeignKey("park.Slots", t => t.SlotId)
                .Index(t => t.BookedByUserId)
                .Index(t => t.SlotId);
            
            CreateTable(
                "common.UserProfiles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(maxLength: 100),
                        PhoneNumber = c.String(maxLength: 50),
                        UserProfileType = c.Int(nullable: false),
                        BillingAddressId = c.Int(),
                        AvatarImageLocation = c.String(maxLength: 150),
                        AllowCaptureVehicleIn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("common.Addresses", t => t.BillingAddressId)
                .Index(t => t.BillingAddressId);
            
            CreateTable(
                "park.Slots",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 150),
                        SlotProviderId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("park.SlotProviders", t => t.SlotProviderId, cascadeDelete: true)
                .Index(t => t.SlotProviderId);
            
            CreateTable(
                "park.SlotPayments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SlotId = c.Int(nullable: false),
                        SlotBookingId = c.Int(nullable: false),
                        PaidOn = c.DateTime(nullable: false),
                        PayAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("park.Slots", t => t.SlotId)
                .ForeignKey("park.SlotBookings", t => t.SlotBookingId, cascadeDelete: true)
                .Index(t => t.SlotId)
                .Index(t => t.SlotBookingId);
            
            CreateTable(
                "park.SlotProviders",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SlotOwnerId = c.Int(nullable: false),
                        Name = c.String(maxLength: 250),
                        Description = c.String(maxLength: 1024),
                        MaximumSlots = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rating = c.Decimal(precision: 18, scale: 2),
                        AddressId = c.Int(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        ThumbnailImageLocation = c.String(maxLength: 150),
                        ImageLocation1 = c.String(maxLength: 150),
                        ImageLocation2 = c.String(maxLength: 150),
                        ImageLocation3 = c.String(maxLength: 150),
                        AvailableFromTime = c.DateTime(),
                        AvailableToTime = c.DateTime(),
                        IsAvailable = c.Boolean(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsOpen247 = c.Boolean(),
                        IsWeekendAvailable = c.Boolean(),
                        IsCoveredParking = c.Boolean(),
                        IsOvernightParking = c.Boolean(),
                        IsBusParking = c.Boolean(),
                        IsCarWashingServiceAvailable = c.Boolean(),
                        IsMondayAvailable = c.Boolean(),
                        MondayOpenTime = c.Int(),
                        MondayClosedTime = c.Int(),
                        IsTuesdayAvailable = c.Boolean(),
                        TuesdayOpenTime = c.Int(),
                        TuesdayClosedTime = c.Int(),
                        IsWednesdayAvailable = c.Boolean(),
                        WednesdayOpenTime = c.Int(),
                        WednesdayClosedTime = c.Int(),
                        IsThursdayAvailable = c.Boolean(),
                        ThursdayOpenTime = c.Int(),
                        ThursdayClosedTime = c.Int(),
                        IsFridayAvailable = c.Boolean(),
                        FridayOpenTime = c.Int(),
                        FridayClosedTime = c.Int(),
                        IsSaturdayAvailable = c.Boolean(),
                        SaturdayOpenTime = c.Int(),
                        SaturdayClosedTime = c.Int(),
                        IsSundayAvailable = c.Boolean(),
                        SundayOpenTime = c.Int(),
                        SundayClosedTime = c.Int(),
                        IsPublic = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("common.Addresses", t => t.AddressId, cascadeDelete: true)
                .ForeignKey("common.UserProfiles", t => t.SlotOwnerId, cascadeDelete: true)
                .Index(t => t.SlotOwnerId)
                .Index(t => t.Price)
                .Index(t => t.AddressId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("park.Slots", "SlotProviderId", "park.SlotProviders");
            DropForeignKey("park.SlotProviders", "SlotOwnerId", "common.UserProfiles");
            DropForeignKey("park.SlotProviders", "AddressId", "common.Addresses");
            DropForeignKey("park.SlotPayments", "SlotBookingId", "park.SlotBookings");
            DropForeignKey("park.SlotPayments", "SlotId", "park.Slots");
            DropForeignKey("park.SlotBookings", "SlotId", "park.Slots");
            DropForeignKey("park.SlotBookings", "BookedByUserId", "common.UserProfiles");
            DropForeignKey("common.UserProfiles", "BillingAddressId", "common.Addresses");
            DropForeignKey("auth.ApplicationServices", "ApplicationId", "auth.Applications");
            DropForeignKey("common.Addresses", "StateId", "common.States");
            DropForeignKey("common.Addresses", "DistrictId", "common.Districts");
            DropForeignKey("common.Districts", "CityId", "common.Cities");
            DropForeignKey("common.Addresses", "CountryId", "common.Countries");
            DropForeignKey("common.Addresses", "CityId", "common.Cities");
            DropForeignKey("common.Cities", "StateId", "common.States");
            DropForeignKey("common.States", "CountryId", "common.Countries");
            DropIndex("park.SlotProviders", new[] { "AddressId" });
            DropIndex("park.SlotProviders", new[] { "Price" });
            DropIndex("park.SlotProviders", new[] { "SlotOwnerId" });
            DropIndex("park.SlotPayments", new[] { "SlotBookingId" });
            DropIndex("park.SlotPayments", new[] { "SlotId" });
            DropIndex("park.Slots", new[] { "SlotProviderId" });
            DropIndex("common.UserProfiles", new[] { "BillingAddressId" });
            DropIndex("park.SlotBookings", new[] { "SlotId" });
            DropIndex("park.SlotBookings", new[] { "BookedByUserId" });
            DropIndex("auth.ApplicationServices", new[] { "ApplicationId" });
            DropIndex("common.Districts", new[] { "CityId" });
            DropIndex("common.States", new[] { "CountryId" });
            DropIndex("common.Cities", new[] { "StateId" });
            DropIndex("common.Addresses", new[] { "CountryId" });
            DropIndex("common.Addresses", new[] { "StateId" });
            DropIndex("common.Addresses", new[] { "CityId" });
            DropIndex("common.Addresses", new[] { "DistrictId" });
            DropTable("park.SlotProviders");
            DropTable("park.SlotPayments");
            DropTable("park.Slots");
            DropTable("common.UserProfiles");
            DropTable("park.SlotBookings");
            DropTable("common.PageItems");
            DropTable("common.Lookups");
            DropTable("common.Comments");
            DropTable("auth.ApplicationUserAudits");
            DropTable("auth.ApplicationServices");
            DropTable("auth.Applications");
            DropTable("common.Districts");
            DropTable("common.Countries");
            DropTable("common.States");
            DropTable("common.Cities");
            DropTable("common.Addresses");
        }
    }
}
