using System.ComponentModel;

namespace API.Enums
{
    public enum c_TipoRecepcion
    {
        [DescriptionAttribute("PorComprobacionGastos")]
        PorComprobacionGastos,
        [DescriptionAttribute("Comprobante No Digital")]
        PorAdquisicion,
    }
}
