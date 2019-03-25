namespace AgentHub.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgentHub : DbMigration
    {
        public override void Up()
        {
            AddColumn("common.PageItems", "Keywords", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("common.PageItems", "Keywords");
        }
    }
}
