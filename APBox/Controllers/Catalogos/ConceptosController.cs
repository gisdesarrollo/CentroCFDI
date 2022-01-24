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
            PopulaImpuestoR();
            PopulaImpuestoT();
            var sucursal = ObtenerSucursal();
            var cat_Conceptos = new Cat_Conceptos();
            return View(cat_Conceptos);
        }
        public ActionResult AgregarConceptos(string ClaveProdServID, string ClaveUnidadID, string Cantidad, string Unidad, string NoIdentificacion, string Descripcion, string ValorUnitario, double Importe, string Descuento, int? ImpuestoR, int? ImpuestoT)
        {

            var claveP = _db.claveProdServ.Find(ClaveProdServID);
            var claveunidad = _db.ClavesUnidad.Find(ClaveUnidadID);
            var pruebaRet = ImpuestoR;
            var pruebaTras = ImpuestoT;


            try
            {
                if (pruebaRet == 0 && ImpuestoT == 0)
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
                        //ImpuestoIdTras = ImpuestoT,
                        //ImpuestoIdRet = ImpuestoR
                    };
                    _db.Cat_Conceptos.Add(Conceptos);
                    _db.SaveChanges();
                }
                else
                {
                    if (pruebaRet > 0 && ImpuestoT > 0)
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
                            ImpuestoIdTras = ImpuestoT,
                            ImpuestoIdRet = ImpuestoR
                        };
                        _db.Cat_Conceptos.Add(Conceptos);
                        _db.SaveChanges();
                    }
                    else
                    {
                        if (ImpuestoR > 0)
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
                                ImpuestoIdRet = ImpuestoR
                                //ImpuestoIdTras = ImpuestoT,
                                //ImpuestoIdRet = ImpuestoR
                            };
                            _db.Cat_Conceptos.Add(Conceptos);
                            _db.SaveChanges();
                        }
                        else
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
                                ImpuestoIdTras = ImpuestoT
                                //ImpuestoIdTras = ImpuestoT,
                                //ImpuestoIdRet = ImpuestoR
                            };
                            _db.Cat_Conceptos.Add(Conceptos);
                            _db.SaveChanges();
                        }
                    }

                }



            }
            catch (Exception e)
            {
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
            PopulaImpuestoR();
            PopulaImpuestoT();
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
        public ActionResult EditarConceptos(int Id, string ClaveProdServID, string ClaveUnidadID, string Cantidad, string Unidad, string NoIdentificacion, string Descripcion, string ValorUnitario, double Importe, string Descuento, int? ImpuestoR, int? ImpuestoT)
        {
            var ImpT = ImpuestoT;
            var ImpR = ImpuestoR;


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
                if (ImpT == 0)
                {
                    Conceptos.ImpuestoIdRet = null;
                }
                else
                {
                    Conceptos.ImpuestoIdRet = ImpuestoR;
                }
                if (ImpR == 0)
                {
                    Conceptos.ImpuestoIdTras = null;
                }
                else
                {
                    Conceptos.ImpuestoIdTras = ImpuestoT;
                }


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
                busqueda = _db.claveProdServ.Where(a => a.c_ClaveUnidad.Equals(valor)).Count();
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
        //DropsDown
        private void PopulaImpuestoT()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ImpuestoT = (popularDropDowns.PopulaImpuestoT());
        }

        private void PopulaImpuestoR()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ImpuestoR = (popularDropDowns.PopulaImpuestoR());
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