using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_TipoDeTrafico
    {

        [DescriptionAttribute("TT01-Tráfico local")]
        TT01,

        [DescriptionAttribute("TT02-Tráfico interlineal remitido")]
        TT02,

        [DescriptionAttribute("TT03-Tráfico interlineal recibido")]
        TT03,

        [DescriptionAttribute("TT04-Tráfico interlineal en tránsito")]
        TT04,
    
}
}
