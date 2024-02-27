using System;
using API.Catalogos;
using API.Context;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infodextra.LogicaPrincipal;
using Utilerias.LogicaPrincipal;
using API.Operaciones.Facturacion;
using System.Data.Entity.Validation;

namespace Aplicacion.RecepcionDocumentos
{
    public class ValidacionesComerciales
    {
        #region Variables

        private readonly DataContext _db = new DataContext();

        #endregion

        public void Negocios(FacturaRecibida facturaRecibida, int sucursalId)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);
            if (facturaRecibida.Receptor == null)
            {
                throw new Exception(String.Format("El receptor de la factura {0} no coincide con el RFC {1}", facturaRecibida.NombreArchivoXml, sucursal.Rfc));
            }

            //Factura Repetida
            var facturaValidadaPreviamente = _db.FacturasRecibidas.FirstOrDefault(f => f.Uuid == facturaRecibida.Uuid);
            if (facturaValidadaPreviamente != null)
            {
                throw new Exception(String.Format("La factura {0} ya fue validada", facturaValidadaPreviamente.NombreArchivoXml));
            }

            //La factura es de la empresa
            if (facturaRecibida.Receptor.Rfc != sucursal.Rfc)
            {
                throw new Exception(String.Format("El rfc {0} no corresponde con la sucursal actual ({1})", facturaRecibida.Receptor.Rfc, sucursal.Rfc));
            }

            //El proveedor Existe
            facturaRecibida.Emisor = ChecarProveedor(facturaRecibida, sucursalId);
        }

        private Proveedor ChecarProveedor(FacturaRecibida facturaRecibida, int sucursalId)
        {
            var proveedor = _db.Proveedores.FirstOrDefault(p => p.Rfc == facturaRecibida.Emisor.Rfc);
            if (proveedor != null)
            {
                return proveedor;
            }

            var sucursal = _db.Sucursales.Find(sucursalId);
            proveedor = new Proveedor
            {
                RazonSocial = facturaRecibida.Emisor.RazonSocial,
                Rfc = facturaRecibida.Emisor.Rfc,
                Pais = "MEX",
                GrupoId = sucursal.GrupoId
            };
            _db.Proveedores.Add(proveedor);

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

            return proveedor;
        }

    }
}
