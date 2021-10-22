using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using APBox.Context;
using API.Models.Facturas;
using API.Operaciones.Facturacion;
using Aplicacion.LogicaPrincipal.Facturas;
using Aplicacion.LogicaPrincipal.GeneracionComplementosPagos;

namespace APBox.Controllers.Catalogos
{
    [APBox.Control.SessionExpire]
    public class FacturasEmitidasController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly PagosManager _pagosManager = new PagosManager();
        private readonly OperacionesCfdisEmitidos _operacionesCfdisEmitidos = new OperacionesCfdisEmitidos();

        #endregion

        // GET: FacturasEmitidas
        public ActionResult Index()
        {
            var sucursalId = ObtenerSucursal();

            var facturasEmitidasModel = new FacturasEmitidasModel
            {
                FechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                FechaFinal = DateTime.Now,
                SucursalId = ObtenerSucursal(),
            };
            
            _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            return View(facturasEmitidasModel);
        }

        [HttpPost]
        public ActionResult Index(FacturasEmitidasModel facturasEmitidasModel)
        {
            if (ModelState.IsValid)
            {
                _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            }
            return View(facturasEmitidasModel);
        }

        // GET: FacturasEmitidas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacturaEmitida facturaEmitida = _db.FacturasEmitidas.Find(id);
            if (facturaEmitida == null)
            {
                return HttpNotFound();
            }
            return View(facturaEmitida);
        }

        // GET: FacturasEmitidas/Create
        public ActionResult Create()
        {
            var facturaEmitida = new FacturaEmitida
            {
                EmisorId = ObtenerSucursal()
            };

            return View(facturaEmitida);
        }

        // POST: FacturasEmitidas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FacturaEmitida facturaEmitida)
        {
            if (ModelState.IsValid)
            {
                _db.FacturasEmitidas.Add(facturaEmitida);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(facturaEmitida);
        }

        // GET: FacturasEmitidas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacturaEmitida facturaEmitida = _db.FacturasEmitidas.Find(id);
            if (facturaEmitida == null)
            {
                return HttpNotFound();
            }
            return View(facturaEmitida);
        }

        // POST: FacturasEmitidas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FacturaEmitida facturaEmitida)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(facturaEmitida).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(facturaEmitida);
        }

        // GET: FacturasEmitidas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacturaEmitida facturaEmitida = _db.FacturasEmitidas.Find(id);
            if (facturaEmitida == null)
            {
                return HttpNotFound();
            }
            return View(facturaEmitida);
        }

        // POST: FacturasEmitidas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FacturaEmitida facturaEmitida = _db.FacturasEmitidas.Find(id);
            _db.FacturasEmitidas.Remove(facturaEmitida);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SubirPdf(int id)
        {
            var facturaEmitida = _db.FacturasEmitidas.Find(id);
            return View(facturaEmitida);
        }

        public ActionResult SubirPdf(FacturaEmitida facturaEmitida)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SubeArchivos();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(facturaEmitida);
                }
                return RedirectToAction("Index");
            }
            return View(facturaEmitida);
        }

        public ActionResult Descargar(int id)
        {
            var pathCompleto = _pagosManager.GenerarZipFacturaEmitida(id);
            byte[] archivoFisico = System.IO.File.ReadAllBytes(pathCompleto);
            string contentType = MimeMapping.GetMimeMapping(pathCompleto);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(pathCompleto),
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(archivoFisico, contentType);
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

        #region Archivos

        private List<String> SubeArchivos()
        {
            var paths = new List<String>();
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {

                            var fileName = Path.GetFileName(file.FileName);

                            if (Path.GetExtension(fileName) != ".xml")
                            {
                                continue;
                            }

                            var path = Path.Combine(Server.MapPath("~/Archivos/PDFs Facturas Emitidas/"), fileName);
                            file.SaveAs(path);
                            paths.Add(path);
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
                return paths;
            }
            return null;
        }

        #endregion

    }
}
