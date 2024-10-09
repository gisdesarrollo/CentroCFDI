using API.Catalogos;
using API.Enums;
using API.Integraciones.Clientes;
using API.Operaciones.Facturacion;
using API.Operaciones.OperacionesRecepcion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("DocumentosRecibidos")]
    public class DocumentoRecibido
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Tipo Documento Recibido")]
        public c_TipoDocumentoRecibido TipoDocumentoRecibido { get; set; }

        [DisplayName("Fecha de Entrega")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaEntrega { get; set; }

        [DisplayName("Fecha del Comprobante")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaComprobante { get; set; }

        public decimal Monto { get; set; }

        [DisplayName("Moneda")]
        public c_Moneda? MonedaId { get; set; }

        [DisplayName("Usuario")]
        public int? UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Validaciones")]
        public int? ValidacionesId { get; set; }
        [ForeignKey("ValidacionesId")]
        public virtual ValidacionesDR Validaciones { get; set; }

        [NotMapped]
        public List<ValidacionesDR> ValidacionesList { get; set; }

        [DisplayName("Validaciones Detalle")]
        public string ValidacionesDetalle { get; set; }

        [NotMapped]
        public List<string> DetalleArrays { get; set; }

        public int? SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        public int? CfdiRecibidosId { get; set; }
        [ForeignKey("CfdiRecibidosId")]
        public virtual RecibidosComprobanteDR RecibidosComprobante { get; set; }

        [DisplayName("Carga PDF")]
        public int? CfdiRecibidosPdfId { get; set; }
        [ForeignKey("CfdiRecibidosPdfId")]
        public virtual RecibidosPDFDR RecibidosPdf { get; set; }

        [DisplayName("Carga XML")]
        public int? CfdiRecibidosXmlId { get; set; }
        [ForeignKey("CfdiRecibidosXmlId")]
        public virtual RecibidosXMLDR RecibidosXml { get; set; }

        [DisplayName("Serie")]
        public string CfdiRecibidosSerie { get; set; }

        [DisplayName("Folio")]
        public string CfdiRecibidosFolio { get; set; }

        [DisplayName("UUID")]
        public string CfdiRecibidosUUID { get; set; }

        [DisplayName("Adjunto")]
        public int? AdjuntosId { get; set; }
        [ForeignKey("AdjuntosId")]
        public virtual AdjuntoDR Adjunto { get; set; }

        [DisplayName("Estado Comercial")]
        public c_EstadoComercial EstadoComercial { get; set; }

        [DisplayName("Estado Pago")]
        public c_EstadoPago EstadoPago { get; set; }

        public int? PagosId { get; set; }
        [ForeignKey("PagosId")]
        public virtual PagosDR Pago { get; set; }

        [DisplayName("Aprobaciones")]
        public int? AprobacionesId { get; set; }
        [ForeignKey("AprobacionesId")]
        public virtual Aprobaciones AprobacionesDR { get; set; }

        [NotMapped]
        public List<Aprobaciones> AprobacionesList { get; set; }

        public string Referencia { get; set; }

        [DisplayName("Documento Asociado")]
        public int? DocumentoAsociadoDRId { get; set; }
        [ForeignKey("DocumentoAsociadoDRId")]
        public virtual DocumentoAsociadoDR DocumentoAsociadoDR { get; set; }

        [DisplayName("Comprobación Gastos")]
        public int? ComprobacionGastoId { get; set; }
        [ForeignKey("ComprobacionGastoId")]
        public virtual ComprobacionGasto ComprobacionesGastos { get; set; }

        [DisplayName("Categoria de Gastos")]
        public int? CategoriaGastoId { get; set; }
        [ForeignKey("CategoriaGastoId")]
        public virtual CategoriaGasto CategoriaGasto { get; set; }
        /*public int? CI_Cofco_Ref_Id { get; set; }
        [ForeignKey("CI_Cofco_Ref_Id")]
        public virtual Custom_Cofco_FacturasRecibidas_Referencias CI_Cofco_FacturasRecibidas_Referencias { get; set; }
        */
        
        #region Not Mapped Properties

        [NotMapped]
        public HttpPostedFileBase Archivo { get; set; }

        [NotMapped]
        public HttpPostedFileBase ArchivoComprobanteNoFiscal { get; set; }
        
        [NotMapped]
        public HttpPostedFileBase ArchivoComprobanteExtranjero { get; set; }
        
        [NotMapped]
        public HttpPostedFileBase ArchivoAdjuntos { get; set; }
        
        [NotMapped]
        public HttpPostedFileBase ArchivoComprobanteCfdiXml { get; set; }
        
        [NotMapped]
        public HttpPostedFileBase ArchivoComprobanteCfdiPdf { get; set; }

        [NotMapped]
        public string PathArchivoXml { get; set; }

        [NotMapped]
        public string PathArchivoPdf { get; set; }

        [NotMapped]
        public string PathNoFiscal { get; set; }

        [NotMapped] 
        public string PathExtranjero { get; set; }

        [NotMapped] 
        public string PathAdjunto { get; set; }

        [NotMapped]
        public bool Procesado { get; set; }

        [NotMapped]
        public string VerificarEmail { get; set; }

        [NotMapped]
        public bool IsProveedor { get; set; }

        [NotMapped]
        public bool Previsualizacion { get; set; }

        [NotMapped]
        public int IdUsuarioSolicitante { get; set; }

        [NotMapped]
        public HttpPostedFileBase AdjuntoDR { get; set; }
        #endregion
    }
}
