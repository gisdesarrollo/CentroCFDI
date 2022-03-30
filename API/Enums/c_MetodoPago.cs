using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
   public enum c_MetodoPago
    {
        [DescriptionAttribute("PUE-Pago en una sola exhibición")]
        PUE,

        [DescriptionAttribute("PPD-Pago en parcialidades o diferido")]
        PPD
    }
}
