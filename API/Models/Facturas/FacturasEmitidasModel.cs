using API.Models.Reportes;
using API.Operaciones.Facturacion;
using System.Collections.Generic;

namespace API.Models.Facturas
{
    public class FacturasEmitidasModel : FechasModel
    {
        public int SucursalId { get; set; }

        public virtual List<FacturaEmitida> FacturasEmitidas { get; set; }
    }
}
