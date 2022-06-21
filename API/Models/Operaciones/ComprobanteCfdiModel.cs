using API.Enums;
using API.Operaciones.ComprobantesCfdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Operaciones
{
   public class ComprobanteCfdiModel
    {
        [DisplayName("Fecha Inicial")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicial { get; set; }

        [DisplayName("Fecha Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinal { get; set; }

        [DisplayName("Tipo de Comprobante")]
        public c_TipoDeComprobante? TipoDeComprobante { get; set; }


        [DisplayName("Estatus")]
        public bool? Estatus { get; set; }

        public virtual List<ComprobanteCfdi> ComprobanteCfdi { get; set; }
    }
}
