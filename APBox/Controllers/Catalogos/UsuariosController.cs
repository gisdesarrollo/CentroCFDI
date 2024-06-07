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
using System.Net.Mail;
using Aplicacion.LogicaPrincipal.Correos;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

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
        public ActionResult Create(Usuario usuario, bool esProveedor)
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
        public ActionResult Edit(Usuario usuario, bool esProveedor)
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
                            usuario.Departamento = null;
                            usuario.DepartamentoId = null;
                        }
                        
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error: seleccione un perfil";
                        ModelState.AddModelError("", "Error: seleccione un perfil");
                        return View(usuario);
                    }
                

                _acondicionarUsuarios.Sucursales(usuario);
                _db.Entry(usuarioAnterior).CurrentValues.SetValues(usuario);
                _db.Entry(usuarioAnterior).State = EntityState.Modified;
                _db.SaveChanges();
                if (username != usuario.NombreUsuario)
                {
                    _operacionesUsuarios.ReseteoUsername(username,usuario.NombreUsuario);
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