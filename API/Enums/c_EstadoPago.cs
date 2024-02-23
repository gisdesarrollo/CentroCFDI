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
        [DescriptionAttribute("En Revisión")]
        EnRevision,
        [DescriptionAttribute("Aprobado")]
        Aprobado,
        [DescriptionAttribute("Pagado")]
        Pagado,
        [DescriptionAttribute("Completado")]
        Completado,
        [DescriptionAttribute("Rechazado")]
        Rechazado,
    }
}
