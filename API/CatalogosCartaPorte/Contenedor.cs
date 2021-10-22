using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_Contenedor")]
    public class Contenedor
    {
        [Key]
        [StringLength(4)]
        public String Clave { get; set; }
        public String TipoContenedor { get; set; }
        public String Descripcion { get; set; }
    }
}
