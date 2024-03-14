using API.Enums;
using API.Enums.CartaPorteEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ComplementoPagoDto
    {
        public string NombreEmisor { get; set; }
        public string NombreReceptor { get; set; }
        public c_UsoCfdiCP UsoCFDI { get; set; }
        public String MetodoPago { get; set; }
        public string FormaPago { get; set; }
        public c_TipoDeComprobante TipoComprobante { get; set; }
        public decimal TipoCambio { get; set; }
        public c_Moneda Moneda { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string Uuid { get; set; }
        public string FechaPago{ get; set; }
        public decimal Monto { get; set; }
    }

}
