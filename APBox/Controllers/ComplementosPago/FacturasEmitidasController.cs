using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using APBox.Context;
using API.Enums;
using API.Models.Dto;
using API.Models.Facturas;
using API.Operaciones.Facturacion;
using Aplicacion.LogicaPrincipal.Descargas;
using Aplicacion.LogicaPrincipal.Facturas;
using Aplicacion.LogicaPrincipal.GeneracionComplementosPagos;
using Aplicacion.LogicaPrincipal.GeneracionComprobante;
using Aplicacion.LogicaPrincipal.Validacion;
using MySql.Data.MySqlClient;


namespace APBox.Controllers.Catalogos
{
    [APBox.Control.SessionExpire]
    public class FacturasEmitidasController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly PagosManager _pagosManager = new PagosManager();
        private readonly OperacionesCfdisEmitidos _operacionesCfdisEmitidos = new OperacionesCfdisEmitidos();
        private readonly DescargasManager _descargasManager = new DescargasManager();
        private readonly DecodificaFacturas _decodifica = new DecodificaFacturas();
        private readonly ComprobanteXsaManager _ComprobanteXsaManager = new ComprobanteXsaManager();
        #endregion

        // GET: FacturasEmitidas
        public ActionResult Index()
        {
            var sucursalId = ObtenerSucursal();

            var facturasEmitidasModel = new FacturasEmitidasModel
            {
                FechaInicial = DateTime.Now.AddDays(-6), // SE RESTA 6 DIAS PARA MOSTRAR EL RANGO DE FACTURAS GENERADAS EN UN SEMANA
                FechaFinal = DateTime.Now,
                SucursalId = ObtenerSucursal(),
            };

            List<facturareferencia> listaFacturaReferencia = new List<facturareferencia>();
            _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            bool isEmpty = facturasEmitidasModel.FacturaEmitidasTemporal.Any();
            if (isEmpty)
            {
                if (facturasEmitidasModel.SucursalId == 42)
                {
                    foreach (var facturasEmitidas in facturasEmitidasModel.FacturaEmitidasTemporal)
                    {
                        //get status
                        FacturaEmitida facturaEmitStatus = _db.FacturasEmitidas.Find(facturasEmitidas.Id);
                        facturasEmitidas.Status = facturaEmitStatus.Status;
                        facturareferencia queryFacturas = facturaidferencia(facturasEmitidas.Id);
                        if (queryFacturas != null)
                        {

                            facturasEmitidas.Referencia = queryFacturas.ReferenciaAddenda;
                            //facturasEmitidas.Status = queryFacturas.Status;
                            // facturasEmitidas.TotalImpRetenidos = queryFacturas.TotalImpuestoRetenidos;
                            // facturasEmitidas.TotalImpTrasladados = queryFacturas.TotalImpuestoTrasladado;
                        }

                    }
                }
                else
                {
                    foreach (var facturasEmitidas in facturasEmitidasModel.FacturaEmitidasTemporal)
                    {
                        //get status
                        FacturaEmitida facturaEmitStatus = _db.FacturasEmitidas.Find(facturasEmitidas.Id);
                        facturasEmitidas.Status = facturaEmitStatus.Status;
                    }
                }

            }

            ViewBag.Controller = "FacturasEmitidas";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.NameHere = "cfdi";

            return View(facturasEmitidasModel);
        }

