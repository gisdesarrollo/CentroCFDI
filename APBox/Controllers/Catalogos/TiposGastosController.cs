
using System.Linq;
using System.Web.Mvc;
using APBox.Context;
using System;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using APBox.Control;
using Aplicacion.LogicaPrincipal.Correos;

using ApplicationUser = APBox.Models.ApplicationUser;
using API.Operaciones.ComplementoCartaPorte;
using API.Catalogos;
using System.Net;
using API.CatalogosCartaPorte;
using API.Enums;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Data.Entity;
using Utilerias.LogicaPrincipal;

namespace APBox.Controllers.Catalogos
{
    public class TiposGastosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();
        private readonly AcondicionarUsuarios _acondicionarUsuarios = new AcondicionarUsuarios();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();

        #endregion Variables





        // GET: TiposGastos
        public ActionResult Index()
        {

            var sucursal = ObtenerSucursal();
            var cat_TipoGastos = _db.TipoGastos.Where(s => s.SucursalId == sucursal).ToList();

            ViewBag.Controller = "TipoGastos";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.Title = "catalogo";

            return View(cat_TipoGastos);
        }

        // GET: TiposGastos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TiposGastos/Create
        public ActionResult Create()
        {
            var sucursal = ObtenerSucursal();
            var cat_TipoGastos = new TipoGasto();

            ViewBag.Controller = "TipoGastos";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.Title = "catalogo";

            return View(cat_TipoGastos);
        }

        // POST: TiposGastos/Create
        [HttpPost]
        public ActionResult Create(String Nombre, String cuentacontable)
        {
            try
            {

                var CuentaContable = new TipoGasto()
                {
                    Nombre = Nombre,
                    CuentaContable = cuentacontable,
                    SucursalId = ObtenerSucursal()

                };

                _db.TipoGastos.Add(CuentaContable);
                _db.SaveChanges();


            }
            catch(Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return RedirectToAction("Index");
        }


        // GET: TiposGastos/Delete/5
        public ActionResult Delete(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoGasto cat_tipogasto = _db.TipoGastos.Find(id);
            if (cat_tipogasto == null)
            {
                return HttpNotFound();
            }

            return View(cat_tipogasto);
        }

        
        public ActionResult DeleteGastos(int Id)
        {
            try
            {
                TipoGasto cat_tipogasto = _db.TipoGastos.Find(Id);
                _db.TipoGastos.Remove(cat_tipogasto);
                _db.SaveChanges();

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Index");
        }



        // GET: TiposGastos/Edit/5
        public ActionResult Edit(int id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoGasto cat_TipoGastos = _db.TipoGastos.Find(id);
            if (cat_TipoGastos == null)
            {
                return HttpNotFound();
            }

            ViewBag.Controller = "TipoGastos";
            ViewBag.Action = "Editar";
            ViewBag.ActionES = "Editar";
            ViewBag.Title = "catalogo";
            return View(cat_TipoGastos);
        }

        // POST: TiposGastos/Edit/5
        [HttpPost]
        public ActionResult Edit(int Id, String Nombre, String cuentacontable)
        {
            try
            {
                var tipogasto = new TipoGasto();

                tipogasto = _db.TipoGastos.Find(Id);

                tipogasto.Nombre = Nombre;
                tipogasto.CuentaContable = cuentacontable;
                
                _db.Entry(tipogasto).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return RedirectToAction("Index");
        }

        


        //
        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

    }
}
