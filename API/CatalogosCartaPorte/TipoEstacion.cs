using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_TipoEstacion")]
    public class TipoEstacion
    {
        [Key]
        [StringLength(2)]
        public String ClaveEstacion { get; set; }
        public String Descripcion { get; set; }
        public String ClaveTransporte { get; set; }
    }
}
