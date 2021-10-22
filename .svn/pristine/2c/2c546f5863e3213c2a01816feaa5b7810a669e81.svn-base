using API.Models.Facturas;
using Aplicacion.Context;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Facturas
{

    public class OperacionesCfdisEmitidos
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void ObtenerFacturas(ref FacturasEmitidasModel facturasEmitidasModel)
        {
            var fechaInicial = facturasEmitidasModel.FechaInicial;
            var fechaFinal = facturasEmitidasModel.FechaFinal;
            var sucursalId = facturasEmitidasModel.SucursalId;
            facturasEmitidasModel.FacturasEmitidas = _db.FacturasEmitidas.Where(fe => fe.EmisorId == sucursalId && fe.Fecha >= fechaInicial && fe.Fecha <= fechaFinal).ToList();
        }
    }
}
