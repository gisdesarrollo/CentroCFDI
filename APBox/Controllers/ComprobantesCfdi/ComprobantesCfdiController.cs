using APBox.Context;
using APBox.Control;
using API.Catalogos;
using API.Enums;
using API.Enums.CartaPorteEnums;
using API.Models.Dto;
using API.Models.Operaciones;
using API.Operaciones.ComplementoCartaPorte;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComprobantesCfdi;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones;
using Aplicacion.LogicaPrincipal.ComplementosPagos;
using Aplicacion.LogicaPrincipal.GeneracionComprobante;
using Aplicacion.LogicaPrincipal.GeneraPDfCartaPorte;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace APBox.Controllers.ComprobantesCfdi
{
    [SessionExpire]
    public class ComprobantesCfdiController : Controller
    {
        #region Variables
            private readonly APBoxContext _db = new APBoxContext();
            private readonly LogicaFacadeFacturas _logicaFacadeFacturas = new LogicaFacadeFacturas();
            private readonly AcondicionarComprobanteCfdi _acondicionarComprobante = new AcondicionarComprobanteCfdi();
            private readonly ComprobanteManager _ComprobanteManager = new ComprobanteManager();
            private readonly ComprobanteXsaManager _ComprobanteXsaManager = new ComprobanteXsaManager();
            private readonly CreationFile _creationFile = new CreationFile();
        #endregion

        // GET: ComprobanteCfdi
        public ActionResult Index()
        {
            PopulaEstatus();
            PopulaTiposDeComprobante();
            var comprobanteCfdiModel = new ComprobanteCfdiModel();
            
            
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var fechaFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            comprobanteCfdiModel.FechaInicial = fechaInicial;
            comprobanteCfdiModel.FechaFinal = fechaFinal;

            comprobanteCfdiModel.ComprobanteCfdi = _logicaFacadeFacturas.FiltrarComprobanteCFDI(fechaInicial, fechaFinal, false, null, ObtenerSucursal());
            return View(comprobanteCfdiModel);
        }

        [HttpPost]
        public ActionResult Index(ComprobanteCfdiModel comprobanteCfdiModel, string actionName)
        {
            PopulaEstatus();
            PopulaTiposDeComprobante();
            if (actionName == "Filtrar")
            {
                DateTime fechaI = comprobanteCfdiModel.FechaInicial;
                DateTime fechaF = comprobanteCfdiModel.FechaFinal;
                

                var fechaInicial = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 0, 0, 0);
                var fechaFinal = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 59);
                comprobanteCfdiModel.FechaInicial = fechaInicial;
                comprobanteCfdiModel.FechaFinal = fechaFinal;

                comprobanteCfdiModel.ComprobanteCfdi = _logicaFacadeFacturas.FiltrarComprobanteCFDI(fechaInicial, fechaFinal, comprobanteCfdiModel.Estatus,comprobanteCfdiModel.TipoDeComprobante, ObtenerSucursal());

            }
            return View(comprobanteCfdiModel);
        }

        // GET: ComprobanteCfdi/Create
        public ActionResult Create()
        {
            PopulaClientes();
            PopulaCfdiRelacionado();
            PopulaTipoRelacion();
            PopulaFormaPago();
            PopulaObjetoImpuesto();
            PopulaImpuestoSat();
            PopulaTiposDeComprobante();
            PopulaConceptos();
            var comprobante = new ComprobanteCfdi()
            {
                Generado = false,
                Status = Status.Activo,
                FechaDocumento = DateTime.Now,
                Mes = (Meses)Enum.ToObject(typeof(Meses), DateTime.Now.Month),
                SucursalId = ObtenerSucursal(),
                Version = "4.0",
                ExportacionId = "01",//No aplica,
                Conceptos = new Conceptos()
                {
                    Traslado = new TrasladoCP()
                    {
                        TipoImpuesto = "Traslado",
                        TipoFactor = c_TipoFactor.Tasa,
                        Base = 0,
                        TasaOCuota = 0,
                        Importe = 0
                    },
                    Retencion = new RetencionCP()
                    {
                        TipoImpuesto = "Retencion",
                        TipoFactor = c_TipoFactor.Tasa,
                        Base = 0,
                        TasaOCuota = 0,
                        Importe = 0
                    }

                }
            };
            return View(comprobante);
        }

        // POST: ComprobanteCfdi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComprobanteCfdi comprobanteCfdi)
        {
            ModelState.Remove("CfdiRelacionado.FacturaEmitidaId");
            ModelState.Remove("Conceptos.ObjetoImpuesto");

            PopulaClientes(comprobanteCfdi.ReceptorId);
            PopulaCfdiRelacionado(comprobanteCfdi.CfdiRelacionadoId);
            PopulaTipoRelacion();
            PopulaFormaPago();
            PopulaObjetoImpuesto();
            PopulaImpuestoSat();
            PopulaTiposDeComprobante();
            PopulaConceptos();

            if (Request.Files.Count > 0)
            {
                var archivo = Request.Files[0];
                if (archivo.ContentLength > 0)
                {

                    return View(comprobanteCfdi);
                }
            }

            if (ModelState.IsValid)
            {
                _acondicionarComprobante.CargaInicial(ref comprobanteCfdi);
                try
                {
                    var conceptos = comprobanteCfdi.Conceptoss;
                    conceptos.ForEach(p => { p.ComprobanteCfdi = null; p.ComplementoCP = null; });
                    comprobanteCfdi.Conceptoss = null;
                    comprobanteCfdi.Status= Status.Activo;
                    _db.ComprobantesCfdi.Add(comprobanteCfdi);
                    _db.SaveChanges();

                    foreach (var concepto in conceptos)
                    {
                        concepto.ComplementoCP = null;
                        concepto.Complemento_Id = null;
                        concepto.ComprobanteCfdi = null;
                        concepto.Comprobante_Id = comprobanteCfdi.Id;
                        _db.Conceptos.Add(concepto);
                    }
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(comprobanteCfdi);
                }
            }
            else
            {
                //Identifica los mensaje de error
                var errors = ModelState.Values.Where(E => E.Errors.Count > 0)
                         .SelectMany(E => E.Errors)
                         .Select(E => E.ErrorMessage)
                         .ToList();
                //Identifica el campo del Required
                var modelErrors = ModelState.Where(m => ModelState[m.Key].Errors.Any());
                ModelState.AddModelError("", "Error revisar los campos requeridos: " + errors);
            }
            return View(comprobanteCfdi);
        }

        // GET: ComprobanteCfdi/Edit/
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComprobanteCfdi CCfdi = _db.ComprobantesCfdi.Find(id);
            if(CCfdi == null) { return HttpNotFound(); }
            PopulaClientes(CCfdi.ReceptorId);
            PopulaCfdiRelacionado(CCfdi.CfdiRelacionadoId);
            PopulaTipoRelacion();
            PopulaFormaPago();
            PopulaObjetoImpuesto();
            PopulaImpuestoSat();
            PopulaTiposDeComprobante();
            PopulaConceptos();
            CCfdi.FormaPagoId = CCfdi.FormaPago;
            CCfdi.TipoRelacionId = CCfdi.TipoRelacion;
            CCfdi.TipoComprobanteId = CCfdi.TipoDeComprobante;
             CCfdi.Conceptos = new Conceptos()
             {
                 Traslado = new TrasladoCP()
                 {
                     TipoImpuesto = "Traslado",
                     TipoFactor = c_TipoFactor.Tasa,
                     Base = 0,
                     TasaOCuota = 0,
                     Importe = 0
                 },
                 Retencion = new RetencionCP()
                 {
                     TipoImpuesto = "Retencion",
                     TipoFactor = c_TipoFactor.Tasa,
                     Base = 0,
                     TasaOCuota = 0,
                     Importe = 0
                 }

             };
            


            return View(CCfdi);
        }

        // POST: ComprobanteCfdi/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComprobanteCfdi comprobanteCfdi)
        {
            ModelState.Remove("CfdiRelacionado.FacturaEmitidaId");
            ModelState.Remove("Conceptos.ObjetoImpuesto");

            PopulaClientes(comprobanteCfdi.ReceptorId);
            PopulaCfdiRelacionado(comprobanteCfdi.CfdiRelacionadoId);
            PopulaTipoRelacion();
            PopulaFormaPago();
            PopulaObjetoImpuesto();
            PopulaImpuestoSat();
            PopulaTiposDeComprobante();
            PopulaConceptos();

            if (ModelState.IsValid)
            {

                _acondicionarComprobante.CargaRelacion(comprobanteCfdi);
                //_acondicionarComprobante.CargaValidacion(ref comprobanteCfdi);
                try
                {
                    comprobanteCfdi.FacturaEmitida = null;
                    comprobanteCfdi.Conceptoss = null;
                    comprobanteCfdi.Status = Status.Activo;
                    _db.Entry(comprobanteCfdi).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(comprobanteCfdi);
                }
            }
            else
            {
                //Identifica los mensaje de error
                var errors = ModelState.Values.Where(E => E.Errors.Count > 0)
                         .SelectMany(E => E.Errors)
                         .Select(E => E.ErrorMessage)
                         .ToList();
                //Identifica el campo del Required
                var modelErrors = ModelState.Where(m => ModelState[m.Key].Errors.Any());
                ModelState.AddModelError("", "Error revisar los campos requeridos: " + errors);
            }
            return View(comprobanteCfdi);
        }

        // GET: ComprobanteCfdi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComprobanteCfdi comprobanteCfdi = _db.ComprobantesCfdi.Find(id);
            if (comprobanteCfdi == null)
            {
                return HttpNotFound();
            }
            PopulaClientes(comprobanteCfdi.ReceptorId);


            _db.ComprobantesCfdi.Remove(comprobanteCfdi);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

     
        public ActionResult Generar(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComprobanteCfdi comprobanteCfdi = _db.ComprobantesCfdi.Find(id);
            
            if (comprobanteCfdi == null)
            {
                return HttpNotFound();
            }
            PopulaClientes(comprobanteCfdi.ReceptorId);
            return View(comprobanteCfdi);
        }
        [HttpPost]
        public ActionResult Generar(ComprobanteCfdi comprobanteCfdi)
        {
            ModelState.Remove("CfdiRelacionado.FacturaEmitidaId");
            ModelState.Remove("Conceptos.ObjetoImpuesto");
            PopulaClientes(comprobanteCfdi.ReceptorId);
            string error = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var sucursalId = ObtenerSucursal();
                    Sucursal sucursal = _db.Sucursales.Find(sucursalId);
                    DateTime fechaDoc = comprobanteCfdi.FechaDocumento;
                    var horaHoy = DateTime.Now;
                    var fechaTime = new DateTime(fechaDoc.Year, fechaDoc.Month, fechaDoc.Day, horaHoy.Hour, horaHoy.Minute, horaHoy.Second);
                    var CCfdi = _db.ComprobantesCfdi.Find(comprobanteCfdi.Id);
                    CCfdi.ReceptorId = comprobanteCfdi.ReceptorId;
                    CCfdi.FechaDocumento = fechaTime;
                    

                    
                    if (sucursal.Trv && sucursal.Txsa)
                    {
                        throw new Exception("Seleccionar Solo Un Tipo De Timbrado..");
                    }
                    else if (sucursal.Trv)
                    {
                        _db.Entry(CCfdi).State = EntityState.Modified;
                        _db.SaveChanges();
                        _ComprobanteManager.GenerarComprobanteCfdi(sucursalId, comprobanteCfdi.Id);
                    }
                    else if(sucursal.Txsa) {
                        _db.Entry(CCfdi).State = EntityState.Modified;
                        _db.SaveChanges();
                        _ComprobanteXsaManager.GenerarComprobanteCfdi(sucursalId, comprobanteCfdi.Id); 
                    }
                    else { throw new Exception("Seleccionar Un Tipo De Timbrado!!"); }

                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                if (error == "")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(comprobanteCfdi);
        }

        public ActionResult DescargarXml(int id)
        {
            var pathCompleto = _ComprobanteManager.GenerarXml(id);
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

        public ActionResult DescargarPDF(int id)
        {
            ComprobanteCFDI oComprobante = new ComprobanteCFDI();
            byte[] archivoFisico = new byte[1024];
            var comprobanteCfdi = _db.ComprobantesCfdi.Find(id);
            if (comprobanteCfdi.Sucursal.Trv)
            {
                oComprobante = _creationFile.DeserealizarComprobanteXML(id);
                archivoFisico = _creationFile.GeneraPDFComprobante(oComprobante, id);
            }
            if (comprobanteCfdi.Sucursal.Txsa) 
            {
                archivoFisico =_ComprobanteXsaManager.DownloadPDFXsa(comprobanteCfdi.Id);   
            }            
            MemoryStream ms = new MemoryStream(archivoFisico, 0, 0, true, true);
            string nameArchivo = comprobanteCfdi.FacturaEmitida.Serie + "-" + comprobanteCfdi.FacturaEmitida.Folio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Response.AddHeader("content-disposition", "attachment;filename= " + nameArchivo + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();


            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public ActionResult Cancelar(int id)
        {
            PopulaMotivoCancelacion();
            ViewBag.Error = null;
            ViewBag.Success = null;
            var comprobante = _db.ComprobantesCfdi.Find(id);
            return PartialView("~/Views/ComprobantesCfdi/_Cancelacion.cshtml", comprobante);
        }

        [HttpPost]
        public ActionResult Cancelar(ComprobanteCfdi comprobanteCfdi)
        {
            PopulaMotivoCancelacion();
            string error = null;
            List<DataCancelacionResponseXsaDto> dataXsa = new List<DataCancelacionResponseXsaDto>();
            var comprobante = _db.ComprobantesCfdi.Find(comprobanteCfdi.Id);
            comprobante.FolioSustitucion = comprobanteCfdi.FolioSustitucion;
            comprobante.MotivoCancelacion = comprobanteCfdi.MotivoCancelacion;
            try
            {
                if (comprobante.Sucursal.Trv) { 
                    _ComprobanteManager.Cancelar(comprobante); 
                }
                if (comprobante.Sucursal.Txsa) { 
                    dataXsa = _ComprobanteXsaManager.Cancelar(comprobanteCfdi); 
                   
                }

            }
            catch (Exception ex)
            {
                error = ex.Message;

            }
            if (error == null)
            {
                if (dataXsa.Count < 1)
                {
                    ViewBag.Success = "Proceso de cancelación finalizado con éxito.";
                }
                else {
                    foreach (var d in dataXsa)
                    {
                        ViewBag.Success = "STATUS:" + d.status + "-" + d.descripcion;
                    }
                }
                    ViewBag.Error = null;
            }
            else
            {
                ViewBag.Error = error;
                ViewBag.Success = null;
            }
            return PartialView("~/Views/ComprobantesCfdi/_Cancelacion.cshtml", comprobante);
        }


        public ActionResult DescargarAcuse(int id)
        {
            var comprobante = _db.ComprobantesCfdi.Find(id);
            string xmlCancelacion = "";
            byte[] byteXml = new byte[1024];
            string error = "";
            try {
                if (comprobante.Sucursal.Trv)
                {
                    xmlCancelacion = _ComprobanteManager.DowloadAcuseCancelacion(comprobante);
                    byteXml = Encoding.UTF8.GetBytes(xmlCancelacion);
                }
                if(comprobante.Sucursal.Txsa)
                {
                    byteXml = _ComprobanteXsaManager.DowloadAcuseCancelacion(comprobante);
                }
           }
            catch (Exception ex){
                error = ex.Message;
            }
            if (error != "")
            {
                ModelState.AddModelError("", error);
                return RedirectToAction("Index");
            }
            
            MemoryStream ms = new MemoryStream(byteXml, 0, 0, true, true);
            string nameArchivo = comprobante.FacturaEmitida.Serie + "-" + comprobante.FacturaEmitida.Folio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Response.AddHeader("content-disposition", "attachment;filename= " + nameArchivo + ".xml");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, "application/xml");
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }
        private void PopulaEstatus()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Generado", Value = "true" });
            items.Add(new SelectListItem { Text = "Pendiente", Value = "false" });
            ViewBag.StatusCP = items;
        }

        private void PopulaClientes(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.ReceptorId = popularDropDowns.PopulaClientes(receptorId);
        }

        private void PopulaCfdiRelacionado(int? cfdiRelacionadoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.CfdiRelacionadoId = popularDropDowns.PopulaFacturasEmitidas(false, 0, cfdiRelacionadoId);
        }

        private void PopulaTipoRelacion()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.tipoRelacion = (popularDropDowns.PopulaTipoRelacion());
        }

        /*private void PopulaFormaPago()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.formaPago = (popularDropDowns.PopulaFormaPago());
        }*/
        private void PopulaFormaPago()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.FormaPAgo = (popularDropDowns.PopulaFormaPago());
        }

        private void PopulaImpuestoSat()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ImpuestoSat = (popularDropDowns.PopulaImpuestoSat());
        }
        private void PopulaObjetoImpuesto()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.objetoImpuesto = (popularDropDowns.PopulaObjetoImpuesto());
        }

        public JsonResult DatosClaveUnidad(string ClaveUnidad)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosClaveUnidad(ClaveUnidad);
            return Json(Clave, JsonRequestBehavior.AllowGet);
        }

        private void PopulaTiposDeComprobante()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoDeComprobante = (popularDropDowns.PopulaTipoDeComprobanteCfdi());
        }

        private void PopulaMotivoCancelacion()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "01 - Comprobante Emitido con errores con relación", Value = "01", Selected = true });
            items.Add(new SelectListItem { Text = "02 - Comprobante emitido con errores sin relacion", Value = "02" });
            items.Add(new SelectListItem { Text = "03 - No se llevo a cabo la operación", Value = "03" });
            items.Add(new SelectListItem { Text = "04 - Operación nominativa relacionada en una factura global", Value = "04" });
            ViewBag.motivoCancelacion = items;
        }

        private void PopulaConceptos()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.Conceptos = (popularDropDowns.PopulaConceptos(ObtenerSucursal()));
        }

        public JsonResult DatosCatalogoConceptos(int claveProd)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Conceptos = popularDropDowns.PopulaCatConceptos(claveProd);

            return Json(Conceptos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DatosCatalogoImpuesto(int IdImpuesto)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var impuestos = popularDropDowns.PopulaCatImpuestos(IdImpuesto);

            return Json(impuestos, JsonRequestBehavior.AllowGet);
        }
    }
}
