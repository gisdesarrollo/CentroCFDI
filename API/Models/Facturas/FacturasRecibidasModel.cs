using API.Models.Reportes;
using API.Operaciones.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Facturas
{
    public class FacturasRecibidasModel: FechasModel
    {
        public int SucursalId { get; set; }

        public virtual List<FacturaRecibida> FacturasRecibidas { get; set; }
    }
}
