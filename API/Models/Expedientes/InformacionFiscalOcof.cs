using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Expedientes
{
    public class InformacionFiscalOcof
    {
        public string RFC { get; set; }                  
        public string Folio { get; set; }                
        public DateTime FechaEmision { get; set; }       
        public string EstatusCumplimiento { get; set; }  
        public string NumeroDocumento { get; set; }      

    }
}
