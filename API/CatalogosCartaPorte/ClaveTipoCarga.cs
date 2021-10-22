using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_ClaveTipoCarga")]
    public class ClaveTipoCarga
    {
        [Key]
        [StringLength(3)]
        public String ClaveTipocarga { get; set; }
        public String Descripcion { get; set; }
    }
}
