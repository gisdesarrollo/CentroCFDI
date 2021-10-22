using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_DerechosDePaso")]
    public class DerechosDePaso
    {
        [Key]
        [StringLength(6)]
        public String ClavederechoPaso { get; set; }
        public String DerechoDePaso { get; set; }
        public String Entre { get; set; }
        public String Hasta { get; set; }
        public String OtorgaRecibe { get; set; }
        public String Concesionario { get; set; }
    }
}
