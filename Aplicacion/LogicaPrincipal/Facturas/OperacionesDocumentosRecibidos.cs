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
        public void ObtenerFacturas(ref DocumentosRecibidosModel documentosRecibidosModel,int usuarioId)
        {
            var fechaInicial = documentosRecibidosModel.FechaInicial;
            var fechaFinal = documentosRecibidosModel.FechaFinal.AddDays(1); //SE AGREGA UN DIA A LA FECHA FINAL
            var sucursalId = documentosRecibidosModel.SucursalId;
            documentosRecibidosModel.DocumentosRecibidos = _db.DocumentoRecibidoDr.Where(fe => fe.Usuario_Id == usuarioId && fe.FechaComprobante >= fechaInicial && fe.FechaComprobante < fechaFinal).ToList();

        }

    }
}
