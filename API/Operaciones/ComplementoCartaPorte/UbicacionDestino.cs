using API.CatalogosCartaPorte;
using API.Catalogos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using CFDI.API.Enums.CFDI33;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_UbicacionDestino")]
    public class UbicacionDestino
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Tipo de Ubicación")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string TipoUbicacion { get; set; }
        [StringLength(8)]
        [RegularExpression(@"/[D][E]\d{6}$")]
        [DisplayName("Id Ubicacion Origen")]
        public string IDUbicacionDestino { get; set; }

        [DisplayName("RFC Remitente Destino")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string RfcDestinatario { get; set; }

        [DisplayName("Nombre Remitente Destino")]
        public string NombreDestinatario { get; set; }
        [DisplayName("Número de identificación")]
        public string NumRegIdTrib { get; set; }
        [DisplayName("Residencia Fiscal")]
        public c_Pais ResidenciaFiscal { get; set; }
        public string Estaciones_Id{ get; set; }
        [ForeignKey("Estaciones_Id")]
        public virtual Estaciones Estaciones { get; set; }
        [NotMapped]
        [DisplayName("Numero de Estación Destino")]
        public String NumEstacion { get; set; }
        [DisplayName("Nombre Estacion")]
        public string NombreEstacion { get; set; }
        [DisplayName("Navegacion Trafico")]
        public string NavegacionTrafico { get; set; }
        [DisplayName("Fecha y Hora Salida de Destino ")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public DateTime FechaHoraLlegada { get; set; }

        [DisplayName("Tipo de Estación")]
        public String TipoEstacion_Id { get; set; }
        [ForeignKey("TipoEstacion_Id")]
        public virtual TipoEstacion TipoEstacion { get; set; }

        /*[NotMapped]
        [DisplayName("Tipo de Estación")]
        public String TipoEstaciones { get; set; }*/

        [DisplayName("Distancia Recorrida")]
        public Decimal DistanciaRecorrida { get; set; }

        public int Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }
    }
}
