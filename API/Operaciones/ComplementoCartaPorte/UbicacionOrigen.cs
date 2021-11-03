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
        [DisplayName("Tipo de Ubicación")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string TipoUbicacion { get; set; }
        [StringLength(8)]
        [RegularExpression(@"/[O][R]\d{6}$")]
        [DisplayName("Id Ubicacion Origen")]
        public string IDUbicacionOrigen { get; set; }

        public int Sucursal_Id { get; set; }
        [ForeignKey("Sucursal_Id")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("RFC Remitente Origen")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string RfcRemitenteDestinatario { get; set; }

        [DisplayName("Nombre Remitente Origen")]
        public string NombreRemitenteDestinatario { get; set; }
        [DisplayName("Número de identificación")]
        public string NumRegIdTrib { get; set; }
        [DisplayName("Residencia Fiscal")]
        public c_Pais ResidenciaFiscal { get; set; }
        public string Estaciones_Id { get; set; }
        [ForeignKey("Estaciones_Id")]
        public virtual Estaciones Estaciones { get; set; }
        [NotMapped]
        [DisplayName("Numero de Estación")]
        public String NumEstacion { get; set; }
        [DisplayName("Nombre Estacion Origen")]
        public string NombreEstacion { get; set; }
        [DisplayName("Navegacion Trafico")]
        public string NavegacionTrafico { get; set; }
        [DisplayName("Fecha y Hora Salida de Origen")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public DateTime FechaHoraSalida { get; set; }
        public String TipoEstacion_Id { get; set; }
        [ForeignKey("TipoEstacion_Id")]
        public virtual TipoEstacion TipoEstacion { get; set; }

        [NotMapped]
        [DisplayName("Tipo de Estación")]
        public String TipoEstaciones { get; set; }
        [DisplayName("Distancia Recorrida")]
        public Decimal DistanciaRecorrida { get; set; }

        public int Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }

    }
}
