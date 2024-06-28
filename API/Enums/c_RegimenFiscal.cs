using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums
{
   public enum c_RegimenFiscal
    {
        [DescriptionAttribute("601 - General de Ley Personas Morales")]
        A = 601,
        [DescriptionAttribute("603 - Personas Morales con Fines no Lucrativos")]
        B = 603,
        [DescriptionAttribute("605 - Sueldos y Salarios e Ingresos Asimilados a Salarios")]
        C = 605,
        [DescriptionAttribute("606 - Arrendamiento")]
        D = 606,
        [DescriptionAttribute("607 - Régimen de Enajenación o Adquisición de Bienes")]
        E = 607,
        [DescriptionAttribute("608 - Demás ingresos")]
        F = 608,
        [DescriptionAttribute("610 - Residentes en el Extranjero sin Establecimiento Permanente en México")]
        G = 610,
        [DescriptionAttribute("611 - Ingresos por Dividendos (socios y accionistas)")]
        H = 611,
        [DescriptionAttribute("612 - Personas Físicas con Actividades Empresariales y Profesionales")]
        I = 612,
        [DescriptionAttribute("614 - Ingresos por intereses")]
        J = 614,
        [DescriptionAttribute("615 - Régimen de los ingresos por obtención de premios")]
        K = 615,
        [DescriptionAttribute("616 - Sin obligaciones fiscales")]
        L = 616,
        [DescriptionAttribute("620 - Sociedades Cooperativas de Producción que optan por diferir sus ingresos")]
        M = 620,
        [DescriptionAttribute("621 - Incorporación Fiscal")]
        N = 621,
        [DescriptionAttribute("622 - Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras")]
        O = 622,
        [DescriptionAttribute("623 - Opcional para Grupos de Sociedades")]
        P = 623,
        [DescriptionAttribute("624 - Coordinados")]
        Q = 624,
        [DescriptionAttribute("625 - Régimen de las Actividades Empresariales con ingresos a través de Plataformas Tecnológicas")]
        R = 625,
        [DescriptionAttribute("626 - Régimen Simplificado de Confianza")]
        S = 626
    }
}
