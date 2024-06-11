using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
    public enum c_TipoDocumentoRecibido
    {
        [DescriptionAttribute("Comprobante CFDI")]
        CFDI,
        [DescriptionAttribute("Comprobante No Fiscal")]
        ComprobanteNoFiscal,
        [DescriptionAttribute("Comprobante Extranjero")]
        ComprobanteExtranjero,
    }
}
