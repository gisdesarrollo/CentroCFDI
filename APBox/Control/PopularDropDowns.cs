using System.Linq;
using System.Web.Mvc;
using API.Enums;
using APBox.Context;
using System;
using System.Collections.Generic;
using API.CatalogosCartaPorte.Domicilio;
using API.Catalogos;
using API.CatalogosCartaPorte;
using API.Operaciones.ComplementoCartaPorte;

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
        public List<ClaveUnidad> PopulaDatosClaveUnidad(string seleccion)
        {
            return _db.ClavesUnidad.Where(a => a.c_ClaveUnidad == seleccion).ToList();
        }
        public List<ClaveProdServCP> PopulaDatosClaveProdCP(string seleccion)
        {
            return _db.ClavesProdServCP.Where(a => a.c_ClaveUnidad == seleccion).ToList();
        }
        public List<FraccionArancelaria> PopulaDatosFraccionArancelaria(string seleccion)
        {
            return _db.FraccionesArancelarias.Where(a => a.c_FraccionArancelaria == seleccion).ToList();
        }
        /*public List<API.Operaciones.ComplementoCartaPorte.Cat_Conceptos> PopulaDatosConceptos(int seleccion)
        {
            //_db.Configuration.ProxyCreationEnabled = false;
            return _db.Cat_Conceptos.Where(a => a.Id == seleccion).ToList();
        }
        public List<API.Operaciones.ComplementoCartaPorte.Cat_SubImpuestoC> PopulaDatosImpuestos(int seleccion)
        {
           // _db.Configuration.ProxyCreationEnabled = false;
            return _db.Cat_Impuestos.Where(a => a.Id == seleccion).ToList();
        }*/
        public List<Cat_Conceptos> PopulaCatConceptos(int claveProd)
        {
            _db.Configuration.ProxyCreationEnabled = false;
            return _db.Cat_Conceptos.Where(a => a.Id == claveProd).ToList();
        }
        public List<Cat_SubImpuestoC> PopulaCatImpuestos(int IdImpuesto)
        {
            _db.Configuration.ProxyCreationEnabled = false;
            return _db.Cat_Impuestos.Where(a => a.Id == IdImpuesto).ToList();
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

        public SelectList PopulaConceptos(int sucursalId)
        {
            var DatosConcatenados = _db.Cat_Conceptos.Where(a => a.SucursalId == sucursalId).ToDictionary(a => a.Id, a => (a.NoIdentificacion + "-" + a.Descripcion));
            return new SelectList(DatosConcatenados, "Key", "Value");

            // return new SelectList(_db.Cat_Conceptos.OrderByDescending(a => a.Id), "Id", "ClavesProdServ");
        }

        public SelectList PopulaImpuestoT()
        {
            return new SelectList(_db.Cat_Impuestos.Where(a => a.TipoImpuesto == "Traslado"), "Id", "TasaOCuota");
        }

        public SelectList PopulaImpuestoR()
        {
            return new SelectList(_db.Cat_Impuestos.Where(a => a.TipoImpuesto == "Retencion"), "Id", "TasaOCuota");
        }
        public SelectList PopulaPaises()
        {
            var concat = _db.Paises.OrderBy(a => a.c_Pais).ToDictionary(a => a.c_Pais, a => a.c_Pais + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }

        public SelectList PopulaImpuestoSat()
        {
            return new SelectList(_db.ImpuestoCP.OrderBy(a => a.c_Impuesto), "c_Impuesto", "Descripcion");
        }
        public SelectList PopulaFormaPagoFiltro(string seleccion)
        {
            return new SelectList(_db.Cat_FormaPago.OrderBy(a => a.c_FormaPago), "c_FormaPago", "Descripcion",seleccion);
        }
        public SelectList PopulaFormaPago()
        {
            var concat = _db.Cat_FormaPago.OrderBy(a => a.c_FormaPago).ToDictionary(a => a.c_FormaPago, a => a.c_FormaPago + "-" + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList PopulaClaveUnidad()
        {
            return new SelectList(_db.ClavesUnidad.OrderBy(a => a.c_ClaveUnidad), "c_ClaveUnidad", "Nombre");
        }
        public SelectList PopulaClaveProdServ()
        {
            return new SelectList(_db.ClavesProdServCP.OrderBy(a => a.c_ClaveUnidad), "c_ClaveUnidad", "Descripcion");
        }

        public SelectList PopularUsocfdi()
        {
            return new SelectList(_db.UsoCfdis.OrderBy(u => u.C_UsoCfdi), "C_UsoCfdi", "descripcion");
        }
       
        public SelectList PopulaDatosEstaciones(string seleccion)
        {
            var concat = _db.Estaciones.Where(a => a.ClaveTransporte_Id == seleccion).OrderBy(a => a.Descripcion).ToDictionary(a => a.ClaveIdentificacion, a => a.ClaveIdentificacion + "-" + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList PopulaDatosSucursal(int? seleccion)
        {
            return new SelectList(_db.Sucursales.Where(a => a.Id == seleccion).ToList());
        }
        public SelectList PopulaTiposEstacion()
        {
            return new SelectList(_db.TipoEstaciones.OrderBy(a => a.Descripcion), "ClaveEstacion", "Descripcion");
        }
        public SelectList PopulaDerechodePaso()
        {
              var concat = _db.DerechosDePasos.OrderBy(a => a.ClavederechoPaso).ToDictionary(a => a.ClavederechoPaso, a=>a.DerechoDePaso+" "+a.Concesionario);
            return new SelectList(concat,"key","value");
         }
        public SelectList PopulaTipoDeComprobanteFiltro(c_TipoDeComprobante seleccion)
        {
            return new SelectList(Enum.GetValues(typeof(c_TipoDeComprobante))
                        .Cast<c_TipoDeComprobante>()
                        .Where(e => (e == c_TipoDeComprobante.I || e == c_TipoDeComprobante.T)), seleccion);
        }
        public SelectList PopulaTipoDeComprobante()
        {
            return new SelectList(Enum.GetValues(typeof(c_TipoDeComprobante))
                        .Cast<c_TipoDeComprobante>()
                        .Where(e => (e == c_TipoDeComprobante.I || e == c_TipoDeComprobante.T)));
        }

        public SelectList PopulaTransporte()
        {
            var concat = _db.CveTransportes.OrderBy(a => a.c_ClaveUnidad).ToDictionary(a => a.c_ClaveUnidad, a => a.c_ClaveUnidad + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }

        public SelectList PopulaTipoPermiso()
        {
            return new SelectList(_db.TipoPermisos.OrderBy(a => a.Clave), "Clave", "Descripcion");
        }
        //evelio dropdow
        public SelectList PopulaClaveProdSTCC()
        {
            return new SelectList(_db.ClavesProdSTCC.OrderBy(a => a.ClaveSTCC), "ClaveSTCC", "Descripcion");
        }
        public SelectList PopulaClaveUnida_Id()
        {
            return new SelectList(_db.ClavesUnidad.OrderBy(a => a.c_ClaveUnidad), "c_ClaveUnidad", "Nombre"); 
        }

        public SelectList PopulaMaterialPeligroso_Id()
        {
            return new SelectList(_db.MaterialesPeligrosos.OrderBy(a => a.ClaveMaterialPeligroso), "ClaveMaterialPeligroso", "Descripcion");
        }
        public SelectList TipoEmbalaje_Id()
        {
            return new SelectList(_db.TipoEmbalajes.OrderBy(a => a.ClaveAsignacion), "ClaveAsignacion", "Descripcion");
        }

        public SelectList PopulaFraccionArancelaria_Id()
        {
            return new SelectList(_db.FraccionesArancelarias.OrderBy(a => a.c_FraccionArancelaria), "c_FraccionArancelaria", "Descripcion");
        }

        public SelectList ClaveUnidadPeso_Id()
        {
            var concat = _db.ClavesUnidadPeso.OrderBy(a => a.ClaveUnidad).ToDictionary(a => a.ClaveUnidad, a => a.ClaveUnidad + " - " + a.Nombre);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList SubTipoRem_Id()
        {
            var concat = _db.SubTipoRems.OrderBy(a => a.ClaveTipoRemolque).ToDictionary(a => a.ClaveTipoRemolque, a => a.ClaveTipoRemolque + " - " + a.Remolque);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList ConfigMaritima_Id()
        {
            var concat = _db.ConfigMaritimas.OrderBy(a => a.c_ClaveUnidad).ToDictionary(a => a.c_ClaveUnidad, a => a.c_ClaveUnidad + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }

        public SelectList TipoPermiso_Id(string seleccion)
        {
            var concat = _db.TipoPermisos.Where(a => a.Nota == seleccion).OrderBy(a => a.Descripcion).ToDictionary(a => a.Clave, a => a.Clave + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList ConfigAutotransporte_Id()
        {
            var concat = _db.ConfigAutotransportes.OrderBy(a => a.Descripcion).ToDictionary(a => a.c_ClaveNomeclatura, a => a.c_ClaveNomeclatura + " - " + a.Descripcion);
            return new SelectList(concat, "key", "value");
        }
        public SelectList ClaveTipoCarga_Id()
        {
            var concat = _db.ClavesTipoCarga.OrderBy(a => a.ClaveTipocarga).ToDictionary(a => a.ClaveTipocarga, a => a.ClaveTipocarga + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList NumAutorizacionNaviero_Id()
        {
            return new SelectList(_db.NumAutorizacionNavieros.OrderBy(a => a.NumeroAutorizacion), "NumeroAutorizacion", "NumeroAutorizacion");
        }
        public SelectList ContenedorMaritimo_Id()
        {
            var concat = _db.ContenedoresMaritimos.OrderBy(a => a.ClaveContenedorMaritimo).ToDictionary(a => a.ClaveContenedorMaritimo, a => a.ClaveContenedorMaritimo + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList CodigoTransporteAereo_Id()
        {
            var concat = _db.CodigosTransporteAereo.OrderBy(a => a.ClaveIdentificacion).ToDictionary(a => a.ClaveIdentificacion, a => a.Designador + " - " + a.NombreAreolinea);
            return new SelectList(concat, "key", "value");
        }
        public SelectList TipoDeServicio_Id()
        {
            var concat = _db.TipoDeServicios.OrderBy(a => a.c_ClaveUnidad).ToDictionary(a => a.c_ClaveUnidad, a => a.c_ClaveUnidad + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList TipoCarro_Id()
        {
            var concat = _db.TipoCarros.OrderBy(a => a.TipoDeCarro).ToDictionary(a => a.Clave, a => a.Clave + " - " + a.TipoDeCarro);
            return new SelectList(concat, "key", "Value");
        }
        public SelectList Contenedor_Id()
        {
            var concat = _db.Contenedores.OrderBy(a => a.Clave).ToDictionary(a => a.Clave, a => a.Clave + " - " + a.Descripcion);
            return new SelectList(concat, "key", "Value");
        }

        //

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