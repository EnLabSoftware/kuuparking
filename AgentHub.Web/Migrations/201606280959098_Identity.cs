namespace AgentHub.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Identity : DbMigration
    {
        public override void Up()
        {
            AddColumn("auth.ApplicationUsers", "IsAdministrator", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("auth.ApplicationUsers", "IsAdministrator");
        }
    }
}
