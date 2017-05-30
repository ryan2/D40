namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.D40",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        Record_ID = c.Int(nullable: false),
                        Asset_Tag = c.String(),
                        Asset_status = c.String(),
                        Serial_Number = c.String(),
                        BB_Phone = c.Int(nullable: false),
                        Refresh_Date = c.DateTime(nullable: false),
                        Model = c.String(),
                        Seat_Type = c.String(),
                        Service_Level = c.String(),
                        HHS_Billing = c.String(),
                        OpDiv = c.String(),
                        StaffDiv = c.String(),
                        Office = c.String(),
                        Last_Name = c.String(),
                        First_Name = c.String(),
                        Site_Address = c.String(),
                        Floor = c.String(),
                        Room = c.String(),
                        Lumension_Report_Date = c.DateTime(nullable: false),
                        Lumension_Computer_Name = c.String(),
                        Lumension_Login_User = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.D40");
        }
    }
}
