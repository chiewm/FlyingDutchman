namespace FlyingDutchman.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreatedOn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "CreatedOn", c => c.DateTime(defaultValueSql: "GETDATE()"));
        }
        
        public override void Down()
        {
        }
    }
}
