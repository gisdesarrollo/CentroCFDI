using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
    public enum c_EstadoPago
    {
        [DescriptionAttribute("Pendiente")]
        Pendiente,
        [DescriptionAttribute("Programado Para Pago")]
        ProgramadoParaPago,
        [DescriptionAttribute("Rechazado")]
        Rechazado,
    }
}
