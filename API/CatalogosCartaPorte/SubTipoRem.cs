using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_SubTipoRem")]
    public class SubTipoRem
    {
        [Key]
        [StringLength(6)]
        public String ClaveTipoRemolque { get; set; }
        public String Remolque { get; set; }
    }
}
