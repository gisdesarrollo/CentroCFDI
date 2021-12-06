using API.CatalogosCartaPorte;
using CFDI.API.Enums.CFDI33;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_Conceptos")]
    public class Conceptos
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Clave de producto o servicio")]
        public string ClavesProdServ { get; set; }

        [DisplayName("Clave de producto o servicio")]
        public string ClaveProdServ_Id { get; set; }
        [ForeignKey("ClaveProdServ_Id")]
        public virtual ClaveProdServCP ClaveProdServCP { get; set; }

        [DisplayName("Clave de unidad")]
        public string ClavesUnidad { get; set; }
        [DisplayName("Clave de unidad")]
        public string ClaveUnidad_Id { get; set; }
        [ForeignKey("ClaveUnidad_Id")]
        public virtual ClaveUnidad ClaveUnidad { get; set; }  
        
        public string Descripcion { get; set; }
        
        [NotMapped]
        public virtual SubImpuestoC Traslado { get; set; }

        [NotMapped]
        public virtual SubImpuestoC Retencion { get; set; }

    }
}
