using System.Linq;
using System.Web.Mvc;
using API.Enums;
using APBox.Context;

namespace APBox.Control
{
    public class PopularDropDowns
    {

        #region Variables

        private readonly bool _soloActivos;

        private readonly int _entidadId;
        private readonly APBoxContext _db = new APBoxContext();
        

        #endregion

        #region Constructor

        public PopularDropDowns(int entidadId, bool soloActivos)
        {
            _soloActivos = soloActivos;
            _entidadId = entidadId;
        }

        #endregion

        #region Catalogos

        public SelectList PopulaBancos(int? seleccion)
        {
            return new SelectList(_db.Bancos.Where(a => a.Status == Status.Activo).OrderBy(a => a.RazonSocial), "Id", "RazonSocial", seleccion);
        }

        public SelectList PopulaBancosClientes(int clienteId, int? seleccion)
        {
            return new SelectList(_db.BancosClientes.Where(a => a.ClienteId == clienteId).OrderBy(a => a.Nombre), "Id", "Nombre", seleccion);
        }

        public SelectList PopulaBancosSucursales(int sucursalId, int? seleccion)
        {
            return new SelectList(_db.BancosSucursales.Where(a => a.SucursalId == sucursalId).OrderBy(a => a.Nombre), "Id", "Nombre", seleccion);
        }

        public SelectList PopulaCentroCostos(int? seleccion)
        {
            return new SelectList(_db.CentrosCostos.Where(a => a.Status == Status.Activo && a.Departamento.SucursalId == _entidadId).OrderBy(a => a.Nombre), "Id", "Nombre", seleccion);
        }

        public SelectList PopulaClientes(int? seleccion)
        {
            return new SelectList(_db.Clientes.Where(a => a.Status == Status.Activo && a.SucursalId == _entidadId).OrderBy(a => a.RazonSocial).ThenBy(a => a.Rfc), "Id", "RfcRazonSocial", seleccion);
        }

        public SelectList PopulaDepartamentos(int? seleccion)
        {
            return new SelectList(_db.Departamentos.Where(a => a.Status == Status.Activo && a.SucursalId == _entidadId).OrderBy(a => a.Nombre), "Id", "Nombre", seleccion);
        }

        public SelectList PopulaFacturasEmitidas(bool soloPpd, int clienteId, int? seleccion)
        {
            //var facturasUtilizadas = _db.DocumentosRelacionados.Where(dr => dr.FacturaEmitida.EmisorId == _entidadId && dr.FacturaEmitida.ReceptorId == clienteId && dr.Pago.ComplementoPago.Status == Status.Activo).Select(dr => dr.FacturaEmitida).ToList();
            var facturasCliente = _db.FacturasEmitidas.Where(a => a.EmisorId == _entidadId && a.ReceptorId == clienteId && a.Total > 0).ToList();
            var facturasFinales = facturasCliente.ToList();//.Except(facturasUtilizadas).OrderBy(a => a.Fecha);

            //if (soloPpd)
            //{
            //    facturasFinales = facturasFinales.Where(ff => ff.MetodoPago == c_MetodoPago.PPD).OrderBy(ff => ff.Fecha);
            //}

            return new SelectList(facturasFinales, "Id", "Desplegado", seleccion);
        }

        public SelectList PopulaGrupos(int? seleccion)
        {
            return new SelectList(_db.Grupos.Where(a => a.Status == Status.Activo).OrderBy(a => a.Nombre), "Id", "Nombre", seleccion);
        }

        public SelectList PopulaPerfiles(int? seleccion)
        {
            return new SelectList(_db.Perfiles.Where(a => a.GrupoId == _entidadId && a.Status == Status.Activo).OrderBy(a => a.Nombre), "Id", "Nombre", seleccion);
        }

        public SelectList PopulaProveedores(int? seleccion)
        {
            return new SelectList(_db.Proveedores.Where(a => a.Status == Status.Activo && a.GrupoId == _entidadId).OrderBy(a => a.RazonSocial), "Id", "RazonSocial", seleccion);
        }

        public SelectList PopulaSucursalesUsuarios(int? sucursalSeleccionada = null, int? usuarioId = null)
        {
            if (usuarioId == null)
            {
                return new SelectList(_db.Sucursales.Where(s => s.Status == Status.Activo && s.GrupoId == _entidadId).OrderBy(m => m.Nombre), "Id", "Nombre", sucursalSeleccionada);
            }
            else
            {
                var usuario = _db.Usuarios.Find(usuarioId);
                if (usuario.TodasSucursales)
                {
                    return new SelectList(_db.Sucursales.Where(s => s.Status == Status.Activo && s.GrupoId == _entidadId).OrderBy(m => m.Nombre), "Id", "Nombre", sucursalSeleccionada);
                }
                else
                {
                    return new SelectList(_db.UsuariosSucursales.Where(us => us.UsuarioId == usuarioId && us.Sucursal.Status == Status.Activo && us.Sucursal.GrupoId == _entidadId).Select(us => us.Sucursal).OrderBy(m => m.Nombre), "Id", "Nombre", sucursalSeleccionada);
                }
            }
        }

        public SelectList PopulaSucursalesProveedores(int? sucursalSeleccionada = null, int? proveedorId = null)
        {
            if (proveedorId == null)
            {
                return new SelectList(_db.Sucursales.Where(s => s.Status == Status.Activo && s.GrupoId == _entidadId).OrderBy(m => m.Nombre), "Id", "Nombre", sucursalSeleccionada);
            }
            else
            {
                var proveedor = _db.Proveedores.Find(proveedorId);
                return new SelectList(_db.ProveedoresSucursales.Where(us => us.ProveedorId == proveedorId && us.Sucursal.Status == Status.Activo && us.Sucursal.GrupoId == _entidadId).Select(us => us.Sucursal).OrderBy(m => m.Nombre), "Id", "Nombre", sucursalSeleccionada);
            }

        }

        public SelectList PopulaUsuarios(int? usuarioSeleccionado)
        {
            var usuarios = _db.UsuariosSucursales.Where(e => e.Usuario.Status == Status.Activo && e.Sucursal.GrupoId == _entidadId).Select(e => e.Usuario).ToList();
            usuarios.AddRange(_db.Usuarios.Where(u => u.GrupoId == _entidadId && u.Status == Status.Activo && u.TodasSucursales));
            return new SelectList(usuarios.Distinct().OrderBy(e => e.Nombre).ThenBy(e => e.ApellidoPaterno).ThenBy(e => e.ApellidoMaterno), "Id", "NombreCompleto", usuarioSeleccionado);
        }

        public SelectList PopulaPagos(int? complementoPagoId = null, int? pagoId = null)
        {
            return new SelectList(_db.Pagos.Where(a => a.ComplementoPagoId == complementoPagoId && a.ComplementoPago.SucursalId == _entidadId), "Id", "Desplegado", pagoId);
        }

        #endregion

    }
}