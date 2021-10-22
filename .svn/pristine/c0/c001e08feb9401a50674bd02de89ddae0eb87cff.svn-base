using API.Catalogos;
using API.Operaciones.ComplementosPagos;
using CFDI.API.Enums.CFDI33;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace API.Models.Facturas
{
    public class Factura
    {
        //Impuestos
        //TODO: Quitar esto, ya que solo pertenece a Pagos
        public virtual List<Impuesto> Impuestos { get; set; }

        //Datos
        public DateTime Fecha { get; set; }

        public String Serie { get; set; }

        public String Folio { get; set; }

        public String NoCertificado { get; set; }

        public String CadenaOriginal { get; set; }

        public c_TipoDeComprobante TipoComprobante { get; set; }

        public String Version { get; set; }

        public c_FormaPago FormaPago { get; set; }

        public String NumeroCuentaPago { get; set; }

        public c_Moneda Moneda { get; set; }

        public double? TipoCambio { get; set; }

        public String LugarExpedicion { get; set; }

        [DisplayName("Metodo de Pago")]
        public c_MetodoPago MetodoPago { get; set; }

        //Totales
        public Double Descuento { get; set; }

        public String MotivoDescuento { get; set; }

        public Double Subtotal { get; set; }

        [DisplayName("Total de Impuestos Retenidos")]
        public Double TotalImpuestosRetenidos { get; set; }

        [DisplayName("Total de Impuestos Trasladados")]
        public Double TotalImpuestosTrasladados { get; set; }

        [XmlAttributeAttribute("total")]
        public Double Total { get; set; }

        //Timbrado
        public DateTime FechaTimbrado { get; set; }

        public String NoCertificadoSat { get; set; }

        public String SelloDigitalCfdi { get; set; }

        public String SelloSat { get; set; }

        public String Certificado { get; set; }

        [DisplayName("Folio Fiscal")]
        public String Uuid { get; set; }

        #region Archivos
        public String NombreArchivoXml { get; set; }

        [DisplayName("Archivo")]
        public byte[] ArchivoFisicoXml { get; set; }

        [DisplayName("Path PDF")]
        public String PathPdf { get; set; }

        #endregion

        #region Objetos

        [NotMapped]
        public String Desplegado { get { return String.Format("Factura: {0}{1} - {2:dd/MM/yyyy} - {3:c}", Serie, Folio, Fecha, Total); } }

        [DisplayName("Departamento")]
        public int? DepartamentoId { get; set; }
        [ForeignKey("DepartamentoId")]
        public virtual Departamento Departamento { get; set; }

        [DisplayName("Centro de Costo")]
        public int? CentroCostoId { get; set; }
        [ForeignKey("CentroCostoId")]
        public virtual CentroCosto CentroCosto { get; set; }

        #endregion

        #region Complementos de Pago

        public virtual List<ComplementoPago> ComplementosPago { get; set; }

        #endregion
    }
}
