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
    [Table("cp_Ubicacion")]
    public class Ubicacion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Distancia Recorrida")]
        public Decimal DistanciaRecorrida { get; set; }

        public String TipoEstacion_Id { get; set; }
        [ForeignKey("TipoEstacion_Id")]
        public virtual TipoEstacion TipoEstacion { get; set; }

        [NotMapped]
        [DisplayName("Tipo de Estación")]
        public String TipoEstaciones { get; set; }

        [NotMapped]
        public virtual UbicacionDestino UbicacionDestino { get; set; }

        [NotMapped]
        public virtual UbicacionOrigen UbicacionOrigen { get; set; }
    }
}
