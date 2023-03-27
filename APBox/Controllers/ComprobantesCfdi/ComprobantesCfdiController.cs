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
using Aplicacion.LogicaPrincipal.CargasMasivas.CSV;
using Aplicacion.LogicaPrincipal.ComplementosPagos;
using Aplicacion.LogicaPrincipal.Descargas;
using Aplicacion.LogicaPrincipal.GeneracionComprobante;
using Aplicacion.LogicaPrincipal.GeneraPDfCartaPorte;
using Aplicacion.LogicaPrincipal.Validacion;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utilerias.LogicaPrincipal;

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
            private readonly CargarConceptos _cargarConceptos = new CargarConceptos();
            private readonly DecodificaFacturas _decodifica = new DecodificaFacturas();
            private readonly DescargasManager _descargasManager = new DescargasManager();
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
            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.NameHere = "emision";
            return View(comprobanteCfdiModel);
        }

        [HttpPost]
        public ActionResult Index(ComprobanteCfdiModel comprobanteCfdiModel, string actionName)
        {
            PopulaEstatus();
            PopulaTiposDeComprobante();
            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.NameHere = "emision";

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
                TipoCambio = "1",
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
            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "emision";
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
            
            PopulaTipoRelacion();
            PopulaFormaPago();
            PopulaObjetoImpuesto();
            PopulaImpuestoSat();
            PopulaTiposDeComprobante();
            PopulaConceptos();
            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "emision";
            string archivo = null;
            List<Conceptos> conceptosCSV = new List<Conceptos>();

            if (ModelState.IsValid)
            {
                try
                {
                if (Request.Files.Count > 0)
                {
                    if (Request.Files[0].ContentLength > 0)
                    {
                        archivo = SubeArchivo(0);
                      
                    }
                }
                }catch(Exception ex)
                {
                    ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                    return View(comprobanteCfdi);
                }

                try
                {
                    if(archivo != null)
                    {
                      conceptosCSV = _cargarConceptos.Importar(archivo,comprobanteCfdi.SucursalId);
                        if (conceptosCSV.Count > 0)
                        {
                            
                            comprobanteCfdi.Conceptoss = new List<Conceptos>();
                            conceptosCSV.ForEach(c => comprobanteCfdi.Conceptoss.Add(c));
                            decimal subtotal = 0;
                            decimal total = 0;
                            decimal totalTraslado = 0;
                            decimal totalRetencion = 0;
                            //calcula subtotal y total
                            conceptosCSV.ForEach(c => subtotal += (decimal)c.Importe);
                            conceptosCSV.ForEach(c => { if (c.Traslado != null) { totalTraslado += c.Traslado.Importe; }
                                if (c.Retencion != null) { totalRetencion += c.Retencion.Importe; }
                            });
                            total = (subtotal + totalTraslado) - totalRetencion;
                            
                            comprobanteCfdi.Subtotal = subtotal;
                            comprobanteCfdi.Total = total;
                            // se actualizan cambios en el ModelState
                            ModelState.SetModelValue("Subtotal", new ValueProviderResult(subtotal, string.Empty, CultureInfo.InvariantCulture));
                            ModelState.SetModelValue("Total", new ValueProviderResult(total, string.Empty, CultureInfo.InvariantCulture));
                            
                            return View(comprobanteCfdi);
                        }
                    }
                }catch(Exception ex)
                {
                    var errores = ex.Message.Split('|');
                    foreach (var error in errores)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            
                try
                {
                    _acondicionarComprobante.CargaInicial(ref comprobanteCfdi);
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
            
            PopulaTipoRelacion();
            PopulaFormaPago();
            PopulaObjetoImpuesto();
            PopulaImpuestoSat();
            PopulaTiposDeComprobante();
            PopulaConceptos();
            CCfdi.FormaPagoId = CCfdi.FormaPago;
            CCfdi.IdTipoRelacion = CCfdi.TipoRelacion;
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


            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Edit";
            ViewBag.ActionES = "Editar";
            ViewBag.NameHere = "emision";
            return View(CCfdi);
        }

        // POST: ComprobanteCfdi/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComprobanteCfdi comprobanteCfdi)
        {
            ModelState.Remove("CfdiRelacionado.FacturaEmitidaId");
            ModelState.Remove("Conceptos.ObjetoImpuesto");
            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Edit";
            ViewBag.ActionES = "Editar";
            ViewBag.NameHere = "emision";
            PopulaClientes(comprobanteCfdi.ReceptorId);
           
            PopulaTipoRelacion();
            PopulaFormaPago();
            PopulaObjetoImpuesto();
            PopulaImpuestoSat();
            PopulaTiposDeComprobante();
            PopulaConceptos();
            string archivo = null;
            List<Conceptos> conceptosCSV = new List<Conceptos>();

            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        if (Request.Files[0].ContentLength > 0)
                        {
                            archivo = SubeArchivo(0);

                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                    return View(comprobanteCfdi);
                }
                try { 
                if (archivo != null)
                {
                    conceptosCSV = _cargarConceptos.Importar(archivo, comprobanteCfdi.SucursalId);
                    if (conceptosCSV.Count > 0)
                    {
                        if(comprobanteCfdi.Conceptoss.Count == 0)
                            {
                                comprobanteCfdi.Conceptoss = new List<Conceptos>();
                            }
                        
                        conceptosCSV.ForEach(c => comprobanteCfdi.Conceptoss.Add(c));
                        decimal subtotal = 0;
                        decimal total = 0;
                        decimal totalTraslado = 0;
                        decimal totalRetencion = 0;
                        //calcula subtotal y total
                        conceptosCSV.ForEach(c => subtotal += (decimal)c.Importe);
                        conceptosCSV.ForEach(c => {
                            if (c.Traslado != null) { totalTraslado += c.Traslado.Importe; }
                            if (c.Retencion != null) { totalRetencion += c.Retencion.Importe; }
                        });
                        total = (subtotal + totalTraslado) - totalRetencion;

                        comprobanteCfdi.Subtotal += subtotal;
                        comprobanteCfdi.Total += total;
                        // se actualizan cambios en el ModelState
                        ModelState.SetModelValue("Subtotal", new ValueProviderResult(comprobanteCfdi.Subtotal, string.Empty, CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("Total", new ValueProviderResult(comprobanteCfdi.Total, string.Empty, CultureInfo.InvariantCulture));

                        return View(comprobanteCfdi);
                    }
                  }
                }catch (Exception ex)
                {
                    var errores = ex.Message.Split('|');
                    foreach (var error in errores)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

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
            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Generar";
            ViewBag.ActionES = "Generar";
            ViewBag.NameHere = "emision";
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
            return View(comprobanteCfdi);
        }
        [HttpPost]
        public ActionResult Generar(ComprobanteCfdi comprobanteCfdi)
        {
            ModelState.Remove("CfdiRelacionado.FacturaEmitidaId");
            ModelState.Remove("Conceptos.ObjetoImpuesto");
            ViewBag.Controller = "ComprobantesCfdi";
            ViewBag.Action = "Generar";
            ViewBag.ActionES = "Generar";
            ViewBag.NameHere = "emision";
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

        public ActionResult Exportar()
        {
            var pathCompleto = _cargarConceptos.Exportar();
            byte[] filedata = System.IO.File.ReadAllBytes(pathCompleto);
            string contentType = MimeMapping.GetMimeMapping(pathCompleto);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(pathCompleto),
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(filedata, contentType);
        }

        private String SubeArchivo(int indice)
        {
            if (Request.Files.Count > 0)
            {
                var archivo = Request.Files[indice];
                if (archivo.ContentLength > 0)
                {
                    var operacionesStreams = new OperacionesStreams();
                    var nombreArchivo = Path.GetFileName(archivo.FileName);

                    var pathDestino = Path.Combine(Server.MapPath("~/Archivos/CargasMasivas/"), archivo.FileName);
                    Stream fileStream = archivo.InputStream;
                    var mStreamer = new MemoryStream();
                    mStreamer.SetLength(fileStream.Length);
                    fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
                    mStreamer.Seek(0, SeekOrigin.Begin);
                    operacionesStreams.StreamArchivo(mStreamer, pathDestino);
                    return pathDestino;
                }
            }
            throw new Exception("Favor de cargar por lo menos un archivo");
        }

        public ActionResult DescargarXml(int id)
        {
            var comprobanteCfdi = _db.ComprobantesCfdi.Find(id);
            var pathCompleto = _descargasManager.GeneraFilePathXml(comprobanteCfdi.FacturaEmitida.ArchivoFisicoXml, comprobanteCfdi.FacturaEmitida.Serie, comprobanteCfdi.FacturaEmitida.Folio);
            //var pathCompleto = _ComprobanteManager.GenerarXml(id);
            byte[] archivoFisico = System.IO.File.ReadAllBytes(pathCompleto);
            string contentType = MimeMapping.GetMimeMapping(pathCompleto);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(pathCompleto),
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            //Elimino el archivo Temp
            System.IO.File.Delete(pathCompleto);
            return File(archivoFisico, contentType);
        }

        public ActionResult DescargarPDF(int id)
        {
            ComprobanteCFDI oComprobante = new ComprobanteCFDI();
            byte[] archivoFisico = new byte[1024];
            var comprobanteCfdi = _db.ComprobantesCfdi.Find(id);
            string tipoDocumento = null;
            if (comprobanteCfdi.Sucursal.Trv)
            {
                oComprobante = _decodifica.DeserealizarXML40(comprobanteCfdi.FacturaEmitida.ArchivoFisicoXml);
                tipoDocumento = _decodifica.TipoDocumentoCfdi40(comprobanteCfdi.FacturaEmitida.ArchivoFisicoXml);
                archivoFisico = _descargasManager.GeneraPDF40(oComprobante, tipoDocumento, id, false);
                //oComprobante = _creationFile.DeserealizarComprobanteXML(id);
                //archivoFisico = _creationFile.GeneraPDFComprobante(oComprobante, id);
            }
            if (comprobanteCfdi.Sucursal.Txsa) 
            {
                archivoFisico =_ComprobanteXsaManager.DownloadPDFXsa(comprobanteCfdi.Id,false);   
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
                    dataXsa = _ComprobanteXsaManager.Cancelar(comprobanteCfdi.Id,comprobanteCfdi.FolioSustitucion,comprobanteCfdi.MotivoCancelacion,false); 
                   
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
                    byteXml = _ComprobanteXsaManager.DowloadAcuseCancelacion(comprobante.FacturaEmitida.Serie,comprobante.FacturaEmitida.Folio,comprobante.FacturaEmitida.Uuid,comprobante.FacturaEmitida.EmisorId);
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
