using API.CatalogosCartaPorte;
using API.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CFDI.API.Enums.CFDI33;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_UbicacionOrigen")]
    public class UbicacionOrigen
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(8)]
        [RegularExpression(@"/[O][R]\d{6}$")]
        [DisplayName("Id Origen")]
        public String IdOrigen { get; set; }
        
        public int Sucursal_Id { get; set; }
        [ForeignKey("Sucursal_Id")]
        public virtual Sucursal Sucursal{ get; set; }

        [NotMapped]
        [DisplayName("Nombre del Remitente")]
        public string NombreRemitente { get; set; }
        
        [NotMapped]
        [DisplayName("Residencia Fiscal")]
        public c_Pais ResidenciaFiscal { get; set; }
        
        [NotMapped]
        public string NumRegIdTrib { get; set; }
        
        [NotMapped]
        [DisplayName("RFC del Remitente")]
        public string RFCRemitente { get; set; }

        public string Estaciones_Id { get; set; }
        [ForeignKey("Estaciones_Id")]
        public virtual Estaciones Estaciones{ get; set; }
        
        [NotMapped]
        [DisplayName("Nombre de Estación")]
        public string NombreEstacion { get; set; }
        [NotMapped]
        [DisplayName("Número de Estación")]
        public string NumEstacion { get; set; }

        public string NavegacionTrafico { get; set; }

        [DisplayName("Fecha y Hora de Salida")]
        [DataType(DataType.DateTime)]
        public DateTime FechaHoraSalida { get; set; }

        public int Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }
    }
}
