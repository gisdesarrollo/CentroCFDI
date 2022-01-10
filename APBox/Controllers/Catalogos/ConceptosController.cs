using APBox.Context;
using APBox.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using API.Operaciones.ComplementoCartaPorte;
using System.Net;
using System.Data.Entity;

namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    public class ConceptosController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();

        // GET: 
        public ActionResult Index() 
        {
            var sucursal = ObtenerSucursal();
            var cat_Conceptos = _db.Cat_Conceptos.Where(s => s.SucursalId == sucursal).ToList();
            return View(cat_Conceptos);
        }
        //Agregar Conceptos
        public ActionResult Create()
        {
            var sucursal = ObtenerSucursal();
            var cat_Conceptos = new Cat_Conceptos();
            return View(cat_Conceptos);
        }
        public ActionResult AgregarConceptos(string ClaveProdServID, string ClaveUnidadID, string Cantidad, string Unidad, string NoIdentificacion, string Descripcion, string ValorUnitario, double Importe, string Descuento)
        {

            var claveP = _db.ClavesProdServCP.Find(ClaveProdServID);
            var claveunidad = _db.ClavesUnidad.Find(ClaveUnidadID);
            try
            {
                var Conceptos = new Cat_Conceptos()
                {
                    ClaveProdServ_Id = ClaveProdServID,
                    ClavesProdServ = claveP.Descripcion,
                    ClaveUnidad_Id = ClaveUnidadID,
                    ClavesUnidad = claveunidad.Nombre,
                    Cantidad = Cantidad,
                    Unidad = Unidad,
                    NoIdentificacion = NoIdentificacion,
                    Descripcion = Descripcion,
                    ValorUnitario = ValorUnitario,
                    Importe = Importe,
                    Descuento = Descuento,
                    SucursalId = ObtenerSucursal(),
                };
                _db.Cat_Conceptos.Add(Conceptos);
                _db.SaveChanges();
            }
            catch (Exception e) {
                ModelState.AddModelError("", e.Message);
            }
            
            return RedirectToAction("Index");
        }
        
        //Eliminar Conceptos
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cat_Conceptos cat_conceptos = _db.Cat_Conceptos.Find(id);
            if (cat_conceptos == null)
            {
                return HttpNotFound();
            }
            
            return View(cat_conceptos);
        }
        public ActionResult EliminarConcepto(int Id)
        {
            try
            {
                Cat_Conceptos cat_Conceptos = _db.Cat_Conceptos.Find(Id);
                _db.Cat_Conceptos.Remove(cat_Conceptos);
                _db.SaveChanges();

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Index");
        }

        //Editar Conceptos
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cat_Conceptos cat_conceptos = _db.Cat_Conceptos.Find(id);
            if (cat_conceptos == null)
            {
                return HttpNotFound();
            }

            return View(cat_conceptos);
        }
        public ActionResult EditarConceptos(int Id,string ClaveProdServID, string ClaveUnidadID, string Cantidad, string Unidad, string NoIdentificacion, string Descripcion, string ValorUnitario, double Importe, string Descuento)
        {
            try
            {
                var Conceptos = new Cat_Conceptos();

                Conceptos = _db.Cat_Conceptos.Find(Id);

                Conceptos.ClaveProdServ_Id = ClaveProdServID;
                Conceptos.ClaveUnidad_Id = ClaveUnidadID;
                Conceptos.Cantidad = Cantidad;
                Conceptos.Unidad = Unidad;
                Conceptos.NoIdentificacion = NoIdentificacion;
                Conceptos.Descripcion = Descripcion;
                Conceptos.ValorUnitario = ValorUnitario;
                Conceptos.Importe = Importe;
                Conceptos.Descuento = Descuento;

                _db.Entry(Conceptos).State = EntityState.Modified;
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
        public int Buscar(string valor, String tipo)
        {

            var busqueda = 0;
            if (tipo.Equals("serv"))
            {
                busqueda = _db.ClavesProdServCP.Where(a => a.c_ClaveUnidad.Equals(valor)).Count();
            }
            if (tipo.Equals("claveUnidad"))
            {
                busqueda = _db.ClavesUnidad.Where(a => a.c_ClaveUnidad.Equals(valor)).Count();
            }

            return busqueda;
        }

        public JsonResult DatosClaveUnidad(string ClaveUnidad)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosClaveUnidad(ClaveUnidad);
            return Json(Clave, JsonRequestBehavior.AllowGet);
        }





        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }
        /*public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }*/
    }
}