using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class DataResponseXsaDto
    {
        public String uuid { get; set; }
        public DateTime fecha { get; set; }
        public String serie { get; set; }
        public int folio { get; set; }
        public String rfc { get; set; }
        public String monto { get; set; }
        public String status { get; set; }
        public String xmlDownload { get; set; }
    }
}
