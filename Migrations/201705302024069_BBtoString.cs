namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BBtoString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.D40", "BB_Phone", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.D40", "BB_Phone", c => c.Int());
        }
    }
}
