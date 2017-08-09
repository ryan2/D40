namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class priceFY : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "FY", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prices", "FY");
        }
    }
}
