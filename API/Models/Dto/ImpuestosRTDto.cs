using API.Operaciones.ComplementosPagos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ImpuestosRTDto
    {

        public RetencionDR retencion { get; set; }

        public TrasladoDR traslado { get; set; }
    }
}
