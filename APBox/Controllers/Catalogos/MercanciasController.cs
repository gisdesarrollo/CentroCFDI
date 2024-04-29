using APBox.Context;
using APBox.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using API.Operaciones.ComplementoCartaPorte;
using System.Net;
using System.Data.Entity;
using API.Enums;

namespace APBox.Controllers.Catalogos
{
    public class MercanciasController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();
        // GET: Mercancias
        public ActionResult Index()
        {
            var sucursal = ObtenerSucursal();
            var cat_Mercancias = _db.Cat_Mercancias.Where(s => s.SucursalId== sucursal).ToList();

            ViewBag.Controller = "Mercancias";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.Title = "catalogo";

            return View(cat_Mercancias);
        }


        // Agregar Mercancias
        public ActionResult Create()
        {
            TipoEmbalaje_Id();
            var sucursal = ObtenerSucursal();
            var cat_Mercancias = new Cat_Mercancias();

            ViewBag.Controller = "Mercancias";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.Title = "catalogo";

            return View(cat_Mercancias);
        }

        // POST: Mercancias/Create
        public ActionResult AgregarMercancias(String ClaveProdServCP, string Descripcion, int Cantidad, String ClavesUnidad, String Unidad, string Dimensiones, Decimal PesoEnKg, String ValorMercancia, c_Moneda Moneda, string ClaveMaterialPeligroso, string DescripEmbalaje, string TipoEmbalaje_Id)
        {
            try
            {
                var Mercancias = new Cat_Mercancias()
                {
                    ClaveProdServCP = ClaveProdServCP,
                    Descripcion = Descripcion,
                    Cantidad = Cantidad,
                    ClavesUnidad = ClavesUnidad,
                    Unidad = Unidad,
                    Dimensiones = Dimensiones,
                    PesoEnKg = PesoEnKg,
                    ValorMercancia = ValorMercancia,
                    Moneda = (API.Enums.c_Moneda)Enum.Parse(typeof(API.Enums.c_Moneda), Moneda.ToString()),
                    ClaveMaterialPeligroso = ClaveMaterialPeligroso,
                    DescripEmbalaje = DescripEmbalaje,
                    TipoEmbalaje_Id = TipoEmbalaje_Id,
                    SucursalId = ObtenerSucursal()
                          
            };
                    _db.Cat_Mercancias.Add(Mercancias);
                    _db.SaveChanges();
                }
           
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return RedirectToAction("Index");
        }



        //Eliminar Mercancias
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cat_Mercancias cat_Mercancias = _db.Cat_Mercancias.Find(id);
            if (cat_Mercancias == null)
            {
                return HttpNotFound();
            }

            return View(cat_Mercancias);
        }
        public ActionResult EliminarMercancia(int Id)
        {
            try
            {
                Cat_Mercancias cat_Mercancias = _db.Cat_Mercancias.Find(Id);
                _db.Cat_Mercancias.Remove(cat_Mercancias);
                _db.SaveChanges();

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Index");
        }




        //Editar Mercancias
        public ActionResult Editar(int? id)
        {
            TipoEmbalaje_Id();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cat_Mercancias cat_Mercancias = _db.Cat_Mercancias.Find(id);
            if (cat_Mercancias == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.Controller = "Mercancias";
            ViewBag.Action = "Editar";
            ViewBag.ActionES = "Editar";
            ViewBag.Title = "catalogo";
            return View(cat_Mercancias);
        }
      
        public ActionResult EditarMercancia(int Id, String ClaveProdServCP, string Descripcion,  String ClavesUnidad, String Unidad, string Dimensiones,  String ValorMercancia,  string ClaveMaterialPeligroso, string DescripEmbalaje, string TipoEmbalaje_Id, int Cantidad = 0, Decimal PesoEnKg = 0, c_Moneda Moneda = 0)
        {
            try
            {
            
                var Mercancias = new Cat_Mercancias();

                Mercancias = _db.Cat_Mercancias.Find(Id);

                Mercancias.ClaveProdServCP = ClaveProdServCP;
                Mercancias.Descripcion = Descripcion;
                Mercancias.Cantidad = Cantidad;
                Mercancias.ClavesUnidad = ClavesUnidad;
                Mercancias.Unidad = Unidad;
                Mercancias.Dimensiones = Dimensiones;
                Mercancias.PesoEnKg = PesoEnKg;
                Mercancias.ValorMercancia = ValorMercancia;
                Mercancias.Moneda = (API.Enums.c_Moneda)Enum.Parse(typeof(API.Enums.c_Moneda), Moneda.ToString());
                Mercancias.ClaveMaterialPeligroso = ClaveMaterialPeligroso;
                Mercancias.DescripEmbalaje = DescripEmbalaje;
                Mercancias.TipoEmbalaje_Id = TipoEmbalaje_Id;
                _db.Entry(Mercancias).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return RedirectToAction("Index");
        }




        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }
        //Validaciones
        private void TipoEmbalaje_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TiposEmbalaje = (popularDropDowns.TipoEmbalaje_Id());
        }

        public JsonResult DatosClaveProdServCP(string ClaveProdServCP)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosClaveProdCP(ClaveProdServCP);
            return Json(Clave, JsonRequestBehavior.AllowGet);
        }

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }



    }


}

    

