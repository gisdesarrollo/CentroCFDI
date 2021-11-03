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
        [Required(ErrorMessage = "Campo Obligatorio")]
        public Decimal Cantidad { get; set; }

        [DisplayName("Claves Transporte")]
        public string CveTransporte_Id{ get; set; }
        [ForeignKey("CveTransporte_Id")]
        public virtual CveTransporte CveTransporte { get; set; }
        /*[NotMapped]
        public String ClaveTransporte { get; set; }
        */
        [DisplayName("ID Destino")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int UbicacionDestino_Id { get; set; }
        [ForeignKey("UbicacionDestino_Id")]
        public virtual UbicacionDestino UbicacionDestino { get; set; }
        /*[NotMapped]
        [DisplayName("ID Destino")]
        public String IDDestino { get; set; }*/

        [DisplayName("ID Origen")]
        [Required(ErrorMessage ="Campo Obligatorio")]
        public int UbicacionOrigen_Id { get; set; }
        [ForeignKey("UbicacionOrigen_Id")]
        public virtual UbicacionOrigen UbicacionOrigen { get; set; }
        /*[NotMapped]
        [DisplayName("ID Origen")]
        public String IDOrigen { get; set; }*/

    }
}
