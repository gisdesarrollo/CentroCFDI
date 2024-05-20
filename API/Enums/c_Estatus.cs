using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
    public enum c_Estatus
    {
        [DescriptionAttribute("Activo")]
        Activo = 1,
        [DescriptionAttribute("Cerrado")]
        Cerrado = 0,
    }
}
