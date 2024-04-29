using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_FormaFarmaceutica
    {
        [Description("01-Tabletas")]
        Tabletas = 1,

        [Description("02-Capsulas")]
        Capsulas = 2,

        [Description("03-Comprimidos")]
        Comprimidos = 3,

        [Description("04-Grageas")]
        Grageas = 4,

        [Description("05-Suspensión")]
        Suspension = 5,

        [Description("06-Solución")]
        Solución = 6,

        [Description("07-Emulsión")]
        Emulsion = 7,

        [Description("08-Jarabe")]
        Jarabe = 8,

        [Description("09-Inyectable")]
        Inyectable = 9,

        [Description("10-Crema")]
        Crema = 10,

        [Description("11-Ungüento")]
        Unguento = 11,

        [Description("12-Aerosol")]
        Aerosol = 12,

        [Description("13-Gas Medicinal")]
        Gas_Medicinal = 13,

        [Description("14-Gel")]
        Gel = 14,

        [Description("15-Implante")]
        Implante = 15,
         
        [Description("16-Óvulo")]
        Ovulo = 16,

        [Description("17-Parche")]
        Parche = 17,

        [Description("18-Pasta")]
        Pasta = 18,

        [Description("19-Polvo")]
        Polvo = 19,

        [Description("20-Supositorio")]
        Supositorios = 20
    }
}
