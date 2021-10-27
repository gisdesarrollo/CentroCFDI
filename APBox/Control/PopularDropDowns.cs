using System.Linq;
using System.Web.Mvc;
using API.Enums;
using APBox.Context;
using System;
using CFDI.API.Enums.CFDI33;
using System.Collections.Generic;
using API.CatalogosCartaPorte.Domicilio;
using API.Catalogos;

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

        public List<(String RFC, String RazonSocial, String Pais)> PopulaDatosCliente(int seleccion)
        { 
                var result=_db.Clientes.Where(a => a.Id == seleccion).Select(a => new { a.Rfc, a.RazonSocial,a.Pais}).ToList();
                return result.Select(r => (r.Rfc, r.RazonSocial, r.Pais.ToString())).ToList();
        }
        public List<Colonia> PopulaColonias(string seleccion)
        {
            return _db.Colonias.Where(a => a.c_CodigoPostal_Id == seleccion).ToList();
        }

        public List<Localidad> PopulaLocalidades(string seleccion)
        {
            return _db.Localidades.Where(a => a.c_Estado_Id == seleccion).ToList();
        }
        public List<Municipio> PopulaMunicipios(string seleccion)
        {
            return _db.Municipios.Where(a => a.c_Estado_Id == seleccion).ToList();
        }
        public List<Estado> PopulaEstados(string seleccion)
        {
            return _db.Estados.Where(a => a.c_Pais_Id == seleccion).ToList();
 
        }
        public SelectList PopulaPaises()
        {
            return new SelectList(_db.Paises.OrderBy(a => a.c_Pais), "c_Pais","Descripcion");
        }

        public SelectList PopularUsocfdi()
        {
            return new SelectList(_db.UsoCfdis.OrderBy(u => u.C_UsoCfdi), "C_UsoCfdi", "descripcion");
        }
       
        public SelectList PopulaDatosEstaciones(string seleccion)
        {
            return new SelectList(_db.Estaciones.Where(a => a.ClaveTransporte_Id == seleccion).OrderBy(a=>a.Descripcion), "ClaveIdentificacion", "Descripcion");
        }
        public SelectList PopulaDatosSucursal(int? seleccion)
        {
            return new SelectList(_db.Sucursales.Where(a => a.Id == seleccion).ToList());
        }
        public SelectList PopulaTiposEstacion()
        {
            return new SelectList(_db.TipoEstaciones.OrderBy(a => a.Descripcion), "ClaveEstacion", "Descripcion");
        }
        public SelectList PopulaTipoDeComprobante()
        {
            return new SelectList(Enum.GetValues(typeof(c_TipoDeComprobante))
                        .Cast<c_TipoDeComprobante>()
                        .Where(e => (e == c_TipoDeComprobante.I || e == c_TipoDeComprobante.T)));
        }

        public SelectList PopulaTransporte()
        {
            return new SelectList(_db.CveTransportes.OrderBy(a => a.c_ClaveUnidad), "c_ClaveUnidad", "Descripcion");
        }



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