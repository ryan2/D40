namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketassetfkey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "D40ID", c => c.Int());
            CreateIndex("dbo.Tickets", "D40ID");
            AddForeignKey("dbo.Tickets", "D40ID", "dbo.D40", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "D40ID", "dbo.D40");
            DropIndex("dbo.Tickets", new[] { "D40ID" });
            DropColumn("dbo.Tickets", "D40ID");
        }
    }
}
