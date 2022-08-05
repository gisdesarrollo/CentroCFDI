using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    [Serializable]
    public class DataCancelacionRequestXsaDto
    {
        public String motivo { get; set; }
        public String folioSustitucion { get; set; }

        public List<String> uuid { get; set; }
    }
}
