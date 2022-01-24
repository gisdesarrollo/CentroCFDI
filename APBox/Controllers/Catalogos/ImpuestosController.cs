using APBox.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using API.Operaciones.ComplementoCartaPorte;
using CFDI.API.Enums.CFDI33;
using System.Data.Entity;
using APBox.Control;

namespace APBox.Controllers.Catalogos
{
    public class ImpuestosController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();
        // GET api/<controller>
        public ActionResult Index()
        {
            var sucursal = ObtenerSucursal();
            var cat_impuestos = _db.Cat_Impuestos.Where(s => s.SucursalId  == sucursal).ToList();
            return View(cat_impuestos);
        }
        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }
        
        // Eliminar Impuesto 
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Cat_SubImpuestoC cat_SubImpuestoC = _db.Cat_Impuestos.Find(id);
            if (cat_SubImpuestoC == null)
            {
                return HttpNotFound();
            }
            
            return View(cat_SubImpuestoC);
        }
        public ActionResult EliminarImpuesto(int Id)
        {
            try
            {
                Cat_SubImpuestoC cat_SubImpuestoC = _db.Cat_Impuestos.Find(Id);
                _db.Cat_Impuestos.Remove(cat_SubImpuestoC);
                _db.SaveChanges();

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Index");
        }
        // Agregar Impuesto
        public ActionResult Create()
        {
            PopulaImpuestoSat();
            PopulaTipoImpuesto();
            var sucursal = ObtenerSucursal();
            var cat_subImpuestoC = new Cat_SubImpuestoC();
            return View(cat_subImpuestoC);
        }
        public ActionResult AgregarImpuesto(String TipoImp, String Nombre, int Base, c_Impuesto Impuesto,c_TipoFactor TipoFactor,Decimal TasaOCuota/*,Decimal Importe*/)
        {
            try
            {
                var cat_SubImpuestoC = new Cat_SubImpuestoC()
                {
                    TipoImpuesto = TipoImp,
                    Nombre = Nombre,
                    Base = Base,
                    Impuesto = Impuesto,
                    TipoFactor = TipoFactor,
                    TasaOCuota = TasaOCuota,
                    //Importe = Importe,
                    SucursalId = ObtenerSucursal(),
                };
                _db.Cat_Impuestos.Add(cat_SubImpuestoC);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return RedirectToAction("Index");
        }
        //Editar Impuesto
        public ActionResult Editar(int? id)
        {
            PopulaImpuestoSat();
            PopulaTipoImpuesto();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cat_SubImpuestoC cat_SubImpuestoC = _db.Cat_Impuestos.Find(id);
            if (cat_SubImpuestoC == null)
            {
                return HttpNotFound();
            }

            return View(cat_SubImpuestoC);
        }
        public ActionResult EditarImpuesto(int Id, String Nombre, string TipoImp, int Base, c_Impuesto Impuesto, c_TipoFactor TipoFactor, Decimal TasaOCuota/*, Decimal Importe*/)
        {
            try {
                var impuesto = new Cat_SubImpuestoC();
                    
                impuesto = _db.Cat_Impuestos.Find(Id);
                impuesto.Nombre = Nombre;
                impuesto.TipoImpuesto = TipoImp;
                impuesto.Base = Base;
                impuesto.Impuesto = Impuesto;
                impuesto.TipoFactor = TipoFactor;
                impuesto.TasaOCuota = TasaOCuota;
                //impuesto.Importe = Importe;

                _db.Entry(impuesto).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Index");
        }

        private void PopulaTipoImpuesto()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Traslado", Value = "Traslado", Selected = true });
            items.Add(new SelectListItem { Text = "Retencion", Value = "Retencion" });
            ViewBag.TImpuesto = items;
        }

        private void PopulaImpuestoSat()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ImpuestoSat = (popularDropDowns.PopulaImpuestoSat());
        }
    }
}