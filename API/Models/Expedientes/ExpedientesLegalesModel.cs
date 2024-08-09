using API.Models.Reportes;
using System.Collections.Generic;

namespace API.Models.Expedientes
{
    public class ExpedientesLegalesModel : FechasModel
    {
        public int SucursalId { get; set; }

        public int UsuarioId { get; set; }

        public virtual List<API.Operaciones.Expedientes.ExpedienteLegal> ExpedientesLegales { get; set; }

    }
}
