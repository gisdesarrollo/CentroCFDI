using API.CatalogosCartaPorte;
using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Enums.CartaPorteEnums;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto
{
    public class DocumentoRelacionadoDto
    {
        public String IdDocumento { get; set; }

        public String Serie { get; set; }

        public String Folio { get; set; }
        public double? EquivalenciaDR {  get; set; }   
        public c_Moneda? Moneda { get; set; }

        public double? ImporteSaldoAnterior { get; set; }

        public double? ImportePagado { get; set; }

        public double? ImporteSaldoInsoluto { get; set; }

        public string ObjetoImpuestoId { get; set; }
        public int PagoId { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double? Base { get; set; }

        public string Impuesto { get; set; }

        public c_TipoFactor? TipoFactor { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? TasaOCuota { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double? Importe { get; set; }
    }
}
