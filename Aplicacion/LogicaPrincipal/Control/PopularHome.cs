using API.Models.Control;
using Aplicacion.Context;
using System;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Control
{
    public class PopularHome
    {

        #region Variables

        private readonly int _sucursalId;
        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        #region Constructor

        public PopularHome(int sucursalId)
        {
            _sucursalId = sucursalId;
        }

        #endregion

        public void Popular(ref HomeModel homeModel)
        {
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFinal = DateTime.Now;

            homeModel.CfdiEmitidos = _db.FacturasEmitidas.Count(fe => fe.EmisorId == _sucursalId && fe.Fecha >= fechaInicial && fe.Fecha <= fechaFinal);
            homeModel.Clientes = _db.Clientes.Count(c => c.SucursalId == _sucursalId);
            homeModel.ComplementosPagoEmitidos = _db.ComplementosPago.Count(fe => fe.SucursalId == _sucursalId && fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);
            homeModel.ComplementosPagoPendientes = _db.ComplementosPago.Count(fe => fe.SucursalId == _sucursalId && !fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);
        }
        
    }
}
