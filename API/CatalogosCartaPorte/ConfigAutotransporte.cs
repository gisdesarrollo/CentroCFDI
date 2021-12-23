using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_ConfigAutotransporte")]
    public class ConfigAutotransporte
    {
        [Key]
        [StringLength(12)]
        public String c_ClaveNomeclatura { get; set; }
        public String Descripcion { get; set; }
        public String NumeroEjes { get; set; }
        public String NumeroLlantas { get; set; }

        public String Remolque { get; set; }

    }
}
