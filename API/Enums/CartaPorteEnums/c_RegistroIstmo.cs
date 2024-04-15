using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_RegistroIstmo
    {
        [Description("01-Coatzacoalcos I")]
        Coatzacoalcos_I = 1,

        [Description("02-Coatzacoalcos II")]
        Coatzacoalcos_II = 2,

        [Description("03-Texistepec")]
        Texistepec = 3,

        [Description("04-San Juan Evangelista")]
        San_Juan_Evangelista = 4,

        [Description("05-Salina Cruz")]
        Salina_Cruz = 5,

        [Description("06-San Blas Atempa")]
        San_Blas_Atempa = 6
    }
}
