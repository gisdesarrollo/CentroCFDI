using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    
    [Table("c_ClaveProdSTCC")]
    public class ClaveProdSTCC
    {
        [Key]
        [StringLength(8)]
        public String ClaveSTCC { get; set; }
        public String Descripcion { get; set; }
    }
}
