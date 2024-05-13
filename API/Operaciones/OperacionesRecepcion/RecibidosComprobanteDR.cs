using API.Catalogos;
using API.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("Recibidos_Comprobante")]
    public class RecibidosComprobanteDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Sucursal")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Socio Comercial")]
        public int SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        [DisplayName("Fecha Comprobante")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        public String Serie { get; set; }

        public String Folio { get; set; }

        [DisplayName("Tipo Comprobante")]
        public c_TipoDeComprobante TipoComprobante { get; set; }

        public String Version { get; set; }

        public string FormaPago { get; set; }

        public c_Moneda Moneda { get; set; }

        public double? TipoCambio { get; set; }

        public String LugarExpedicion { get; set; }

        [DisplayName("Metodo de Pago")]
        public c_MetodoPago? MetodoPago { get; set; }

        public Double Descuento { get; set; }

        public Double Subtotal { get; set; }

        public Double Total { get; set; }

        public String Uuid { get; set; }

        public DateTime FechaTimbrado { get; set; }

        [DisplayName("Total de Impuestos Retenidos")]
        public Double TotalImpuestosRetenidos { get; set; }

        [DisplayName("Total de Impuestos Trasladados")]
        public Double TotalImpuestosTrasladados { get; set; }
    }
}
