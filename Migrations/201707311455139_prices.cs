namespace D40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prices : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        C_G = c.Decimal(nullable: false, precision: 18, scale: 2),
                        C_S = c.Decimal(nullable: false, precision: 18, scale: 2),
                        C_P = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ph_G = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ph_S = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ph_P = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pr_S = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pr_P = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pr_G = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ps_S = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ps_P = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ps_G = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Prices");
        }
    }
}
