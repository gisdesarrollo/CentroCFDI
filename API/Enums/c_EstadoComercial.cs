using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
    public enum c_EstadoComercial
    {
        [DescriptionAttribute("En Revisión")]
        EnRevision,
        [DescriptionAttribute("Aprobado")]
        Aprobado,
        [DescriptionAttribute("Rechazado")]
        Rechazado,
    }
}
