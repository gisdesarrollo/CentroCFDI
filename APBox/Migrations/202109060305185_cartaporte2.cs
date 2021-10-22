namespace APBox.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartaporte2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("cp_ComplementoCartaPorte", "EntradaSalidaMerc", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("cp_ComplementoCartaPorte", "EntradaSalidaMerc");
        }
    }
}
