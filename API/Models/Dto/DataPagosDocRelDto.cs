using API.Enums;
using API.Enums.CartaPorteEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class DataPagosDocRelDto
    {
        public string Rfc { get; set; }
        public string RazonSocial { get; set; } 
        public string Version { get; set; } 
        public string Serie { get; set; }   
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }
        public double? MontoTotalPagos { get; set; }
        public double? TotalTraslado { get; set; }
        public string IdDocumentoDr { get; set; }
        public string FolioDr { get; set; }
        public string SerieDr {  get; set; }
        public double? EquivalenciaDr {  get; set; }
        public c_Moneda? MonedaDr { get; set; }
        public double? ImporteSaldoAnteriorDr { get; set; }
        public double? ImportePagadoDr { get; set; }
        public double? ImporteSaldoInsolutoDr { get; set; }
        public string  ObjetoImpuestoDR { get; set; }
        public double? BaseDr { get; set; }
        public string ImpuestoDr { get; set; }
        public c_TipoFactor? TipoFactorDr { get; set; }
        public decimal? TasaOCuotaDr { get; set; }
        public double? ImporteDr { get; set; }
        public int TotalPagoImpuestoId { get; set; }
    }
}
