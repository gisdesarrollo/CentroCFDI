using API.Catalogos;
using Aplicacion.Context;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos
{
    public class AcondicionarUsuarios
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void CargaInicial(ref Usuario usuario)
        {
            if(usuario.Sucursales != null)
            {
                foreach (var sucursal in usuario.Sucursales)
                {
                    sucursal.Sucursal = null;
                    sucursal.Usuario = null;
                }
            }
        }

        public void Sucursales(Usuario usuario)
        {
            if (usuario.Sucursales != null)
            {
                var idsBorrar = usuario.Sucursales.Select(e => e.Id);
                var sucursalesAnteriores = _db.UsuariosSucursales.Where(es => es.UsuarioId == usuario.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.UsuariosSucursales.RemoveRange(sucursalesAnteriores);
                _db.SaveChanges();

                foreach (var sucursal in usuario.Sucursales)
                {
                    sucursal.UsuarioId = usuario.Id;
                    sucursal.Sucursal = null;
                    _db.UsuariosSucursales.AddOrUpdate(sucursal);
                }

                _db.SaveChanges();
            }
            else
            {
                var sucursalesAnteriores = _db.UsuariosSucursales.Where(es => es.UsuarioId == usuario.Id).ToList();

                _db.UsuariosSucursales.RemoveRange(sucursalesAnteriores);
                _db.SaveChanges();
            }
        }
    }
}
