using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using CFDI.API.Enums.CFDI33;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_Ubicaciones")]
    public class Ubicacion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UbicacionD_Id { get; set; }
        [ForeignKey("UbicacionD_Id")]
        public virtual UbicacionDestino UbicacionDestino { get; set; }
        public int UbicacionO_Id { get; set; }
        [ForeignKey("UbicacionO_Id")]
        public virtual UbicacionOrigen UbicacionOrigen { get; set; }

        [NotMapped]
        [DisplayName("Nombre Remitente Destino")]
        public Decimal DistanciaRecorridas { get; set; }

    }
}
