using API.Operaciones.Expedientes;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Facturas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Expedientes
{
    public class ProcesaExpediente
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion Variables

        public List<ExpedienteFiscal> Filtrar(DateTime fechaInicial, DateTime fechaFinal, int sucursalId, int socioComercialId)
        {
            List<ExpedienteFiscal> expedientesFiscales = new List<ExpedienteFiscal>();

            //Si el usuario es proveedor
            expedientesFiscales = _db.ExpedientesFiscales
                                .Where(dr =>
                                        dr.FechaCreacion >= fechaInicial &&
                                        dr.FechaCreacion <= fechaFinal &&
                                        dr.SocioComercialId == socioComercialId &&
                                        dr.SucursalId == sucursalId)
                                .OrderBy(dr => dr.FechaCreacion)
                                .ToList();

            return expedientesFiscales;
        }


    }
}
