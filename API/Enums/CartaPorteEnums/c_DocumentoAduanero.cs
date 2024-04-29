using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Enums.CartaPorteEnums
{
    public enum c_DocumentoAduanero
    {
        [DescriptionAttribute("01-Pedimento")]
        Ejemplo01 = 1,
        [DescriptionAttribute("02-Autorización de importación temporal")]
        Ejemplo02 = 2,
        [DescriptionAttribute("03-Autorización de importación temporal de embarcaciones")]
        Ejemplo03 = 3,
        [DescriptionAttribute("04-Autorización de importación temporal de mercancías, destinadas al mantenimiento y reparación de las mercancías importadas temporalmente")]
        Ejemplo04 = 4,
        [DescriptionAttribute("05-Autorización para la importación de vehículos especialmente construidos o transformados, equipados con dispositivos o aparatos diversos para cumplir con contrato derivado de licitación pública")]
        Ejemplo05 = 5,
        [DescriptionAttribute("06-Aviso de exportación temporal")]
        Ejemplo06 = 6,
        [DescriptionAttribute("07-Aviso de traslado de mercancías de empresas con Programa IMMEX, RFE u Operador Económico Autorizado")]
        Ejemplo07 = 7,
        [DescriptionAttribute("08-'Aviso para el traslado de autopartes ubicadas en la franja o región fronteriza a la industria terminal automotriz o manufacturera de vehículos de autotransporte en el resto del territorio nacional")]
        Ejemplo08 = 8,
        [DescriptionAttribute("09-Constancia de importación temporal, retorno o transferencia de contenedores")]
        Ejemplo09 = 9,
        [DescriptionAttribute("10-Constancia de transferencia de mercancías")]
        Ejemplo10 = 10,
        [DescriptionAttribute("11-Autorización de donación de mercancías al Fisco Federal que se encuentren en el extranjero")]
        Ejemplo11 = 11,
        [DescriptionAttribute("12-Cuaderno ATA")]
        Ejemplo12 = 12,
        [DescriptionAttribute("13-Listas de intercambio")]
        Ejemplo13 = 13,
        [DescriptionAttribute("14-Permiso de Importación Temporal")]
        Ejemplo14 = 14,
        [DescriptionAttribute("15-Permiso de importación temporal de casa rodante")]
        Ejemplo15 = 15,
        [DescriptionAttribute("16-Permiso de importación temporal de embarcaciones")]
        Ejemplo16 = 16,
        [DescriptionAttribute("17-Solicitud de donación de mercancías en casos de emergencias o desastres naturales")]
        Ejemplo17 = 17,
        [DescriptionAttribute("18-Aviso de consolidado")]
        Ejemplo18 = 18,
        [DescriptionAttribute("19-Aviso de cruce de mercancias")]
        Ejemplo19 = 19,
        [DescriptionAttribute("20-Otro")]
        Ejemplo20 = 20
    }
}
