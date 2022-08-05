using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    [Serializable]
    public class DataJsonXsaDto
    {
        public String idTipoCfd { get; set; }
        public String idSucursal { get; set; }
        public String nombre { get; set; }
        public String archivoFuente { get; set; }

    }
}
