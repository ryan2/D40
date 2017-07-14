namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.D40",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Record_ID = c.Int(),
                        Asset_Tag = c.String(nullable: false),
                        Asset_status = c.String(),
                        Serial_Number = c.String(),
                        BB_Phone = c.String(),
                        Refresh_Date = c.DateTime(),
                        Model = c.String(),
                        Seat_Type = c.String(),
                        Service_Level = c.String(),
                        HHS_Billing = c.String(),
                        OpDiv = c.String(),
                        StaffDiv = c.String(),
                        Office = c.String(),
                        Last_Name = c.String(nullable: false),
                        First_Name = c.String(nullable: false),
                        Site_Address = c.String(),
                        Floor = c.String(),
                        Room = c.String(),
                        Lumension_Report_Date = c.DateTime(),
                        Lumension_Computer_Name = c.String(),
                        Lumension_Login_User = c.String(),
                        Received_Date = c.DateTime(nullable: false),
                        Returned_Date = c.DateTime(),
                        NameID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Names", t => t.NameID, cascadeDelete: true)
                .Index(t => t.NameID);
            
            CreateTable(
                "dbo.Names",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Last_Name = c.String(),
                        First_Name = c.String(),
                        Active = c.Boolean(nullable: false),
                        Office = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SoftwareNames",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NameID = c.Int(nullable: false),
                        SoftwareID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Softwares", t => t.SoftwareID, cascadeDelete: true)
                .ForeignKey("dbo.Names", t => t.NameID, cascadeDelete: true)
                .Index(t => t.NameID)
                .Index(t => t.SoftwareID);
            
            CreateTable(
                "dbo.Softwares",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        license = c.String(nullable: false),
                        num = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Ticket_Num = c.String(nullable: false),
                        Open_Date = c.DateTime(nullable: false),
                        Closed_Date = c.DateTime(),
                        Description = c.String(nullable: false),
                        NameID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Names", t => t.NameID, cascadeDelete: true)
                .Index(t => t.NameID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "NameID", "dbo.Names");
            DropForeignKey("dbo.SoftwareNames", "NameID", "dbo.Names");
            DropForeignKey("dbo.SoftwareNames", "SoftwareID", "dbo.Softwares");
            DropForeignKey("dbo.D40", "NameID", "dbo.Names");
            DropIndex("dbo.Tickets", new[] { "NameID" });
            DropIndex("dbo.SoftwareNames", new[] { "SoftwareID" });
            DropIndex("dbo.SoftwareNames", new[] { "NameID" });
            DropIndex("dbo.D40", new[] { "NameID" });
            DropTable("dbo.Tickets");
            DropTable("dbo.Softwares");
            DropTable("dbo.SoftwareNames");
            DropTable("dbo.Names");
            DropTable("dbo.D40");
        }
    }
}
