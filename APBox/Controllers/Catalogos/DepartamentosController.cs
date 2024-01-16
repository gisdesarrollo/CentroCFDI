using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using APBox.Context;
using API.Catalogos;

namespace APBox.Controllers.Catalogos
{
    [APBox.Control.SessionExpire]
    public class DepartamentosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        // GET: Departamentos
        public ActionResult Index()
        {
            ViewBag.Controller = "Departamentos";
            ViewBag.Action = "Index";
            ViewBag.Button = "Departamentos";
            ViewBag.NameHere = "sistema";
            var sucursalId = ObtenerSucursal();
            var departamentos = _db.Departamentos.Where(d => d.SucursalId == sucursalId).ToList();
            return View(departamentos);
        }


        // GET: Departamentos/Create
        public ActionResult Create()
        {
            ViewBag.Controller = "Departamentos";
            ViewBag.Action = "Create";
            ViewBag.NameHere = "sistema";
            var departamento = new Departamento
            {
                SucursalId = ObtenerSucursal()
            };

            return View(departamento);
        }

        // POST: Departamentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                _db.Departamentos.Add(departamento);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(departamento);
        }

        // GET: Departamentos/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Controller = "Departamentos";
            ViewBag.Action = "Edit";
            ViewBag.NameHere = "sistema";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamento departamento = _db.Departamentos.Find(id);
            if (departamento == null)
            {
                return HttpNotFound();
            }
            return View(departamento);
        }

        // POST: Departamentos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(departamento).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(departamento);
        }

        
        // POST: Departamentos/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            Departamento departamento = _db.Departamentos.Find(id);
            _db.Departamentos.Remove(departamento);
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

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion
    }
}
