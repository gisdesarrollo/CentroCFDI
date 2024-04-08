using APBox.Context;
using APBox.Control;
using API.Catalogos;
using API.Relaciones;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using Aplicacion.LogicaPrincipal.Correos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace APBox.Controllers.Catalogos
{
    public class AutorregistroSociosComercialesController : Controller
    {
        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();
        private readonly AcondicionarUsuarios _acondicionarUsuarios = new AcondicionarUsuarios();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();
        private readonly AcondicionarClientes _acondicionarClientes = new AcondicionarClientes();

        #endregion

        #region Metodos
        // GET: Clientes/Create
        [AllowAnonymous]
        public ActionResult Create(Guid? id)
        {
            PopulaForma();
            var grupo = _db.Grupos.FirstOrDefault(c => c.Llave == id);
            
            try
            {
                Guid llaveGrupo = ObtenerLlaveGrupo(id.Value);
            }
            catch (Exception)
            {
                string error = "Se encontró un error en el link del acceso que te compartieron, favor de contactar a tu administrador.";
                return RedirectToAction("Error", "AutorregistroSociosComerciales", new { errorMessage = error });
            }

            try
            {
                int perfilId = ObtenerPerfilId(grupo.Id);
            }
            catch (Exception)
            {
                string error = "Parece que la empresa aun no cuenta con todas las configuraciones necesarias para dar de alta proveedores, favor de consultar a tu administrador para revisar el perfil faltante.";
                return RedirectToAction("Error", "AutorregistroSociosComerciales", new { errorMessage = error });
            }

            var sucursal = _db.Sucursales.FirstOrDefault(e => e.GrupoId == grupo.Id);

            TempData["grupoId"] = grupo.Id;
            TempData["grupoNombre"] = grupo.Nombre;
            TempData["perfilId"] = ObtenerPerfilId(grupo.Id);

            var socioComercial = new SocioComercial
            {
                Status = API.Enums.Status.Activo,
                FechaAlta = DateTime.Now,
                Pais = (API.Enums.c_Pais)c_Pais.MEX,
                SucursalId = sucursal.Id,
                GrupoId = grupo.Id

            };

            ViewBag.Controller = "AutorRegistro";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "Registro de Socios Comerciales";
            ViewBag.Grupo = TempData["grupoNombre"];

            return View(socioComercial);
        }

        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SocioComercial socioComercial)
        {            
            _acondicionarClientes.CargaInicial(ref socioComercial);
            var receptor = _db.SociosComerciales.Where(c => c.Rfc == socioComercial.Rfc && c.RazonSocial == socioComercial.RazonSocial && c.SucursalId == socioComercial.SucursalId).FirstOrDefault();
            if (receptor != null)
            {
                ModelState.AddModelError("", "Error RFC o Razon Social Ya Se Encuentra Registrado!!");
                return View(socioComercial);
            }
            // Guardar datos en TempData para asignarlo a otro metodo
            socioComercial.Status = API.Enums.Status.Activo;
            

            try
            {
                _db.SociosComerciales.Add(socioComercial);
                _db.SaveChanges();
            }

            catch (Exception ex)

            {
                ModelState.AddModelError("", ex.Message);
                return View(socioComercial);
            }

            TempData["SocioComercialId"] = socioComercial.Id;
            return View("CreateUsuario");
        }

        [AllowAnonymous]
        public ActionResult CreateUsuario(Guid? id)
        {
            var grupo = _db.Grupos.FirstOrDefault(c => c.Llave == id);
            int perfilId = ObtenerPerfilId(grupo.Id);

            var usuario = new Usuario
            {   
                PerfilId = perfilId,
                Status = API.Enums.Status.Activo,
                Sucursales = new List<UsuarioSucursal>(),
                GrupoId = grupo.Id,
            };

            ViewBag.Controller = "Autoregistro";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "Registro de Socios Comerciales";

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUsuario(Usuario usuario)
        {
            var entidadExistente = _db.Usuarios.FirstOrDefault(e => e.NombreUsuario == usuario.NombreUsuario);
            if (entidadExistente != null)
            {
                ViewBag.ErrorMessage = "El nombre de usuario ya existe";
                ModelState.AddModelError("", "El nombre de usuario ya existe");
                return View(usuario);
            }

            _acondicionarUsuarios.CargaInicial(ref usuario);

            try
            {
                _operacionesUsuarios.Crear(usuario.NombreUsuario);
            }

            catch (Exception ex)
            
            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

            //Asignacion de valor si es Proveedor
            usuario.esProveedor = true;
            usuario.Departamento = null;
            usuario.Departamento_Id = null;
            usuario.GrupoId = (int)(TempData["grupoId"] as int?);
            usuario.PerfilId = (int)(TempData["perfilId"] as int?);
            usuario.Status = API.Enums.Status.Activo;

            // Obtener los datos guardados en TempData
            var ClienteId = TempData["SocioComercialId"] as int?;
            usuario.SocioComercialID = ClienteId;


            try
            {
                _db.Usuarios.Add(usuario);
                _db.SaveChanges();
            }

            catch (Exception ex)

            {
                ModelState.AddModelError("", ex.Message);
                return View(usuario);
            }

            // Envío de correo electrónico de bienvenida
            EnviarCorreoBienvenida(usuario);

            return RedirectToAction("Completado", "AutorregistroSociosComerciales");

        }

        [AllowAnonymous]
        public ActionResult Error(string errorMessage)
        {
            ViewBag.Controller = "Autoregistro";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "Error en acceso";
            ViewBag.Error = errorMessage;

            return View();
        }

        [AllowAnonymous]
        public ActionResult Completado()
        {
            ViewBag.Controller = "Autoregistro";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "Registo Completado";
            return View();
        }
        #endregion

        #region Metodos Privados
        private void EnviarCorreoBienvenida(Usuario usuario)
        {
            var sucursal = _db.Sucursales.Where(s => s.GrupoId == usuario.GrupoId).FirstOrDefault();

            _envioEmail.NotificacionNuevoUsuario(usuario, sucursal.Id);
        }
        private void PopulaClientes(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.SocioComercialID = popularDropDowns.PopulaClientes(receptorId);
        }
        private void PopulaForma(int? perfilId = null, int? grupoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), false);

            ViewBag.PerfilId = popularDropDowns.PopulaPerfiles(perfilId);

            ViewBag.GrupoId = popularDropDowns.PopulaGrupos(grupoId);

            ViewBag.SucursalId = popularDropDowns.PopulaSucursalesUsuarios(null);
        }
        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }
        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }
        private Guid ObtenerLlaveGrupo(Guid id)
        {
            var grupo = _db.Grupos.FirstOrDefault(c => c.Llave == id);
            if (grupo == null)
            {
                // Grupo no encontrado, lanzar una excepción o redirigir a la página de error
                throw new Exception("Se encontró un error en el link del acceso que te compartieron, favor de contactar a tu administrador.");
            }
            return grupo.Llave;
        }
        private int ObtenerPerfilId(int grupoId)
        {
            var perfil = _db.Perfiles.Where(p => p.GrupoId == grupoId && p.Proveedor == true ).FirstOrDefault();
            
            if (perfil == null)
            {
                // Perfil no encontrado, lanzar una excepción o redirigir a la página de error
                throw new Exception("Parece que la empresa aun no cuenta con todas las configuraciones necesarias para dar de alta proveedores, favor de consultar a tu administrador.");
            }

            return perfil.Id;

        }
        #endregion
    }

}
