using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_Estaciones")]
    public class Estaciones
    {
        [Key]
        [StringLength(10)]
        public String ClaveIdentificacion { get; set; }

        public String ClaveTransporte_Id { get; set; }
        [ForeignKey("ClaveTransporte_Id")]
        public virtual CveTransporte CveTransporte { get; set; }

        [NotMapped]
        public String ClaveTransporte { get; set; }

        public String Descripcion { get; set; }
        public String Nacionalidad { get; set; }

        public String DesignadorIATA { get; set; }

        public String LineaFerrea { get; set; }



    }
}
