namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newDBFKeyTest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Names", "Last_Name", c => c.String());
            AddColumn("dbo.Names", "First_Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Names", "First_Name");
            DropColumn("dbo.Names", "Last_Name");
        }
    }
}
