namespace APBox.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class FKNotes : DbMigration
    {
        public override void Up()
        {
            //AddColumn("ORI_FACTURASEMITIDAS", "TotalImpuestosRetenidos", c => c.Double(nullable: false));
            //AddColumn("ORI_FACTURASEMITIDAS", "TotalImpuestosTrasladados", c => c.Double(nullable: false));
            AddColumn("ORI_FACTURASRECIBIDAS", "TotalImpuestosRetenidos", c => c.Double(nullable: false));
            AddColumn("ORI_FACTURASRECIBIDAS", "TotalImpuestosTrasladados", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ORI_FACTURASRECIBIDAS", "TotalImpuestosTrasladados");
            DropColumn("ORI_FACTURASRECIBIDAS", "TotalImpuestosRetenidos");
            //DropColumn("ORI_FACTURASEMITIDAS", "TotalImpuestosTrasladados");
            //DropColumn("ORI_FACTURASEMITIDAS", "TotalImpuestosRetenidos");
        }
    }
}
