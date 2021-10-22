using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_CodigoTransporteAereo")]
    public class CodigoTransporteAereo
    {
        [Key]
        [StringLength(5)]
        public String ClaveIdentificacion { get; set; }
        public String Nacionalidad { get; set; }
        public String NombreAreolinea { get; set; }
        public String Designador { get; set; }

    }
}
