using API.Catalogos;
using Aplicacion.Context;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos
{
    public class AcondicionarProveedores
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void CargaInicial(ref Proveedor proveedor)
        {
            foreach (var sucursal in proveedor.Sucursales)
            {
                sucursal.Sucursal = null;
                sucursal.Proveedor = null;
            }
        }

        public void Sucursales(Proveedor proveedor)
        {
            if (proveedor.Sucursales != null)
            {
                var idsBorrar = proveedor.Sucursales.Select(e => e.Id);
                var sucursalesAnteriores = _db.ProveedoresSucursales.Where(es => es.ProveedorId == proveedor.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.ProveedoresSucursales.RemoveRange(sucursalesAnteriores);
                _db.SaveChanges();

                foreach (var sucursal in proveedor.Sucursales)
                {
                    sucursal.ProveedorId = proveedor.Id;
                    sucursal.Sucursal = null;
                    _db.ProveedoresSucursales.AddOrUpdate(sucursal);
                }

                _db.SaveChanges();
            }
            else
            {
                var sucursalesAnteriores = _db.ProveedoresSucursales.Where(es => es.ProveedorId == proveedor.Id).ToList();

                _db.ProveedoresSucursales.RemoveRange(sucursalesAnteriores);
                _db.SaveChanges();
            }
        }
    }
}
