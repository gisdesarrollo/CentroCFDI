using APBox.Context;
using APBox.Control;
using API.Catalogos;
using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using System;
using System.Linq;
using System.Web.Mvc;


namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    public class ProyectosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion        

        // GET: Proyectos
        public ActionResult Index()
        {
            ViewBag.Controller = "Proyectos";
            ViewBag.ActionES = "Index";
            ViewBag.Title = "Proyectos";

            var sucursalId = ObtenerSucursal();
            var proyectos = _db.Proyectos.Where(p => p.SucursalId == sucursalId).ToList();

            return View(proyectos);
        }

        // GET: Proyectos/Create
        public ActionResult Create()
        {
            ViewBag.Controller = "Proyectos";
            ViewBag.Action = "Crear";
            ViewBag.Title = "Proyectos";

            var sucursalId = ObtenerSucursal();

            var proyecto = new Proyecto
            {
                Estatus = API.Enums.c_Estatus.Activo,
                SucursalId = sucursalId,
                FechaCreacion = DateTime.Now,
            };

            return View(proyecto);
        }

        // POST: Proyectos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Proyectos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Proyectos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Proyectos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Proyectos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private int ObtenerUsuario()
        {
            return Convert.ToInt32(Session["UsuarioId"]);
        }
    }
}
