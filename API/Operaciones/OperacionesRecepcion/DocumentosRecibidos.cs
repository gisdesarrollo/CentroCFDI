using API.Catalogos;
using API.Enums;
using API.Operaciones.Facturacion;
using API.Operaciones.OperacionesRecepcion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("DocumentosRecibidos")]
    public class DocumentosRecibidos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Tipo Documento Recibido")]
        public c_TipoDocumentoRecibido TipoDocumentoRecibido { get; set; }

        [DisplayName("Fecha Entrega")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaEntrega { get; set; }

        [DisplayName("Fecha Comprobante")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaComprobante { get; set; }

        public Decimal Monto { get; set; }

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
        public String ValidacionesDetalle { get; set; }

        [NotMapped]
        public List<String> DetalleArrays { get; set; }

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
        public String CfdiRecibidosSerie { get; set; }

        [DisplayName("Folio")]
        public String CfdiRecibidosFolio { get; set; }

        [DisplayName("UUID")]
        public String CfdiRecibidosUUID { get; set; }

        public int? AdjuntosId { get; set; }

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

        public String Referencia { get; set; }
        
        public String OrdenDeCompra { get; set; }

        [DisplayName("Comprobación Gastos")]
        public int? ComprobacionGastoId { get; set; }
        [ForeignKey("ComprobacionGastoId")]
        public virtual ComprobacionGasto ComprobacionesGastos { get; set; }

        //NotMapped
        [DisplayName("Archivo")]
        [NotMapped]
        public HttpPostedFileBase Archivo { get; set; }

        [NotMapped]
        public String PathArchivoXml { get; set; }
        
        [NotMapped]
        public string PathArchivoPdf { get; set; }
        
        [NotMapped]
        public bool Procesado { get; set; }
        
        [NotMapped]
        public string VerificarEmail { get; set; }

        [NotMapped]
        public bool isProveedor { get; set; }

        [NotMapped]
        public bool Previsualizacion { get; set; }

        
    }
}