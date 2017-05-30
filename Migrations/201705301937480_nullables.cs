namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.D40", "BB_Phone", c => c.Int());
            AlterColumn("dbo.D40", "Lumension_Report_Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.D40", "Lumension_Report_Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.D40", "BB_Phone", c => c.Int(nullable: false));
        }
    }
}