        [HttpPost]
        public ActionResult Index(FacturasEmitidasModel facturasEmitidasModel)
        {
            bool isEmpty;
            if (ModelState.IsValid)
            {
                _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            }
            /*else
            {
                _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            }*/
            isEmpty = facturasEmitidasModel.FacturaEmitidasTemporal.Any();

            if (isEmpty)
            {
                if (facturasEmitidasModel.SucursalId == 42)
                {
                    foreach (var facturasEmitidas in facturasEmitidasModel.FacturaEmitidasTemporal)
                    {
                        //get status
                        FacturaEmitida facturaEmitStatus = _db.FacturasEmitidas.Find(facturasEmitidas.Id);
                        facturasEmitidas.Status = facturaEmitStatus.Status;
                        facturareferencia queryFacturas = facturaidferencia(facturasEmitidas.Id);
                        if (queryFacturas != null)
                        {
                            facturasEmitidas.Referencia = queryFacturas.ReferenciaAddenda;
                            //facturasEmitidas.Status = queryFacturas.Status;
                            //facturasEmitidas.TotalImpRetenidos = queryFacturas.TotalImpuestoRetenidos;
                            // facturasEmitidas.TotalImpTrasladados = queryFacturas.TotalImpuestoTrasladado;
                        }
                    }
                }
                else
                {
                    foreach (var facturasEmitidas in facturasEmitidasModel.FacturaEmitidasTemporal)
                    {
                        //get status
                        FacturaEmitida facturaEmitStatus = _db.FacturasEmitidas.Find(facturasEmitidas.Id);
                        facturasEmitidas.Status = facturaEmitStatus.Status;
                    }
                }
            }
            ViewBag.Controller = "FacturasEmitidas";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.NameHere = "cfdi";
            return View(facturasEmitidasModel);
        }

        public ActionResult ReporteFacturasEmitidas()
        {
            var sucursalId = ObtenerSucursal();
            bool isEmpty;
            var facturasEmitidasModel = new FacturasEmitidasModel
            {
                FechaInicial = DateTime.Now.AddDays(-5), // SE RESTA 6 DIAS PARA MOSTRAR EL RANGO DE FACTURAS GENERADAS EN UN SEMANA
                FechaFinal = DateTime.Now,
                SucursalId = ObtenerSucursal(),
            };

            _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);

            isEmpty = facturasEmitidasModel.FacturaEmitidasTemporal.Any();

            if (isEmpty)
            {
                if (facturasEmitidasModel.SucursalId == 42)
                {
                    foreach (var facturasEmitidas in facturasEmitidasModel.FacturaEmitidasTemporal)
                    {
                        facturareferencia queryFacturas = facturaidferencia(facturasEmitidas.Id);
                        if (queryFacturas != null)
                        {
                            facturasEmitidas.Referencia = queryFacturas.ReferenciaAddenda;
                            //facturasEmitidas.TotalImpRetenidos = queryFacturas.TotalImpuestoRetenidos;
                            //facturasEmitidas.TotalImpTrasladados = queryFacturas.TotalImpuestoTrasladado;
                        }
                    }
                }
            }

            ViewBag.Controller = "FacturasEmitidas";
            ViewBag.Action = "ReporteFacturasEmitidas";
            ViewBag.ActionES = "Reporte Facturas Emitidas";
            ViewBag.NameHere = "reportes";

