using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_ContenedorMaritimo")]
    public class ContenedorMaritimo
    {
        [Key]
        [StringLength(12)]
        public String ClaveContenedorMaritimo { get; set; }
        public String Descripcion { get; set; }
    }
}
