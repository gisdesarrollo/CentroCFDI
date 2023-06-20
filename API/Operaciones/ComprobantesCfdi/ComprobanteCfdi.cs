using API.Catalogos;
using API.CatalogosCartaPorte;
using API.Enums;
using API.Operaciones.ComplementoCartaPorte;
using API.Operaciones.Facturacion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace API.Operaciones.ComprobantesCfdi
{
    [Table("ori_comprobantecfdi")]
    public class ComprobanteCfdi
    {
        //[DisplayName("Folio")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Versión del Complemento")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Version { get; set; }

        [DisplayName("Forma de Pago")]
        public String FormaPago { get; set; }
        [ForeignKeyAttribute("FormaPago")]
        public virtual FormaPagos FormaPagos { get; set; }

        [NotMapped]
        public String FormaPagoId { get; set; }

        [NotMapped]
        public c_TipoDeComprobante TipoComprobanteId { get; set; }

        [DisplayName("Método de Pago")]
        public c_MetodoPago? MetodoPago { get; set; }

        [DisplayName("Tipo Cambio")]
        public string TipoCambio { get; set; }

        [DisplayName("Condiciones Pago")]
        public string CondicionesPago { get; set; }

        public string Descuento { get; set; }

        public Decimal Subtotal { get; set; }

        public Decimal Total { get; set; }

        public c_Moneda? Moneda { get; set; }

        public Meses? Mes { get; set; }

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

        public Decimal TotalImpuestoTrasladado { get; set; }

        public Decimal TotalImpuestoRetenidos { get; set; }

        [Required]
        [DisplayName("Tipo de Comprobante")]
        public c_TipoDeComprobante TipoDeComprobante { get; set; }


        public bool Generado { get; set; }

        public Status Status { get; set; }

        [DisplayName("Factura Emitida")]
        public int? FacturaEmitidaId { get; set; }
        [ForeignKey("FacturaEmitidaId")]
        public virtual FacturaEmitida FacturaEmitida { get; set; }

        [DisplayName("Exportacion")]
        public string ExportacionId { get; set; }

        [NotMapped]
        public string FolioSustitucion { get; set; }

        [NotMapped]
        public string MotivoCancelacion { get; set; }

        #region Cfdis Relacionados

        [DisplayName("Tipo de Relación")]
        public String TipoRelacion { get; set; }

        [NotMapped]
        public string IdTipoRelacion { get; set; }

        [DisplayName("UUID De CFDI Relacionado")]
        public String UUIDCfdiRelacionado { get; set; }

        //public int? CfdiRelacionadoId { get; set; }
        //[ForeignKey("CfdiRelacionadoId")]
        //public virtual FacturaEmitida CfdiRelacionado { get; set; }

        #endregion

        [NotMapped]
        public Conceptos Conceptos { get; set; }

        public virtual List<Conceptos> Conceptoss { get; set; }

       
    }
}
