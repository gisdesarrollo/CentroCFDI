using API.Enums;
using API.Enums.CartaPorteEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aplicacion.Utilidades
{
    public class DescripcionesEnum
    {
        
        public static string ObtenerDescripcionEnum(object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        //presenta Error
        public static string ObtenerDescripcionXml(object valorEnum)
        {
            string name = valorEnum.GetType().Name;
            try
            {
                switch (name)
                {
                    case "c_RegimenFiscal":
                        return ((XmlEnumAttribute)typeof(c_RegimenFiscal).GetMember(valorEnum.ToString())[0].GetCustomAttributes(typeof(XmlEnumAttribute), false)[0]).Name;
                    case "c_TipoDeComprobante":
                        return ((XmlEnumAttribute)typeof(c_TipoDeComprobante).GetMember(valorEnum.ToString())[0].GetCustomAttributes(typeof(XmlEnumAttribute), false)[0]).Name;
                    case "c_TipoRelacion":
                        return ((XmlEnumAttribute)typeof(c_TipoRelacion).GetMember(valorEnum.ToString())[0].GetCustomAttributes(typeof(XmlEnumAttribute), false)[0]).Name;
                    case "c_UsoCfdiCP":
                        return ((XmlEnumAttribute)typeof(c_UsoCfdiCP).GetMember(valorEnum.ToString())[0].GetCustomAttributes(typeof(XmlEnumAttribute), false)[0]).Name;
                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return "N/A";
            }
        }

        public static string GetDescripcionTipoRelacion(string value)
        {
            try
            {
                switch (value)
                {
                    case "01":
                        return "01-Nota de crédito de los documentos relacionados";
                    case "02":
                        return "02-Nota de débito de los documentos relacionados";
                    case "03":
                        return "03-Devolución de mercancía sobre facturas o traslados previos";
                    case "04":
                        return "04-Sustitución de los CFDI previos";
                    case "05":
                        return "05-Traslados de mercancias facturados previamente";
                    case "06":
                        return "06-Factura generada por los traslados previos";
                    case "07":
                        return "07-CFDI por aplicación de anticipo";
                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return "N/A";
            }
        }

        
        public static string NumerosLetras(string numero)
        {
            
            string str1 = "";
            if (numero.IndexOf(".") == -1)
                numero += ".00";
            DescripcionesEnum.r = new Regex("\\d{1,9}.\\d{1,2}");
            if (DescripcionesEnum.r.Matches(numero).Count <= 0)
                return str1 = (string)null;
            string[] strArray = numero.Split('.');
            string str2 = strArray[1] + "/100 mn";
            return ((int.Parse(strArray[0]) != 0 ? (int.Parse(strArray[0]) <= 999999 ? (int.Parse(strArray[0]) <= 999 ? (int.Parse(strArray[0]) <= 99 ? (int.Parse(strArray[0]) <= 9 ? DescripcionesEnum.getUnidades(strArray[0]) : DescripcionesEnum.getDecenas(strArray[0])) : DescripcionesEnum.getCentenas(strArray[0])) : DescripcionesEnum.getMiles(strArray[0])) : DescripcionesEnum.getMillones(strArray[0])) : "cero ") + " pesos " + str2).ToUpper();
        }

        private static string getUnidades(string numero)
        {
            string s = numero.Substring(numero.Length - 1);
            return DescripcionesEnum.UNIDADES[int.Parse(s)];
        }

        private static string getDecenas(string num)
        {
            int num1 = int.Parse(num);
            if (num1 < 10)
                return DescripcionesEnum.getUnidades(num);
            if (num1 <= 19)
                return DescripcionesEnum.DECENAS[num1 - 10];
            string unidades = DescripcionesEnum.getUnidades(num);
            return unidades.Equals("") ? DescripcionesEnum.DECENAS[int.Parse(num.Substring(0, 1)) + 8] : DescripcionesEnum.DECENAS[int.Parse(num.Substring(0, 1)) + 8] + "y " + unidades;
        }

        private static string getCentenas(string num)
        {
            if (int.Parse(num) <= 99)
                return DescripcionesEnum.getDecenas(string.Concat((object)int.Parse(num)) ?? "");
            return int.Parse(num) == 100 ? " cien " : DescripcionesEnum.CENTENAS[int.Parse(num.Substring(0, 1))] + DescripcionesEnum.getDecenas(num.Substring(1));
        }

        private static string getMiles(string numero)
        {
            string num = numero.Substring(numero.Length - 3);
            string str = numero.Substring(0, numero.Length - 3);
            return int.Parse(str) > 0 ? DescripcionesEnum.getCentenas(str) + "mil " + DescripcionesEnum.getCentenas(num) : DescripcionesEnum.getCentenas(num) ?? "";
        }

        private static string getMillones(string numero)
        {
            string numero1 = numero.Substring(numero.Length - 6);
            string str = numero.Substring(0, numero.Length - 6);
            return (int.Parse(str) <= 1 ? DescripcionesEnum.getUnidades(str) + "millon " : DescripcionesEnum.getCentenas(str) + "millones ") + DescripcionesEnum.getMiles(numero1);
        }
        public static string GetDescripcionImpuesto(string value)
        {
            switch (value)
            {
                case "001":
                    return "001-ISR";
                case "002":
                    return "002-IVA";
                case "003":
                    return "003-IEPS";
                default:
                    return string.Empty;
            }
        }

        public static string GetDescripcionFormaPago(string value)
        {
            switch (value)
            {
                case "01":
                    return "01-Efectivo";
                case "02":
                    return "02-Cheque Normativo";
                case "03":
                    return "03-Transferencia Electrónica De Fondos";
                case "04":
                    return "04-Tarjeta De Credito";
                case "05":
                    return "05-Monedero Electronico";
                case "06":
                    return "06-Dinero Electronico";
                case "08":
                    return "08-Vales De Despensa";
                case "12":
                    return "12-Dacion En Pago";
                case "13":
                    return "13-Pago Por Subrogación";
                case "14":
                    return "14-Pago Por Consignación";
                case "15":
                    return "15-Condonación";
                case "17":
                    return "17-Compensación";
                case "23":
                    return "23-Novación";
                case "24":
                    return "24-Confusión";
                case "25":
                    return "25-Remision De Deuda";
                case "26":
                    return "26-Prescripción O Caducidad";
                case "27":
                    return "27-A Satisfaccion Del Acreedor";
                case "28":
                    return "28-Tarjeta De Debito";
                case "29":
                    return "29-Tarjeta De Servicios";
                case "30":
                    return "30-Aplicacion De Anticipos";
                case "31":
                    return "31-Intermediario Pagos";
                case "99":
                    return "99-Por Definir";
                default:
                    return string.Empty;

            }
        }
        private static Regex r;
        private static string[] UNIDADES = new string[10]
   {
      "",
      "un ",
      "dos ",
      "tres ",
      "cuatro ",
      "cinco ",
      "seis ",
      "siete ",
      "ocho ",
      "nueve "
   };
        private static string[] DECENAS = new string[18]
    {
      "diez ",
      "once ",
      "doce ",
      "trece ",
      "catorce ",
      "quince ",
      "dieciseis ",
      "diecisiete ",
      "dieciocho ",
      "diecinueve",
      "veinte ",
      "treinta ",
      "cuarenta ",
      "cincuenta ",
      "sesenta ",
      "setenta ",
      "ochenta ",
      "noventa "
    };
        private static string[] CENTENAS = new string[10]
        {
      "",
      "ciento ",
      "doscientos ",
      "trescientos ",
      "cuatrocientos ",
      "quinientos ",
      "seiscientos ",
      "setecientos ",
      "ochocientos ",
      "novecientos "
        };

        public static string fechaFormat(string fecha)
        {
            DateTime fechaConvertDate = Convert.ToDateTime(fecha);
            string fechaFormat = fechaConvertDate.ToString("dd/MM/yyyy");
            return fechaFormat;
        }

    }
}
