using API.Catalogos;
using API.Enums;
using API.Operaciones.Facturacion;
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
    public class DocumentosRecibidosDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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
        public c_Moneda? Moneda_Id { get; set; }

        [DisplayName("Usuario")]
        public int? Usuario_Id { get; set; }
        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Validaciones")]
        public int? Validaciones_Id { get; set; }
        [ForeignKey("Validaciones_Id")]
        public virtual ValidacionesDR Validaciones { get; set; }

        [NotMapped]
        public List<ValidacionesDR> ValidacionesList { get; set; }

        [DisplayName("Validaciones Detalle")]
        public String Validaciones_Detalle { get; set; }

        [NotMapped]
        public List<String> DetalleArrays { get; set; }

        public int? SocioComercial_Id { get; set; }
        [ForeignKey("SocioComercial_Id")]
        public virtual SocioComercial SocioComercial { get; set; }

        public int? CfdiRecibidos_Id { get; set; }
        [ForeignKey("CfdiRecibidos_Id")]
        public virtual RecibidosComprobanteDR RecibidosComprobante { get; set; }

        [DisplayName("Carga PDF")]
        public int? CfdiRecibidos_PDF_Id { get; set; }
        [ForeignKey("CfdiRecibidos_PDF_Id")]
        public virtual RecibidosPDFDR RecibidosPdf { get; set; }

        [DisplayName("Carga XML")]
        public int? CfdiRecibidos_XML_Id { get; set; }
        [ForeignKey("CfdiRecibidos_XML_Id")]
        public virtual RecibidosXMLDR RecibidosXml { get; set; }

        [DisplayName("Serie")]
        public String CfdiRecibidos_Serie { get; set; }

        [DisplayName("Folio")]
        public String CfdiRecibidos_Folio { get; set; }

        [DisplayName("UUID")]
        public String CfdiRecibidos_UUID { get; set; }

        public int? Adjuntos_Id { get; set; }

        [DisplayName("Estado Comercial")]
        public c_EstadoComercial EstadoComercial { get; set; }

        [DisplayName("Validaciones")]
        public int? Solicitud_Id { get; set; }
        [ForeignKey("Solicitud_Id")]
        public virtual SolicitudesDR Solicitudes { get; set; }

        [DisplayName("Validaciones")]
        public int? Pagos_Id { get; set; }
        [ForeignKey("Pagos_Id")]
        public virtual PagosDR Pagos { get; set; }

        public int? Aprobador_Id { get; set; }
        public int? Departamento_Id { get; set; }
        [ForeignKey("Departamento_Id")]
        public virtual Departamento Departamento { get; set; }

        public String MotivoRechazo { get; set; }

        public String OrdenDeCompra { get; set; }
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

    }
}