            return View(facturasEmitidasModel);
        }

        [HttpPost]
        public ActionResult ReporteFacturasEmitidas(FacturasEmitidasModel facturasEmitidasModel)
        {
            bool isEmpty;
            if (ModelState.IsValid)
            {
                _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            }
            isEmpty = facturasEmitidasModel.FacturaEmitidasTemporal.Any();

            if (isEmpty)
            {
                if (facturasEmitidasModel.SucursalId == 42)
                {
                    foreach (var facturasEmitidas in facturasEmitidasModel.FacturaEmitidasTemporal)
                    {
                        facturareferencia queryFacturas = facturaidferencia(facturasEmitidas.Id);
                        if (queryFacturas != null)
                        {
                            facturasEmitidas.Referencia = queryFacturas.ReferenciaAddenda;
                            //facturasEmitidas.TotalImpRetenidos = queryFacturas.TotalImpuestoRetenidos;
                            //facturasEmitidas.TotalImpTrasladados = queryFacturas.TotalImpuestoTrasladado;
                        }
                    }
                }
            }

            ViewBag.Controller = "FacturasEmitidas";
            ViewBag.Action = "ReporteFacturasEmitidas";
            ViewBag.ActionES = "Reporte Facturas Emitidas";
            ViewBag.NameHere = "reportes";

            return View(facturasEmitidasModel);
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
        public ActionResult DescargaXML(int id)
        {
            //get xml
            var facturaEmtida = _db.FacturasEmitidas.Find(id);
            var pathCompleto = _descargasManager.GeneraFilePathXml(facturaEmtida.ArchivoFisicoXml, facturaEmtida.Serie, facturaEmtida.Folio);
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

        public ActionResult DescargaPDF(int id)
        {
            //objetos version 4.0
            ComprobanteCFDI oComprobante = new ComprobanteCFDI();
            //objetos version 3.3
            ComprobanteCFDI33 oComprobante33 = new ComprobanteCFDI33();
            string tipoDocumento = null;
            bool isXsa = false;
            byte[] archivoFisico = new byte[255];
            var facturaEmitida = _db.FacturasEmitidas.Find(id);
            //buscar si la factura se genero dentro del sistema
            var cfdiNormal = _db.ComprobantesCfdi.Where(n => n.FacturaEmitidaId == id).FirstOrDefault();
            var pago = _db.ComplementosPago.Where(p => p.FacturaEmitidaId == id).FirstOrDefault();
            var cartaPorte = _db.ComplementoCartaPortes.Where(cp => cp.FacturaEmitidaId == id).FirstOrDefault();
            if (cfdiNormal == null && pago == null && cartaPorte == null && facturaEmitida != null)
            {
                //dowload pdf in XSA
                archivoFisico = _ComprobanteXsaManager.DownloadPDFXsa(id, true);

            }
            else
            {
                if (facturaEmitida.Emisor.Txsa)
                {
                    if (cfdiNormal != null)
                    {
                        //dowload pdf in XSA
                        archivoFisico = _ComprobanteXsaManager.DownloadPDFXsa(id, false);
                        isXsa = true;
                    }
                }
                if (!isXsa)
                {
                    //busca version del CFDI del archivo
                    string CadenaXML = System.Text.Encoding.UTF8.GetString(facturaEmitida.ArchivoFisicoXml);
                    string versionCfdi = _decodifica.LeerValorXML(CadenaXML, "Version", "Comprobante");
                    if (versionCfdi == "3.3")
                    {
                        oComprobante33 = _decodifica.DeserealizarXML33(facturaEmitida.ArchivoFisicoXml);
                        tipoDocumento = _decodifica.TipoDocumentoCfdi33(facturaEmitida.ArchivoFisicoXml);
                        archivoFisico = _descargasManager.GeneraPDF33(oComprobante33, tipoDocumento, id, true);
                    }
                    else
                    {
                        oComprobante = _decodifica.DeserealizarXML40(facturaEmitida.ArchivoFisicoXml);
                        tipoDocumento = _decodifica.TipoDocumentoCfdi40(facturaEmitida.ArchivoFisicoXml);
                        archivoFisico = _descargasManager.GeneraPDF40(oComprobante, tipoDocumento, id, true);
                    }
                }
            }
            MemoryStream ms = new MemoryStream(archivoFisico, 0, 0, true, true);
            string nameArchivo = facturaEmitida.Serie + "-" + facturaEmitida.Folio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
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
            var facturaEmitida = _db.FacturasEmitidas.Find(id);
            return PartialView("~/Views/FacturasEmitidas/_Cancelacion.cshtml", facturaEmitida);
        }

        [HttpPost]
        public ActionResult Cancelar(FacturaEmitida facturaEmitida)
        {
            PopulaMotivoCancelacion();
            string error = null;
            bool isXsa = false;
            List<DataCancelacionResponseXsaDto> dataXsa = new List<DataCancelacionResponseXsaDto>();
            var emitida = _db.FacturasEmitidas.Find(facturaEmitida.Id);
            emitida.FolioSustitucion = facturaEmitida.FolioSustitucion;
            emitida.MotivoCancelacion = facturaEmitida.MotivoCancelacion;
            var comprobante = _db.ComprobantesCfdi.Where(c => c.FacturaEmitidaId == facturaEmitida.Id).FirstOrDefault();
            var pagos = _db.ComplementosPago.Where(p => p.FacturaEmitidaId == facturaEmitida.Id).FirstOrDefault();
            var cartaPorte = _db.ComplementoCartaPortes.Where(cp => cp.FacturaEmitidaId == facturaEmitida.Id).FirstOrDefault();

            try
            {
                if (comprobante == null && pagos == null && cartaPorte == null && emitida != null)
                {
                    dataXsa = _ComprobanteXsaManager.Cancelar(emitida.Id, emitida.FolioSustitucion, emitida.MotivoCancelacion, true);
                }
                else
                {
                    if (emitida.Emisor.Txsa)
                    {
                        if (comprobante != null)
                        {
                            dataXsa = _ComprobanteXsaManager.Cancelar(facturaEmitida.Id, facturaEmitida.FolioSustitucion, facturaEmitida.MotivoCancelacion, false);
                            isXsa = true;
                        }
                    }
                    if (!isXsa)
                    {
                        _operacionesCfdisEmitidos.Cancelar(emitida);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;

            }
            if (error == null)
            {
                ViewBag.Success = "Proceso de cancelación finalizado con éxito.";
                ViewBag.Error = null;
            }
            else
            {
                ViewBag.Error = error;
                ViewBag.Success = null;
            }
            return PartialView("~/Views/FacturasEmitidas/_Cancelacion.cshtml", emitida);
        }

        public ActionResult DescargarAcuse(int id)
        {
            string xmlCancelacion = null;
            bool isXsa = false;
            byte[] byteXml = new byte[1024];
            var facturaEmitida = _db.FacturasEmitidas.Find(id);
            var comprobante = _db.ComprobantesCfdi.Where(c => c.FacturaEmitidaId == id).FirstOrDefault();
            var pagos = _db.ComplementosPago.Where(p => p.FacturaEmitidaId == id).FirstOrDefault();
            var cartaPorte = _db.ComplementoCartaPortes.Where(cp => cp.FacturaEmitidaId == id).FirstOrDefault();
            if (comprobante == null && pagos == null && cartaPorte == null && facturaEmitida != null)
            {
                byteXml = _ComprobanteXsaManager.DowloadAcuseCancelacion(facturaEmitida.Serie, facturaEmitida.Folio, facturaEmitida.Uuid, facturaEmitida.EmisorId);
            }
            else
            {
                if (facturaEmitida.Emisor.Txsa)
                {
                    if (comprobante != null)
                    {
                        byteXml = _ComprobanteXsaManager.DowloadAcuseCancelacion(facturaEmitida.Serie, facturaEmitida.Folio, facturaEmitida.Uuid, facturaEmitida.EmisorId);
                        isXsa = true;
                    }
                }
                if (!isXsa)
                {
                    xmlCancelacion = _descargasManager.DowloadAcuseCancelacion(facturaEmitida.EmisorId, facturaEmitida.ArchivoFisicoXml);
                }
            }
            if (xmlCancelacion != null)
            {
                byteXml = Encoding.UTF8.GetBytes(xmlCancelacion);
            }
            MemoryStream ms = new MemoryStream(byteXml, 0, 0, true, true);
            string nameArchivo = facturaEmitida.Serie + "-" + facturaEmitida.Folio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Response.AddHeader("content-disposition", "attachment;filename= " + nameArchivo + ".xml");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, "application/xml");
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

        public ActionResult ReportePagos() {
            var sucursalId = ObtenerSucursal();

            var facturasEmitidasModel = new FacturasEmitidasModel
            {
                FechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                FechaFinal = DateTime.Now,
                SucursalId = ObtenerSucursal(),
            };

            List<search_doc_rel_fac_emi> listaComplementosPagos = new List<search_doc_rel_fac_emi>();
            _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            bool isEmpty = facturasEmitidasModel.FacturasEmitidas.Any();
            if (isEmpty)
            {
                foreach (var facturasEmitidas in facturasEmitidasModel.FacturasEmitidas)
                {
                    search_doc_rel_fac_emi queryFacturas = queryFacturasPagadas(facturasEmitidas.Id);
                    if (queryFacturas != null && queryFacturas.FacturaEmitidaId != 0)
                    {
                        facturasEmitidas.FolioComplementoPago = queryFacturas.Folio;
                        facturasEmitidas.SerieComplementoPago = queryFacturas.Serie;
                        facturasEmitidas.FacturaComplementoPagoId = queryFacturas.Id;
                        facturasEmitidas.FacturaEmitidaPagada = true;
                    }

                }

            }
            ViewBag.Controller = "FacturasEmitidas";
            ViewBag.Action = "ReportePagos";
            ViewBag.ActionES = "Reporte Pago";
            ViewBag.NameHere = "reportes";
            return View(facturasEmitidasModel);
        }

        [HttpPost]
        public ActionResult ReportePagos(FacturasEmitidasModel facturasEmitidasModel)
        {
            bool isEmpty;
            if (!ModelState.IsValid)
            {
                //_operacionesCfdisEmitidos.ObtenerFacturasById(ref facturasEmitidasModel);
            }
            else
            {
                _operacionesCfdisEmitidos.ObtenerFacturas(ref facturasEmitidasModel);
            }
            isEmpty = facturasEmitidasModel.FacturasEmitidas.Any();
            if (isEmpty)
            {
                foreach (var facturasEmitidas in facturasEmitidasModel.FacturasEmitidas)
                {
                    search_doc_rel_fac_emi queryFacturas = queryFacturasPagadas(facturasEmitidas.Id);
                    if (queryFacturas != null && queryFacturas.FacturaEmitidaId != 0)
                    {
                        facturasEmitidas.FolioComplementoPago = queryFacturas.Folio;
                        facturasEmitidas.SerieComplementoPago = queryFacturas.Serie;
                        facturasEmitidas.FacturaComplementoPagoId = queryFacturas.Id;
                        facturasEmitidas.FacturaEmitidaPagada = true;
                    }
                }
            }
            ViewBag.Controller = "FacturasEmitidas";
            ViewBag.Action = "ReportePagos";
            ViewBag.ActionES = "Reporte Pago";
            ViewBag.NameHere = "reportes";
            return View(facturasEmitidasModel);
        }

        public search_doc_rel_fac_emi queryFacturasPagadas(int id)
        {
            var listRelTblSearch = new search_doc_rel_fac_emi();
            const string query = @"select IFNULL(cp.FacturaEmitidaId,0) as FacturaEmitidaId, fe.Folio,fe.Serie,cp.Id from ori_documentosrelacionados dr " +
                        "join ori_pagos p on(dr.PagoId = p.Id) " +
                        "join ori_complementospagos cp on(p.ComplementoPagoId = cp.Id) " +
                        "join ori_facturasemitidas fe on (cp.FacturaEmitidaId = fe.Id) " +
                        "where dr.FacturaEmitidaId in (@Id); ";

            var resultados = _db.Database.SqlQuery<search_doc_rel_fac_emi>(query,
                    new MySqlParameter { ParameterName = "@Id", MySqlDbType = MySqlDbType.String, Value = id }).FirstOrDefault();

            return resultados;
        }

        public class search_doc_rel_fac_emi
        {
            public int FacturaEmitidaId { get; set; }

            public string Folio { get; set; }

            public string Serie { get; set; }

            public int Id { get; set; }

        }




        public facturareferencia facturaidferencia(int id)
        {
            var listReferencia = new facturareferencia();
            const string query = @"select  cm.ReferenciaAddenda,  cm.TotalImpuestoTrasladado, cm.TotalImpuestoRetenidos , fe.Status from cp_complementocartaporte cm  " +
                        "join ori_facturasemitidas fe on (fe.Id= cm.FacturaEmitidaId) " +
                        "where fe.Id in (@Id); ";
           
            var consulta = _db.Database.SqlQuery<facturareferencia>(query,
                    new MySqlParameter { ParameterName = "@Id", MySqlDbType = MySqlDbType.String, Value = id }).FirstOrDefault();

            return consulta;

        }

        public class facturareferencia
        {
            public int FacturaEmitidaId { get; set; }

            public string ReferenciaAddenda { get; set; }

            public double TotalImpuestoTrasladado { get; set; }

            public double TotalImpuestoRetenidos { get; set; }

            public Status Status { get; set; }

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
      
    

    }
}
