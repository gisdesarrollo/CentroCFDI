using APBox.Context;
using API.Operaciones.Facturacion;
using CFDI.API.Enums.CFDI33;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace APBox.Controllers.Operaciones
{
    public class DocumentosExtranjerosController : Controller
    {
        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        // GET: DocumentosExtranjeros
        public ActionResult Index()
        {
            var documentosExtranjeros = _db.DocumentosExtranjeros.ToList();
            return View(documentosExtranjeros);
        }

        // GET: DocumentosExtranjeros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentoExtranjero documentoExtranjero = _db.DocumentosExtranjeros.Find(id);
            if (documentoExtranjero == null)
            {
                return HttpNotFound();
            }
            return View(documentoExtranjero);
        }

        // GET: DocumentosExtranjeros/Create
        public ActionResult Create()
        {
            var documentoExtranjero = new DocumentoExtranjero
            {
                UsuarioId = ObtenerUsuario(),
                Fecha = DateTime.Now,
                Moneda = c_Moneda.USD,
                TipoCambio = 1,
                ReceptorId = ObtenerSucursal(),
                TipoGasto = API.Enums.TiposGastos.Personal
            };

            return View(documentoExtranjero);
        }

        // POST: DocumentosExtranjeros/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DocumentoExtranjero documentoExtranjero)
        {
            if (ModelState.IsValid)
            {
                _db.DocumentosExtranjeros.Add(documentoExtranjero);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(documentoExtranjero);
        }

        // GET: DocumentosExtranjeros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentoExtranjero documentoExtranjero = _db.DocumentosExtranjeros.Find(id);
            if (documentoExtranjero == null)
            {
                return HttpNotFound();
            }
            return View(documentoExtranjero);
        }

        // POST: DocumentosExtranjeros/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DocumentoExtranjero documentoExtranjero)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(documentoExtranjero).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(documentoExtranjero);
        }

        // GET: DocumentosExtranjeros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentoExtranjero documentoExtranjero = _db.DocumentosExtranjeros.Find(id);
            if (documentoExtranjero == null)
            {
                return HttpNotFound();
            }
            return View(documentoExtranjero);
        }

        // POST: DocumentosExtranjeros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentoExtranjero documentoExtranjero = _db.DocumentosExtranjeros.Find(id);
            _db.DocumentosExtranjeros.Remove(documentoExtranjero);
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

        #region Popula Forma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private int ObtenerUsuario()
        {
            return Convert.ToInt32(Session["UsuarioId"]);
        }

        #endregion

    }
}