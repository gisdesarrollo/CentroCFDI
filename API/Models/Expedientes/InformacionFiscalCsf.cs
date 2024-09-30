using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Expedientes
{
    public class InformacionFiscalCsf
    {
        public DateTime Fecha { get; set; }
        public string RFC { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
