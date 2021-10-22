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

        [StringLength(8)]
        [RegularExpression(@"/[D][E]\d{6}$")]
        [DisplayName("Id Destino")]
        public String IdDestino { get; set; }
        public int Cliente_Id { get; set; }
        [ForeignKey("Cliente_Id")]
        public virtual Cliente Cliente { get; set; }
        
        [NotMapped]
        [DisplayName("Nombre del Destinatario")]
        public string NombreDestinatario { get; set; }
        [NotMapped]
        [DisplayName("Residencia Fiscal")]
        public c_Pais ResidenciaFiscal { get; set; }
        [NotMapped]
        public string NumRegIdTrib { get; set; }
        [NotMapped]
        [DisplayName("RFC Destinatario")]
        public string RFCDestinatario { get; set; }

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

        [DisplayName("Fecha y Hora Programada de Llegada")]
        public DateTime FechaHoraProgLlegada { get; set; }

        public int Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }
    }
}
