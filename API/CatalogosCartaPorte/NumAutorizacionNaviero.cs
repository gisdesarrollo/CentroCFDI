using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_NumAutorizacionNaviero")]
    public class NumAutorizacionNaviero
    {
        [Key]
        [StringLength(16)]
        public String NumeroAutorizacion { get; set; }
    }
}
