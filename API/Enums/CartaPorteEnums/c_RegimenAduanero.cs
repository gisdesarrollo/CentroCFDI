using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_RegimenAduanero
    {
        [DescriptionAttribute("IMD-Definitivo de importación")]
        IMD = 0,
        [DescriptionAttribute("EXD-Definitivo de exportación")]
        EXD = 1,
        [DescriptionAttribute("ITR-Temporales de importación para retomar al extranjero en el mismo estado")]
        ITR = 2,
        [DescriptionAttribute("ITE-Temporales de importación para elaboración, transformación o reparación para empresas con programa IMMEX")]
        ITE = 3,
        [DescriptionAttribute("ETR-Temporales de exportación para retornar al país en el mismo estado")]
        ETR = 4,
        [DescriptionAttribute("ETE-Temporales de exportación para elaboración, transformación o reparación")]
        ETE = 5,
        [DescriptionAttribute("DFI-Depósito Fisca")]
        DFI = 6,
        [DescriptionAttribute("RFE-Elaboración, transformación o reparación en recinto fiscalizado")]
        RFE = 7,
        [DescriptionAttribute("RFS-Recinto fiscalizado estratégico")]
        RFS = 8,
        [DescriptionAttribute("TRA-Tránsitos")]
        TRA = 9
    }
}
