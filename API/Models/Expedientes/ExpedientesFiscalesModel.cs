using API.Models.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.ExpedientesFiscales
{
    public class ExpedientesFiscalesModel : FechasModel
    {
        public int SucursalId { get; set; }

        public int UsuarioId { get; set; }

        public virtual List<API.Operaciones.Expedientes.ExpedienteFiscal> ExpedientesFiscales { get; set; }

    }

}