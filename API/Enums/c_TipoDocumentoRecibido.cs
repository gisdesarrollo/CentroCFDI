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
        [DescriptionAttribute("CFDI")]
        CFDI,
        [DescriptionAttribute("Comprobante No Digital")]
        ComprobanteNoDigital,
        [DescriptionAttribute("Documento Extranjero")]
        DocumentoExtranjero,
    }
}
