namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class produpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Disputes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Change_Description = c.String(),
                        Change_Reason = c.String(),
                        Change_Field_To = c.String(),
                        Date_Call_Change = c.DateTime(),
                        Comments = c.String(),
                        LM_Response = c.String(),
                        LM_Rationale = c.String(),
                        Remove_Asset = c.String(),
                        Date = c.DateTime(nullable: false),
                        Asset_Tag = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Tickets", "DisputesID", c => c.Int());
            CreateIndex("dbo.Tickets", "DisputesID");
            AddForeignKey("dbo.Tickets", "DisputesID", "dbo.Disputes", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "DisputesID", "dbo.Disputes");
            DropIndex("dbo.Tickets", new[] { "DisputesID" });
            DropColumn("dbo.Tickets", "DisputesID");
            DropTable("dbo.Disputes");
        }
    }
}
