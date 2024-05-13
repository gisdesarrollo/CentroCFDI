using API.Models.DocumentosRecibidos;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Facturas
{
    public class OperacionesDocumentosRecibidos
    {
        #region variables
        private readonly AplicacionContext _db = new AplicacionContext();
        #endregion
        
        public void ObtenerFacturas(ref DocumentosRecibidosModel documentosRecibidosModel, int sucursalId)
        {
            var fechaInicial = documentosRecibidosModel.FechaInicial;
            var fechaFinal = documentosRecibidosModel.FechaFinal.AddDays(1); //SE AGREGA UN DIA A LA FECHA FINAL
            documentosRecibidosModel.DocumentosRecibidos = _db.DocumentosRecibidos.Where(fe => fe.FechaComprobante >= fechaInicial && fe.FechaComprobante < fechaFinal && fe.SucursalId == sucursalId).ToList();

        }

    }
}
