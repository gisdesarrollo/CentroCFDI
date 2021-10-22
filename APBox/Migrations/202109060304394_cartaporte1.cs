namespace APBox.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartaporte1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("cp_ComplementoCartaPorte", "EntradaSalidaMerc");
        }
        
        public override void Down()
        {
            AddColumn("cp_ComplementoCartaPorte", "EntradaSalidaMerc", c => c.Boolean(nullable: false));
        }
    }
}
