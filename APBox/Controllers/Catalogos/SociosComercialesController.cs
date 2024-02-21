using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using APBox.Context;
using API.Catalogos;
using System;
using APBox.Control;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;

namespace APBox.Controllers.Catalogos
{
    [APBox.Control.SessionExpire]
    public class SociosComercialesController : Controller
    {
        
        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly AcondicionarClientes _acondicionarClientes = new AcondicionarClientes();

        #endregion

       
        // GET: Clientes
        public ActionResult Index()
        {
            var sucursalId = ObtenerSucursal();
            var clientes = _db.SociosComerciales.Where(c => c.SucursalId == sucursalId).ToList();


            ViewBag.Controller = "SociosComerciales";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.NameHere = "Crea y modifica Socios Comerciales";

            return View(clientes);
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocioComercial cliente = _db.SociosComerciales.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            PopulaForma();
            return View(cliente);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            PopulaForma();

            var sociocomercial = new SocioComercial
            {
                Status = API.Enums.Status.Activo,
                FechaAlta = DateTime.Now,
                Pais = (API.Enums.c_Pais)c_Pais.MEX,
                SucursalId = ObtenerSucursal()
            };

            ViewBag.Controller = "SociosComerciales";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "catalogo";

            return View(sociocomercial);
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SocioComercial sociocomercial)
        {
            ModelState.Remove("Banco.Id");
            ModelState.Remove("Banco.Nombre");
            ModelState.Remove("Banco.NumeroCuenta");
            PopulaForma();
            if (ModelState.IsValid)
            {
                _acondicionarClientes.CargaInicial(ref sociocomercial);
                var receptor = _db.SociosComerciales.Where(c=> c.Rfc == sociocomercial.Rfc && c.RazonSocial == sociocomercial.RazonSocial && c.SucursalId == sociocomercial.SucursalId).FirstOrDefault();
                if(receptor != null)
                {
                    ModelState.AddModelError("", "Error RFC o Razon Social Ya Se Encuentra Registrado!!");
                    return View(sociocomercial);
                }
                _db.SociosComerciales.Add(sociocomercial);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sociocomercial);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocioComercial sociocomercial = _db.SociosComerciales.Find(id);
            if (sociocomercial == null)
            {
                return HttpNotFound();
            }

            PopulaForma();
            ViewBag.Controller = "SociosComerciales";
            ViewBag.Action = "Edit";
            ViewBag.ActionES = "Editar";
            ViewBag.NameHere = "catalogo";
            return View(sociocomercial);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SocioComercial sociocomercial)
        {
            ModelState.Remove("Banco.Id");
            ModelState.Remove("Banco.Nombre");
            ModelState.Remove("Banco.NumeroCuenta");

            if (ModelState.IsValid)
            {
                _acondicionarClientes.Bancos(sociocomercial);

                _db.Entry(sociocomercial).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulaForma();
            return View(sociocomercial);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocioComercial cliente = _db.SociosComerciales.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            PopulaForma();
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SocioComercial cliente = _db.SociosComerciales.Find(id);
            _db.SociosComerciales.Remove(cliente);
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

            ViewBag.BancoId = popularDropDowns.PopulaBancos(null);
        }

        #endregion

    }
}
