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

namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    //[Authorize(Roles = "USUARIOS")]
    public class UsuariosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();
        private readonly AcondicionarUsuarios _acondicionarUsuarios = new AcondicionarUsuarios();

        #endregion

        // GET: Usuarios
        public ActionResult Index()
        {
            var grupoId = ObtenerGrupo();
            var usuarios = _db.Usuarios.Where(u => u.GrupoId == grupoId).ToList();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
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

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            PopulaForma();

            var usuario = new Usuario
            {
                Status = API.Enums.Status.Activo,
                Sucursales = new List<UsuarioSucursal>(),
                GrupoId = ObtenerGrupo()
            };

            return View(usuario);
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            PopulaForma(usuario.PerfilId);
            if (ModelState.IsValid)
            {
                var entidadExistente = _db.Usuarios.FirstOrDefault(e => e.Nombre == usuario.Nombre && e.ApellidoPaterno == usuario.ApellidoPaterno && e.ApellidoMaterno == usuario.ApellidoMaterno);
                if (entidadExistente != null)
                {
                    ModelState.AddModelError("", "Ese usuario ya existe");
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

                _db.Usuarios.Add(usuario);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario)
        {
            PopulaForma(usuario.PerfilId);
            if (ModelState.IsValid)
            {
                var entidadExistente = _db.Usuarios.FirstOrDefault(e => e.Nombre == usuario.Nombre && e.ApellidoPaterno == usuario.ApellidoPaterno && e.ApellidoMaterno == usuario.ApellidoMaterno && e.Id != usuario.Id);
                if (entidadExistente != null)
                {
                    ModelState.AddModelError("", "Ese usuario ya existe");
                    return View(usuario);
                }

                _acondicionarUsuarios.Sucursales(usuario);
                _db.Entry(usuario).State = EntityState.Modified;
                _db.SaveChanges();
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = _db.Usuarios.Find(id);
            _db.Usuarios.Remove(usuario);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Reseteo(int id)
        {
            var personal = _db.Usuarios.Find(id);
            _operacionesUsuarios.Reseteo(personal.NombreUsuario);
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

        #endregion
    }
}
