using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using API.Catalogos;
using APBox.Context;
using System;
using APBox.Control;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using API.Enums;

namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    //[Authorize(Roles = "PROVEEDORES")]
    public class ProveedoresController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();
        private readonly AcondicionarProveedores _acondicionarProveedor = new AcondicionarProveedores();

        #endregion

        // GET: Proveedores
        public ActionResult Index()
        {
            var grupoId = ObtenerGrupo();
            var proveedores = _db.Proveedores.Where(p => p.GrupoId == grupoId).ToList();
            return View(proveedores);
        }

        // GET: Proveedores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = _db.Proveedores.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }

            PopulaForma();
            return View(proveedor);
        }

        // GET: Proveedores/Create
        public ActionResult Create()
        {
            var proveedor = new Proveedor
            {
                FechaAlta = DateTime.Now,
                Pais = "MEXICO",
                Status = Status.Activo,
            };

            PopulaForma();

            return View(proveedor);
        }

        // POST: Proveedores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proveedor proveedor)
        {
            PopulaForma();

            if (ModelState.IsValid)
            {
                _acondicionarProveedor.CargaInicial(ref proveedor);

                try
                {
                    _operacionesUsuarios.Crear(proveedor.Rfc);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(proveedor);
                }

                _db.Proveedores.Add(proveedor);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(proveedor);
        }

        // GET: Proveedores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = _db.Proveedores.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }

            PopulaForma();
            return View(proveedor);
        }

        // POST: Proveedores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Proveedor proveedor)
        {
            PopulaForma();

            if (ModelState.IsValid)
            {
                _acondicionarProveedor.Sucursales(proveedor);

                _db.Entry(proveedor).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(proveedor);
        }

        // GET: Proveedores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = _db.Proveedores.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }

            PopulaForma();
            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Proveedor proveedor = _db.Proveedores.Find(id);
            _db.Proveedores.Remove(proveedor);
            _db.SaveChanges();
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

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private void PopulaForma(int? sucursalId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), false);

            ViewBag.SucursalId = popularDropDowns.PopulaSucursalesUsuarios(null);
        }

        #endregion
    }
}
