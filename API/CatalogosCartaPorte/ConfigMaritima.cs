using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_ConfigMaritima")]
    public class ConfigMaritima
    {
        [Key]
        [StringLength(3)]
        public String c_ClaveUnidad { get; set; }
        public String Descripcion { get; set; }

    }
}
