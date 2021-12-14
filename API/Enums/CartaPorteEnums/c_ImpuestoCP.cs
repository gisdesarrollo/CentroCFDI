using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_ImpuestoCP
    {
        Isr = 001,
        Iva = 002,
        Ieps = 003
    }
     public static class myEnumExtensions
    {
        public static string ToReport(this c_ImpuestoCP value)
        {
            return ((int)value).ToString("000"); 
        }
    }

}
