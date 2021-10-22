using APBox.Context;
using APBox.Control;
using API.Operaciones.ComplementosPagos;
using CFDI.API.Enums.CFDI33;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace APBox.Controllers.PortalProveedores
{
    [APBox.Control.SessionExpire]
    public class ComplementosPagosProveedoresController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        // GET: Facturas
        public ActionResult Index()
        {
            var sucursalId = ObtenerSucursal();
            var complementosPago = _db.Pagos.Where(p => p.SucursalId == sucursalId).ToList();
            return View(complementosPago);
        }

        // GET: ComplementosPago/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago complementoPago = _db.Pagos.Find(id);
            if (complementoPago == null)
            {
                return HttpNotFound();
            }
            PopulaForma();
            return View(complementoPago);
        }

        // GET: ComplementosPago/Create
        public ActionResult Create()
        {
            var complementoPago = new Pago
            {
                FechaPago = DateTime.Now,
                Moneda = c_Moneda.MXN,
                TipoCambio = 1,
                SucursalId = ObtenerSucursal()
            };

            PopulaForma();
            return View(complementoPago);
        }

        // POST: ComplementosPago/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago complementoPago)
        {
            PopulaForma();

            if (ModelState.IsValid)
            {
                _db.Pagos.Add(complementoPago);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(complementoPago);
        }

        // GET: ComplementosPago/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago complementoPago = _db.Pagos.Find(id);

            if (complementoPago == null)
            {
                return HttpNotFound();
            }
            PopulaForma();
            return View(complementoPago);
        }

        // POST: ComplementosPago/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pago complementoPago)
        {
            PopulaForma();

            if (ModelState.IsValid)
            {
                _db.Entry(complementoPago).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(complementoPago);
        }

        // GET: ComplementosPago/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago complementoPago = _db.Pagos.Find(id);
            if (complementoPago == null)
            {
                return HttpNotFound();
            }
            PopulaForma();
            return View(complementoPago);
        }

        // POST: ComplementosPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pago complementoPago = _db.Pagos.Find(id);
            _db.Pagos.Remove(complementoPago);
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

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private void PopulaForma()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), true);
            ViewBag.ReceptorId = popularDropDowns.PopulaSucursalesUsuarios(null);


            //TODO: Hardcodeado
            ViewBag.FacturaId = popularDropDowns.PopulaFacturasEmitidas(true, 1, null);

        }

        #endregion

    }
}
