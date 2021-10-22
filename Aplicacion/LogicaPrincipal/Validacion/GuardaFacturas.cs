using API.Catalogos;
using API.Context;
using API.Enums;
using API.Operaciones.Facturacion;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Validacion
{
    public class GuardaFacturas
    {
        #region Variables

        private readonly DataContext _db = new DataContext();

        #endregion

        public bool GuardarFacturaRecibida(FacturaRecibida facturaRecibida, int sucursalId, TiposGastos tipoGasto)
        {
            var receptor = _db.Sucursales.Find(sucursalId);
            var emisor = _db.Proveedores.FirstOrDefault(s => s.Rfc == facturaRecibida.Emisor.Rfc && s.GrupoId == receptor.GrupoId);

            if(emisor == null)
            {
                emisor = new Proveedor
                {
                    //Domicilio
                    CodigoPostal = facturaRecibida.Emisor.CodigoPostal,
                    Pais = facturaRecibida.Emisor.Pais,
                    Referencia = facturaRecibida.Emisor.Referencia,

                    //Datos
                    RazonSocial = facturaRecibida.Emisor.RazonSocial,
                    Rfc = facturaRecibida.Emisor.Rfc,
                    Status = Status.Activo
                };
                _db.Proveedores.Add(emisor);
                _db.SaveChanges();
            }

            //Asignaciones Factura
            facturaRecibida.EmisorId = emisor.Id;
            facturaRecibida.ReceptorId = receptor.Id;
            facturaRecibida.Version = "3.2";
            facturaRecibida.TipoGasto = tipoGasto;

            facturaRecibida.Emisor = null;
            facturaRecibida.Receptor = null;

            _db.FacturasRecibidas.Add(facturaRecibida);

            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var errores = new List<String>();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errores.Add(String.Format("Propiedad: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
                throw new Exception(string.Join(",", errores.ToArray()));
            }

            return true;
        }
    }
}
