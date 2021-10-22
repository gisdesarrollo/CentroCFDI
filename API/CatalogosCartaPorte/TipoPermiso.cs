using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_TipoPermiso")]
    public class TipoPermiso
    {
        [Key]
        [StringLength(6)]
        public String Clave { get; set; }
        public String Descripcion { get; set; }
        public String Nota { get; set; }
    }
}
