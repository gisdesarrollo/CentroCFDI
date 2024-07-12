using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using API.Catalogos;
using APBox.Context;
using System;
using System.Collections.Generic;
using API.Relaciones;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using APBox.Control;
using Aplicacion.LogicaPrincipal.Correos;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using API.Context;
using APBox.Models;
using ApplicationUser = APBox.Models.ApplicationUser;

namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    public class UsuariosController : Controller
    {
        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();
        private readonly AcondicionarUsuarios _acondicionarUsuarios = new AcondicionarUsuarios();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();

        #endregion Variables

        // GET: Usuarios
        public ActionResult Index()
        {
            var grupoId = ObtenerGrupo();
            var usuarios = _db.Usuarios.Where(u => u.GrupoId == grupoId).ToList();

            ViewBag.Controller = "Usuarios";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.Title = "Usuarios";

            return View(usuarios);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            PopulaForma();
            PopulaClientes();
            PopulaDepartamento();
            var usuario = new Usuario
            {
                Status = API.Enums.Status.Activo,
                Sucursales = new List<UsuarioSucursal>(),
                GrupoId = ObtenerGrupo()
            };

            ViewBag.Controller = "Usuarios";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.Title = "Crear Usuario";

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario, bool esProveedor, int[] SucursalIds)
        {
            PopulaClientes(usuario.SocioComercialId);
            PopulaForma(usuario.PerfilId);
            PopulaDepartamento(usuario.DepartamentoId);
            if (ModelState.IsValid)
            {
                var entidadExistente = _db.Usuarios.FirstOrDefault(e => e.NombreUsuario == usuario.NombreUsuario);
                if (entidadExistente != null)
                {
                    ViewBag.ErrorMessage = "El usuario ya existe";
                    return View(usuario);
                }

                _acondicionarUsuarios.CargaInicial(ref usuario);

                if (usuario.PerfilId != null)
                {
                    var perfil = _db.Perfiles.Find(usuario.PerfilId);
                    if (perfil.Proveedor && usuario.SocioComercialId != null)
                    {
                        usuario.Departamento = null;
                        usuario.DepartamentoId = null;
                        usuario.esProveedor = true;
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "Error: Seleccione un perfil";
                    ModelState.AddModelError("", "Error: Seleccione un perfil");
                    return View(usuario);
                }


                try
                {
                    _operacionesUsuarios.Crear(usuario.NombreUsuario);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(usuario);
                }


                _db.Usuarios.Add(usuario);
                _db.SaveChanges();

                // Guardar las relaciones UsuarioSucursal
                foreach (var sucursalId in SucursalIds)
                {
                    var usuarioSucursal = new UsuarioSucursal
                    {
                        UsuarioId = usuario.Id,
                        SucursalId = sucursalId
                    };
                    _db.UsuariosSucursales.Add(usuarioSucursal);
                }
                _db.SaveChanges();
                EnviarCorreoBienvenida(usuario);
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            PopulaClientes();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {

                return HttpNotFound();
            }
            PopulaForma(usuario.PerfilId);
            PopulaDepartamento(usuario.DepartamentoId);

            usuario.DepartamentoId = usuario.DepartamentoId;

            ViewBag.Controller = "Usuarios";
            ViewBag.Action = "Edit";
            ViewBag.ActionES = "Editar";
            ViewBag.Title = "sistema";
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario, bool esProveedor, int[] SucursalIds)
        {
            PopulaClientes(usuario.SocioComercialId);
            PopulaForma(usuario.PerfilId);
            PopulaDepartamento(usuario.DepartamentoId);

            if (ModelState.IsValid)
            {
                var usuarioAnterior = _db.Usuarios.Find(usuario.Id);
                string username = usuarioAnterior.NombreUsuario;
                if (username != usuario.NombreUsuario)
                {
                    var entidadExistente = _db.Usuarios.FirstOrDefault(e => e.NombreUsuario == usuario.NombreUsuario);
                    if (entidadExistente != null)
                    {
                        ModelState.AddModelError("", "Ese usuario ya existe");
                        return View(usuario);
                    }
                }


                //Asignacion de valor si es Proveedor
                if (usuario.PerfilId != null)
                {
                    var perfil = _db.Perfiles.Find(usuario.PerfilId);
                    if (perfil.Proveedor && usuario.SocioComercialId != null)
                    {
                        usuario.esProveedor = true;
                        usuario.Departamento = null;
                        usuario.DepartamentoId = null;
                    }
                    else { usuario.esProveedor = false; }
                }
                else
                {

                    ViewBag.ErrorMessage = "Error: seleccione un perfil";
                    ModelState.AddModelError("", "Error: seleccione un perfil");
                    return View(usuario);
                }

                usuario.Status = API.Enums.Status.Activo;
                _acondicionarUsuarios.Sucursales(usuario);
                _db.Entry(usuarioAnterior).CurrentValues.SetValues(usuario);
                _db.Entry(usuarioAnterior).State = EntityState.Modified;
                _db.SaveChanges();

                // Guardar las relaciones UsuarioSucursal
                foreach (var sucursalId in SucursalIds)
                {
                    var usuarioSucursal = new UsuarioSucursal
                    {
                        UsuarioId = usuario.Id,
                        SucursalId = sucursalId
                    };
                    _db.UsuariosSucursales.Add(usuarioSucursal);
                }
                _db.SaveChanges();

                if (username != usuario.NombreUsuario)
                {
                    _operacionesUsuarios.ReseteoUsername(username, usuario.NombreUsuario);
                }
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            PopulaForma(usuario.PerfilId);
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = _db.Usuarios.Find(id);
            _db.Usuarios.Remove(usuario);
            _db.SaveChanges();
            //eliminar de aspnetusers
            _operacionesUsuarios.EliminarUsuario(usuario.NombreUsuario);
            return RedirectToAction("Index");
        }

        public ActionResult Reseteo(int id)
        {
            var usuario = _db.Usuarios.Find(id);
            _operacionesUsuarios.Reseteo(usuario.NombreUsuario);
            EnviarCorreoBienvenida(usuario);
            return RedirectToAction("Index");
        }
        public ActionResult ResetPassword(int id)
        {
            ViewBag.Controller = "Usuarios";
            ViewBag.Action = "ResetPassword";
            ViewBag.ActionES = "Resetear Password";
            ViewBag.Title = "sistema";
            var usuario = _db.Usuarios.Find(id);
            return View(usuario);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(Usuario usuarioReset)
        {
            ViewBag.Controller = "Usuarios";
            ViewBag.Action = "ResetPassword";
            ViewBag.ActionES = "Resetear Password";
            ViewBag.Title = "sistema";
            if (usuarioReset.PasswordAnterior != null || usuarioReset.PasswordNuevo != null)
            {
                var usuario = _db.Usuarios.Find(usuarioReset.Id);
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = userManager.FindByName(usuario.NombreUsuario);
                bool esSucccessPassword = VerifyPassword(user.PasswordHash, usuarioReset.PasswordAnterior);
                if (!esSucccessPassword)
                {
                    ViewBag.ErrorMessage = "Error: la contraseña anterior es incorrecta";
                    return View(usuarioReset);
                }
                var result = await userManager.ChangePasswordAsync(user.Id, usuarioReset.PasswordAnterior, usuarioReset.PasswordNuevo);
                if (!result.Succeeded)
                {
                    ViewBag.ErrorMessage = "Error: no se pudo cambiar la contraseña";
                    return View(usuarioReset);
                }
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorMessage = "Error: los dos campos son requeridos";
            return View(usuarioReset);
        }
        public bool VerifyPassword(string passwordHash, string plainPassword)
        {
            var passwordHasher = new PasswordHasher();
            var result = passwordHasher.VerifyHashedPassword(passwordHash, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region PopulaForma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private void PopulaForma(int? perfilId = null, int? grupoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), false);

            ViewBag.PerfilId = popularDropDowns.PopulaPerfiles(perfilId);
            ViewBag.GrupoId = popularDropDowns.PopulaGrupos(grupoId);

            ViewBag.SucursalId = popularDropDowns.PopulaSucursalesUsuarios(null);
        }

        //DropDown Socios Comerciales
        private void PopulaClientes(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.SocioComerciales = popularDropDowns.PopulaClientes(receptorId);
        }

        //DropDown Departamentos
        private void PopulaDepartamento(int? DepartamentoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.DepartamentoId = popularDropDowns.PopulaDepartamentos(DepartamentoId);
        }

        private void EnviarCorreoBienvenida(Usuario usuario)
        {
            //corregir este método, ya que cuando es un buzón que no existe genera error.
            _envioEmail.NotificacionNuevoUsuario(usuario, (int)ObtenerSucursal());
        }

        #endregion
    }
}

