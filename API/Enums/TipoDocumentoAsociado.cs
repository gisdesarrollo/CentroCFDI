using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
    public enum TipoDocumentoAsociado
    {
        [DescriptionAttribute("Orden de Compra")]
        OrdenCompra,

        [DescriptionAttribute("Nota de Crédito")]
        NotaCredito,

        [DescriptionAttribute("Nota de Débito")]
        NotaDebito,
    }
}
