using API.Control;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.LogicaAlterna
{
    public class ObtenerConfiguraciones
    {
        #region Configuraciones

        private readonly List<Configuracion> _configuracion;

        #endregion

        public ObtenerConfiguraciones()
        {
            var db = new AplicacionContext();
            _configuracion = db.Configuraciones.ToList();
        }

        public double Obtener(string concepto)
        {
            return Convert.ToDouble(_configuracion.Where(p => p.Concepto == concepto));
        }
    }
}
