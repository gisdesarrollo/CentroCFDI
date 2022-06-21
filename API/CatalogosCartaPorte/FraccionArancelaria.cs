using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_FraccionArancelaria")]
    public class FraccionArancelaria
    {
        [Key]
        [StringLength(10)]
        public String c_FraccionArancelaria { get; set; }
        public String Descripcion { get; set; }

        public String UMT { get; set; }
    }
}
