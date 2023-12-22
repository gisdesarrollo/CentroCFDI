using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ResponseValidaCfdiDto
    {
        public String status { get; set; }
        public List<DetailSection> detail { get; set; }
        public string cadenaOriginalSAT { get; set; }
        public string cadenaOriginalComprobante { get; set; }
        public string uuid { get; set; }
        public string statusSat { get; set; }
        public string statusCodeSat { get; set; }
        public string isCancelable { get; set; }
        public string statusCancelation { get; set; }

    }

    public class DetailItem
    {
        public String message { get; set; }

        public String messageDetail { get; set; }

        public int type { get; set; }

        public String typeValue { get; set; }

    }

    public class DetailSection
    {
        public List<DetailItem> Detail { get; set; }
        public String Section { get; set; }
    }
}
