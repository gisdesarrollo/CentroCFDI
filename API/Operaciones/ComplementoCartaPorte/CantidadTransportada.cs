using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_CantidadTransportada")]
    public class CantidadTransportada
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Decimal Cantidad { get; set; }

        [DisplayName("Claves Transporte")]
        public string CveTransporte_Id{ get; set; }
        [ForeignKey("CveTransporte_Id")]
        public virtual CveTransporte CveTransporte { get; set; }
        
     
        [DisplayName("ID Destino")]
        public String IDDestino { get; set; }

        
        [DisplayName("ID Origen")]
        public String IDOrigen { get; set; }

        public int? Mercancia_Id { get; set; }
        [ForeignKey("Mercancia_Id")]
        public virtual Mercancia Mercancia { get; set; }
    }
}
