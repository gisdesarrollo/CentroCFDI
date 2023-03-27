using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_cvetransporte")]
    public class CveTransporte
    {
        [Key]
        [StringLength(2)]
        public String c_ClaveUnidad { get; set; }
        public String Descripcion { get; set; }

    }
}
