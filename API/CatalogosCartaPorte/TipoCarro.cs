using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_TipoCarro")]
    public class TipoCarro
    {
        [Key]
        [StringLength(4)]
        public String Clave { get; set; }
        public String TipoDeCarro { get; set; }
        public String Descripcion { get; set; }
    }
}
