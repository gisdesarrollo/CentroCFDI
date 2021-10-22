namespace APBox.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartaporte : DbMigration
    {
        public override void Up()
        {
            AlterColumn("cp_ComplementoCartaPorte", "TranspInternac", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("cp_ComplementoCartaPorte", "TranspInternac", c => c.String(unicode: false));
        }
    }
}
