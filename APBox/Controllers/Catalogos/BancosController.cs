using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using APBox.Context;
using API.Catalogos;
using API.Enums;

namespace APBox.Controllers.Catalogos
{
    [APBox.Control.SessionExpire]
    public class BancosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        // GET: Bancos
        public ActionResult Index()
        {
            var bancos = _db.Bancos.ToList();
            return View(bancos);
        }

        // GET: Bancos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banco banco = _db.Bancos.Find(id);
            if (banco == null)
            {
                return HttpNotFound();
            }
            return View(banco);
        }

        // GET: Bancos/Create
        public ActionResult Create()
        {
            var banco = new Banco
            {
                Status = Status.Activo
            };

            return View(banco);
        }

        // POST: Bancos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Banco banco)
        {
            if (ModelState.IsValid)
            {
                _db.Bancos.Add(banco);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(banco);
        }

        // GET: Bancos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banco banco = _db.Bancos.Find(id);
            if (banco == null)
            {
                return HttpNotFound();
            }
            return View(banco);
        }

        // POST: Bancos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Banco banco)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(banco).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(banco);
        }

        // GET: Bancos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banco banco = _db.Bancos.Find(id);
            if (banco == null)
            {
                return HttpNotFound();
            }
            return View(banco);
        }

        // POST: Bancos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Banco banco = _db.Bancos.Find(id);
            _db.Bancos.Remove(banco);
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
    }
}
