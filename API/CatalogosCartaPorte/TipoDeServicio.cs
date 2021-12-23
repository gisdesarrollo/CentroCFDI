using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_TipoDeServicio")]
    public class TipoDeServicio
    {
        [Key]
        [StringLength(4)]
        public String c_ClaveUnidad { get; set; }
        public String Descripcion { get; set; }

        public String Contenedor { get; set; }

    }
}