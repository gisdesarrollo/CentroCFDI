using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using API.Catalogos;
using APBox.Context;
using API.Enums;
using System;
using APBox.Control;

namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    //[Authorize(Roles = "CENTROSCOSTOS")]
    public class CentrosCostosController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();

        // GET: CentrosCostos
        public ActionResult Index()
        {
            var sucursalId = ObtenerSucursal();
            var centrosCostos = _db.CentrosCostos.Where(cc => cc.SucursalId == sucursalId).ToList();
            return View(centrosCostos);
        }

        // GET: CentrosCostos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CentroCosto centroCosto = _db.CentrosCostos.Find(id);
            if (centroCosto == null)
            {
                return HttpNotFound();
            }

            PopulaForma(centroCosto.DepartamentoId);
            return View(centroCosto);
        }

        // GET: CentrosCostos/Create
        public ActionResult Create()
        {
            PopulaForma();

            var centroCosto = new CentroCosto
            {
                Status = Status.Activo,
                SucursalId = ObtenerSucursal()
            };

            return View(centroCosto);
        }

        // POST: CentrosCostos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CentroCosto centroCosto)
        {
            PopulaForma(centroCosto.DepartamentoId);

            if (ModelState.IsValid)
            {
                _db.CentrosCostos.Add(centroCosto);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(centroCosto);
        }

        // GET: CentrosCostos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CentroCosto centroCosto = _db.CentrosCostos.Find(id);
            if (centroCosto == null)
            {
                return HttpNotFound();
            }
            PopulaForma(centroCosto.DepartamentoId);
            return View(centroCosto);
        }

        // POST: CentrosCostos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CentroCosto centroCosto)
        {
            PopulaForma(centroCosto.DepartamentoId);

            if (ModelState.IsValid)
            {
                _db.Entry(centroCosto).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(centroCosto);
        }

        // GET: CentrosCostos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CentroCosto centroCosto = _db.CentrosCostos.Find(id);
            if (centroCosto == null)
            {
                return HttpNotFound();
            }
            PopulaForma(centroCosto.DepartamentoId);
            return View(centroCosto);
        }

        // POST: CentrosCostos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CentroCosto centroCosto = _db.CentrosCostos.Find(id);
            _db.CentrosCostos.Remove(centroCosto);
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

        private void PopulaForma(int? departamentoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.DepartamentoId = popularDropDowns.PopulaDepartamentos(departamentoId);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion
    }
}
