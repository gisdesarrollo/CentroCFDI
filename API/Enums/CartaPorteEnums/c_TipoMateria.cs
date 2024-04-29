using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_TipoMateria
    {
        [Description("01-Materia prima")]
        Materia_Prima = 1,

        [Description("02-Materia procesada")]
        Materia_Procesada = 2,

        [Description("03-Materia terminada(producto terminado)")]
        Materia_Terminada = 3,

        [Description("04-Materia para la industria manufacturera")]
        Materia_Industria_Manufacturera = 4,

        [Description("05-Otra")]
        Otra = 5

    }
}
