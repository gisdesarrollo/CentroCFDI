using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_CondicionesEspeciales
    {
        [Description("01-Congelados")]
        Congelados = 1,

        [Description("02-Refrigerados")]
        Refrigerados = 2,

        [Description("03-Temperatura Controlada")]
        Temperatura_Controlada = 3,

        [Description("04-Temperatura Ambiente")]
        Temperatura_Ambiente = 4
    }
}
