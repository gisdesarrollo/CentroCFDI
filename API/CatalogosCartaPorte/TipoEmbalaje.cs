using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_TipoEmbalaje")]
    public class TipoEmbalaje
    {
        [Key]
        [StringLength(4)]
        public String ClaveAsignacion { get; set; }
        public String Descripcion { get; set; }
    }
}
