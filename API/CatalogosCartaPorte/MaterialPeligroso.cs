using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_MaterialPeligroso")]
    public class MaterialPeligroso
    {
        [Key]
        [StringLength(5)]
        public String ClaveMaterialPeligroso { get; set; }
        public String Descripcion { get; set; }
        public String ClaseDiv  { get; set; }
        public String PeligroSecundario { get; set; }
        public String NombreTecnico { get; set; }
        //public String GrupoEmbEnvOnu  { get; set; }
        //public String DispEspec { get; set; }
        //public String CantidadesLimitadas   { get; set; }
        //public String CantidadesExceptuadas{ get; set; }
        //public String EmbEnvInst { get; set; }
        //public String EmbEnvDispEspec { get; set; }
        //public String CisternasContenedoresInstTrasnp { get; set; }
        //public String CisternasContenedoresDispEspec{ get; set; }
}
}
