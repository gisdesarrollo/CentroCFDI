using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ComplementoCartaPorteDto
    {
        public string Version { get; set; }
        public string TipoComprobante { get; set; }
        public string moneda { get; set; }
        public Decimal Subtotal { get; set; }
        public Decimal Total { get; set; }
        public string TipoRelacion {get;set;}
        public string UuidRelacionado { get; set; }
        public string RfcReceptor { get; set; }
        public string ClaveTransporte { get; set; }
        public string TransporteInternacional { get; set; }
        public string EntradaSalidMercancia { get; set; }
        public string ViaEntradaSalidMercancia { get; set; }


    }
}
