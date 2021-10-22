using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_TipoRelacion")]
    public class TipoRelacion
    {
        [Key]
        [StringLength(12)]
        public String c_TipoRelacion { get; set; }
        public String Descripcion { get; set; }
    }
}
