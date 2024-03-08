using API.Operaciones.OperacionesProveedores;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.DocumentosPagos
{
    public class ProcesaDocumentoPago
    {
        private readonly AplicacionContext _db = new AplicacionContext();
        public List<PagosDR> Filtrar(DateTime fechaInicial, DateTime fechaFinal, bool esProveedor, int? socioComercialId)
        {
            //var usuario = _db.Usuarios.Find(usuarioId);
            var pagos = new List<PagosDR>();
            
                pagos = _db.PagoDr
                        .Where(dr => dr.FechaPago >= fechaInicial && dr.FechaPago <= fechaFinal)
                        .ToList();
            
            return pagos;
        }
    }
}
