namespace APBox.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartaporte3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("cp_ComplementoCartaPorte", "TranspInternac", c => c.Boolean(nullable: false));
            AlterColumn("cp_ComplementoCartaPorte", "EntradaSalidaMerc", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("cp_ComplementoCartaPorte", "EntradaSalidaMerc", c => c.Int(nullable: false));
            AlterColumn("cp_ComplementoCartaPorte", "TranspInternac", c => c.Int(nullable: false));
        }
    }
}
