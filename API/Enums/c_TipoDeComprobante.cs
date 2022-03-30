using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
    public enum c_TipoDeComprobante
    {
        [DescriptionAttribute("I-Ingreso")]
        I,
        [DescriptionAttribute("E-Egreso")]
        E,
        [DescriptionAttribute("T-Traslado")]
        T,
        [DescriptionAttribute("N-Nómina")]
        N,
        [DescriptionAttribute("P-Pago")]
        P
    }
}
