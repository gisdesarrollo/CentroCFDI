using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_SectorCofepris
    {
        [Description("01-Medicamento")]
        Medicamento = 1,

        [Description("02-Precursores y químicos de uso dual")]
        Precursores_y_quimicos_de_uso_dual = 2,

        [Description("03-Psicotrópicos y estupefacientes")]
        Psicotrópicos_y_estupefacientes = 3,

        [Description("04-Sustancias tóxicas")]
        Sustancias_tóxicas = 4,

        [Description("05-Plaguicidas y fertilizantes")]
        Plaguicidas_y_fertilizantes = 5
    }
}
