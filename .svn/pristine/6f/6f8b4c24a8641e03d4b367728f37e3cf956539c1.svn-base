using API.Enums;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Facturas
{
    public class OperacionesCfdisRecibidos
    {

        #region Variables

        private readonly int _sucursalId;
        private readonly DateTime _fechaInicial;
        private readonly DateTime _fechaFinal;

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        #region Constructor

        public OperacionesCfdisRecibidos(int sucursalId)
        {
            _sucursalId = sucursalId;
        }

        public OperacionesCfdisRecibidos(int sucursalId, DateTime fechaInicial, DateTime fechaFinal)
        {
            _sucursalId = sucursalId;
            _fechaInicial = fechaInicial;
            _fechaFinal = fechaFinal;
        }

        #endregion

        public List<FacturaRecibida> ObtenerFacturasRecibidas(TiposGastos? tipoGasto = null)
        {
            var facturasRecibidas = _db.FacturasRecibidas.Where(fr => fr.ReceptorId == _sucursalId).ToList();

            if(tipoGasto != null)
            {
                facturasRecibidas = facturasRecibidas.Where(fr => fr.TipoGasto == tipoGasto).ToList();
            }

            return facturasRecibidas;
        }

        public void Autorizar(bool autorizar, int facturaRecibidaId, int usuarioId, String notasAutorizacion)
        {
            var facturaRecibida = _db.FacturasRecibidas.Find(facturaRecibidaId);

            facturaRecibida.Autorizada = autorizar;
            facturaRecibida.UsuarioAutorizacionId = usuarioId;
            facturaRecibida.FechaAutorizacion = DateTime.Now;
            facturaRecibida.NotasAutorizacion = notasAutorizacion;

            _db.Entry(facturaRecibida).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

    }
}
