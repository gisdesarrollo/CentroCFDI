using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using API.Catalogos;
using API.Enums;
using API.Operaciones.Facturacion;
using CFDI.API.Enums.CFDI33;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_ComplementoCartaPorte")]
    public class ComplementoCartaPorte
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Transporte Internacional")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public Boolean TranspInternac { get; set; }


        [DisplayName("Salida o Entrada de Mercancia")]
        public Boolean EntradaSalidaMerc { get; set; }

        [DisplayName("Total de Distancia Recorrida")]
        public Decimal TotalDistRec { get; set; }

        [DisplayName("Pais de origen destino")]
        public c_Pais PaisOrigendestino { get; set; }

        [Required(ErrorMessage ="Campo Obligatorio")]
        public string Version { get; set; }

        

        [DisplayName("Clave Transporte")]
        public string ClaveTransporteId { get; set; }
        [ForeignKey("ClaveTransporteId")]
        public virtual CveTransporte ViaEntradaSalida { get; set; }
        [NotMapped]
        [DisplayName("Vía de Entrada o Salida")]
        public String ClaveTransporte { get; set; }

        [NotMapped]
        [Required]
        [DisplayName("Tipo de Comprobante")]
        public c_TipoDeComprobante TipoDeComprobante { get; set; }
        [NotMapped]
        public c_Moneda? Moneda { get; set; }
        [NotMapped]
        public Decimal Subtotal { get; set; }
        [NotMapped]
        public Decimal Total { get; set; }

        [NotMapped]
        public virtual Ubicacion Ubicacion { get; set; }
        [NotMapped]
        public virtual List<Ubicacion> Ubicaciones { get; set; }
        public int Mercancias_Id { get; set; }
        [ForeignKey("Mercancias_Id")]
        public virtual Mercancias Mercancias { get; set; }

        [NotMapped]
        public virtual TiposFigura TiposFigura { get; set; }

        [NotMapped]
        public virtual List<TiposFigura> FiguraTransporte { get; set; }

        #region Campos CFDI

        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        //Filtros
        [DisplayName("Receptor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ReceptorId { get; set; }
        [ForeignKey("ReceptorId")]
        public virtual Cliente Receptor { get; set; }

        [DisplayName("Fecha del Documento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaDocumento { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public Meses Mes { get; set; }

        public bool Generado { get; set; }

        public Status Status { get; set; }

        [DisplayName("Factura Emitida")]
        public int? FacturaEmitidaId { get; set; }
        [ForeignKey("FacturaEmitidaId")]
        public virtual FacturaEmitida FacturaEmitida { get; set; }

        [NotMapped]
        public bool hidden { get; set; }

        [NotMapped]
        public bool Seleccionado { get; set; }
        #endregion


        #region Cfdis Relacionados

        [DisplayName("Tipo de Relación")]
        public c_TipoRelacion? TipoRelacion { get; set; }

        [DisplayName("CFDI Relacionado")]
        public int? CfdiRelacionadoId { get; set; }
        [ForeignKey("CfdiRelacionadoId")]
        public virtual FacturaEmitida CfdiRelacionado { get; set; }

        #endregion

    }
}
