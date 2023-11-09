using API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ComprobanteDto
    {
        public int SucursalId { get; set; }
        public String RfcSucursal { get; set; }
        public int ReceptorId { get; set; }
        public String RfcReceptor { get; set; }
        public DateTime FechaDocumento { get; set; }
        public int Folio { get; set; }
        public c_Moneda? Moneda { get; set; }
        public String Serie { get; set; }
        public double Subtotal { get; set; }
        public double? TipoCambio { get; set; }
        public c_TipoDeComprobante TipoComprobante { get; set; }
        public double Total { get; set; }
        public string FormaPago { get; set; }
        public c_MetodoPago? MetodoPago { get; set; }
        public byte[] ArchivoFisicoXml { get; set; }

        public String Referencia { get; set; }

        public double TotalImpuestoTrasladado { get; set; }
        public double TotalImpuestoRetenidos { get; set; }

    }
}
