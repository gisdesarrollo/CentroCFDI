using API.Catalogos;
using API.Enums;
using API.Relaciones;
using Aplicacion.Context;
using System.Data.Entity;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.SolicitudesAccesos
{
    public class OperarSolicitudes
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public string Autorizar(int solicitudAccesoId)
        {
            var solicitudAcceso = _db.SolicitudesAccesos.Find(solicitudAccesoId);
            solicitudAcceso.Procesado = true;
            _db.Entry(solicitudAcceso).State = EntityState.Modified;
            _db.SaveChanges();

            var proveedor = new Proveedor
            {
                CodigoPostal = solicitudAcceso.CodigoPostal,
                Email = solicitudAcceso.Email,
                FechaAlta = solicitudAcceso.FechaAlta,
                PaginaWeb = solicitudAcceso.PaginaWeb,
                Pais = solicitudAcceso.Pais,
                RazonSocial = solicitudAcceso.RazonSocial,
                Rfc = solicitudAcceso.Rfc,
                Saldo = solicitudAcceso.Saldo,
                Status = solicitudAcceso.Status,
                Telefono1 = solicitudAcceso.Telefono1,
                Telefono2 = solicitudAcceso.Telefono2,
                GrupoId = solicitudAcceso.GrupoId
            };

            _db.Proveedores.Add(proveedor);
            _db.SaveChanges();

            //Asignar Sucursales
            var sucursalesGrupo = _db.Sucursales.Where(s => s.GrupoId == solicitudAcceso.GrupoId && s.Status == Status.Activo);
            foreach (var sucursalGrupo in sucursalesGrupo)
            {
                var proveedorSucursal = new ProveedorSucursal
                {
                    ProveedorId = proveedor.Id,
                    SucursalId = sucursalGrupo.Id
                };
                _db.ProveedoresSucursales.Add(proveedorSucursal);
            }
            _db.SaveChanges();

            //TODO: Enviar Correo

            return proveedor.Rfc;
        }

        public void Rechazar(int solicitudAccesoId)
        {
            var solicitudAcceso = _db.SolicitudesAccesos.Find(solicitudAccesoId);
            solicitudAcceso.Status = Status.Cancelado;
            solicitudAcceso.Procesado = true;
            _db.Entry(solicitudAcceso).State = EntityState.Modified;
            _db.SaveChanges();

            //TODO: Enviar Correo
            
        }
    }
}
