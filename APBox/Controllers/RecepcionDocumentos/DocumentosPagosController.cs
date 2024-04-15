using APBox.Context;
using APBox.Control;
using API.Enums;
using API.Models.DocumentosRecibidos;
using API.Models.Dto;
using API.Operaciones.OperacionesProveedores;
using Aplicacion.LogicaPrincipal.Correos;
using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using Aplicacion.LogicaPrincipal.Facturas;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Utilerias.LogicaPrincipal;
using System.Data.Entity;
using System.Web;
using Aplicacion.LogicaPrincipal.CargasMasivas.CSV;
using API.Models.DocumentosPagos;
using Aplicacion.LogicaPrincipal.DocumentosPagos;
using Aplicacion.RecepcionDocumentos;

namespace APBox.Controllers.Operaciones
{
    [SessionExpire]
    public class DocumentosPagosController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();
        private readonly ProcesaDocumentoRecibido _procesaDocumentoRecibido = new ProcesaDocumentoRecibido();
        private readonly Decodificar _decodifica = new Decodificar();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();
        private readonly CargaPagosDR _cargaPagosDR = new CargaPagosDR();
        private readonly ProcesaDocumentoPago _procesaDocumentoPago = new ProcesaDocumentoPago();
        private readonly ValidacionesPagos _validaPagos = new ValidacionesPagos();

        // GET: DocumentosPagos
        public ActionResult Index()
        {
            ViewBag.Controller = "DocumentosAprobados";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Índice";
            ViewBag.Button = "CargaDocumentoRecibido";
            ViewBag.NameHere = "Documentos Aprobados";
            //get usaurio
            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Usuarios.Find(ObtenerSucursal());

            var documentosRecibidosModel = new DocumentosRecibidosModel();
            var fechaInicial = DateTime.Today.AddDays(-10);
            var fechaFinal = DateTime.Today.AddDays(1).AddTicks(-1);
            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;

            documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, sucursal.Id, usuario.Id);

            return View(documentosRecibidosModel);
        }

        // POST: DocumentosPagos filtrados por el rango de fecha
        [HttpPost]
        public ActionResult Index(DocumentosRecibidosModel documentosRecibidosModel)
        {
            ViewBag.Controller = "DocumentosAprobados";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "CargaDocumentoRecibido";
            ViewBag.NameHere = "Documentos Aprobados";
            //get usaurio
            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Usuarios.Find(ObtenerSucursal());

            DateTime fechaI = documentosRecibidosModel.FechaInicial;
            DateTime fechaF = documentosRecibidosModel.FechaFinal;
            var fechaInicial = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 0, 0, 0);
            var fechaFinal = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 59);

            documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, sucursal.Id, usuario.Id);

            return View(documentosRecibidosModel);
        }

        // GET: DocumentosPagos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DocumentosPagos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DocumentosPagos/Edit/5
        public ActionResult Revision(int id)
        {
            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "Edit";
            ViewBag.ActionES = "Editar";
            ViewBag.NameHere = "Revisión de Comprobante Recibido para Pago";

            var documentoRecibido = _db.DocumentoRecibidoDr.Find(id);
            var usuario = _db.Usuarios.Find(ObtenerUsuario());

            if (usuario.esProveedor)
            {
                documentoRecibido.isProveedor = true;
                ViewBag.isProveedor = "Proveedor";
            }
            else
            {
                documentoRecibido.isProveedor = false;
                ViewBag.isProveedor = "Usuario";
            }
            // Splitting the string into lines
            string[] lines = documentoRecibido.Validaciones_Detalle.Split('\n');
            documentoRecibido.DetalleArrays = lines.ToList();

            return View(documentoRecibido);
        }

        // POST: DocumentosPagos/Edit/5
        [HttpPost]
        public ActionResult Revision(DocumentosRecibidosDR documentoRecibidoEdit)
        {
            try
            {
                var usuario = _db.Usuarios.Find(ObtenerUsuario());
                var documentoRecibido = _db.DocumentoRecibidoDr.Find(documentoRecibidoEdit.Id);
                var usuarioEntrega = _db.Usuarios.Find(documentoRecibido.AprobacionesDR.UsuarioEntrega_Id);
                var sucursal = _db.Sucursales.Find(ObtenerSucursal());

                documentoRecibido.EstadoComercial = documentoRecibidoEdit.EstadoComercial;
                documentoRecibido.EstadoPago = documentoRecibidoEdit.EstadoPago;
                documentoRecibido.AprobacionesDR.Observaciones = documentoRecibidoEdit.AprobacionesDR.Observaciones;

                if (documentoRecibidoEdit.EstadoComercial == c_EstadoComercial.EnRevision && documentoRecibidoEdit.AprobacionesDR.Observaciones != null)
                {
                    var usuarioAprobador = _db.Usuarios.Find(documentoRecibido.AprobacionesDR.UsuarioAprobacionComercial_id);

                    documentoRecibido.AprobacionesDR.FechaAprobacionComercial = DateTime.Now;
                    documentoRecibido.AprobacionesDR.UsuarioAprobacionComercial_id = usuario.Id;
                    _envioEmail.NotificacionRevisionComercial(usuarioAprobador, documentoRecibido, sucursal.Id);
                }

                if (documentoRecibidoEdit.EstadoPago == c_EstadoPago.Aprobado)
                {
                    documentoRecibido.AprobacionesDR.FechaAprobacionPagos = DateTime.Now;
                    documentoRecibido.AprobacionesDR.UsuarioAprobacionPagos_id = usuario.Id;
                    documentoRecibido.EstadoPago = c_EstadoPago.Aprobado;
                }

                if (documentoRecibidoEdit.EstadoPago == c_EstadoPago.EnRevision)
                {
                    documentoRecibido.AprobacionesDR.FechaAprobacionPagos = null;
                    documentoRecibido.AprobacionesDR.UsuarioAprobacionPagos_id = null;
                    documentoRecibido.EstadoPago = c_EstadoPago.EnRevision;
                }

                if (documentoRecibidoEdit.EstadoPago == c_EstadoPago.Rechazado)
                {
                    documentoRecibido.AprobacionesDR.FechaRechazo = DateTime.Now;
                    documentoRecibido.AprobacionesDR.UsuarioRechazo_id = usuario.Id;
                    documentoRecibido.AprobacionesDR.DetalleRechazo = documentoRecibidoEdit.AprobacionesDR.DetalleRechazo;

                    documentoRecibido.EstadoPago = c_EstadoPago.Rechazado;

                    //Notificación al usuario que entrega
                    _envioEmail.NotificacionCambioEstadoComercial(usuarioEntrega, documentoRecibido, c_EstadoComercial.Rechazado, (int)ObtenerSucursal());
                }

                _db.Entry(documentoRecibido).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Index", "DocumentosPagos");
            }
            catch
            {
                return View(documentoRecibidoEdit);
            }
        }

        public ActionResult Exportar()
        {
            var pathCompleto = _cargaPagosDR.Exportar();
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

        public ActionResult Pagos()
        {
            ViewBag.Controller = "DocumentosPagos";
            ViewBag.Action = "Pagos";
            ViewBag.ActionES = "Pagos";
            ViewBag.NameHere = "Pagos Procesados";

            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Sucursales.Find(ObtenerSucursal());
            DocumentosPagosModel pagosModel = new DocumentosPagosModel();
            DateTime dayI = DateTime.Now.AddDays(-6);
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, dayI.Day, 0, 0, 0);
            var fechaFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            pagosModel.FechaInicial = fechaInicial;
            pagosModel.FechaFinal = fechaFinal;
            pagosModel.Pagos = _procesaDocumentoPago.Filtrar(fechaInicial, fechaFinal, false, null, sucursal.Id);

            return View(pagosModel);
        }

        [HttpPost]
        public ActionResult Pagos(DocumentosPagosModel pagosModel)
        {
            ViewBag.Controller = "DocumentosPagos";
            ViewBag.Action = "Pagos";
            ViewBag.ActionES = "Pagos";
            ViewBag.NameHere = "Pagos Procesados";

            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Sucursales.Find(ObtenerSucursal());
            DateTime fechaI = pagosModel.FechaInicial;
            DateTime fechaF = pagosModel.FechaFinal;

            if (usuario.esProveedor)
            {
                pagosModel.Pagos = _procesaDocumentoPago.Filtrar(fechaI, fechaF, true, usuario.SocioComercialID, sucursal.Id);
            }
            else
            {
                pagosModel.Pagos = _procesaDocumentoPago.Filtrar(fechaI, fechaF, false, null, sucursal.Id);
            }

            var fechaInicial = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 0, 0, 0);
            var fechaFinal = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 59);
            pagosModel.FechaInicial = fechaInicial;
            pagosModel.FechaFinal = fechaFinal;
            pagosModel.Pagos = _procesaDocumentoPago.Filtrar(fechaInicial, fechaFinal, false, null, sucursal.Id);

            return View(pagosModel);
        }

        public ActionResult CargaLayout()
        {
            ViewBag.Controller = "DocumentosPagos";
            ViewBag.Action = "CargaLayout";
            ViewBag.ActionES = "Carga Layout";
            ViewBag.NameHere = "Carga Layout de Pagos";

            DocumentosPagosModel documentoPagoModel = new DocumentosPagosModel();
            documentoPagoModel.Previsualizacion = true;

            return View(documentoPagoModel);
        }

        [HttpPost]
        public ActionResult CargaLayout(DocumentosPagosModel documentoPagoModel)
        {
            ViewBag.Controller = "DocumentosPagos";
            ViewBag.Action = "CargaLayout";
            ViewBag.ActionES = "Carga Layout";
            ViewBag.NameHere = "Carga Layout de Pagos";

            String archivo;
            try
            {
                archivo = SubeArchivo(0);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                return View(documentoPagoModel);
            }

            try
            {
                var usuario = _db.Usuarios.Find(ObtenerUsuario());
                documentoPagoModel.Pagos = _cargaPagosDR.Importar(archivo, ObtenerSucursal(), documentoPagoModel.Previsualizacion, usuario.Id);
                if (!documentoPagoModel.Previsualizacion)
                {
                    return RedirectToAction("Pagos");
                }
            }
            catch (Exception ex)
            {
                var errores = ex.Message.Split('|');
                foreach (var error in errores)
                {
                    ModelState.AddModelError("", error);
                }
            }

            documentoPagoModel.Previsualizacion = false;

            return View(documentoPagoModel);
        }

        public ActionResult CargaComplementoPago(int Id)
        {
            ViewBag.Controller = "DocumentosPagos";
            ViewBag.Action = "Carga Complemento de Pago";
            ViewBag.ActionES = "Carga Complemento de Pago";
            ViewBag.NameHere = "Carga complemento de Pago";
            var pago = _db.PagoDr.Find(Id);
            pago.Procesado = false;

            return View(pago);
        }

        [HttpPost]
        public ActionResult CargaComplementoPago(PagosDR pagoDR)
        {
            ViewBag.Controller = "DocumentosPagos";
            ViewBag.Action = "Carga Complemento de Pago";
            ViewBag.ActionES = "Carga Complemento de Pago";
            ViewBag.NameHere = "Carga complemento de Pago";

            ComprobanteCFDI cfdi = new ComprobanteCFDI();
            ComplementoPagoDto pagoDto = new ComplementoPagoDto();
            String archivo;
            try
            {
                archivo = SubeArchivo(0);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                return View(pagoDR);
            }

            var pago = _db.PagoDr.Find(pagoDR.Id);
            try
            {
                cfdi = _procesaDocumentoRecibido.DecodificaXML(archivo);
                if (cfdi != null)
                {
                    pago.ComplementoPago = new ComplementoPagoDto();
                    pago.ComplementoPago.NombreEmisor = cfdi.Emisor.Nombre;
                    pago.ComplementoPago.NombreReceptor = cfdi.Receptor.Nombre;
                    if (cfdi.Pagos.Pago.Length > 1)
                    {
                        ModelState.AddModelError("", "Error: se encontraron mas de 1 pago realizado");
                    }
                    foreach (var pagoXml in cfdi.Pagos.Pago)
                    {
                        pago.ComplementoPago.FormaPago = pagoXml.FormaDePagoP;
                        pago.ComplementoPago.TipoComprobante = cfdi.TipoDeComprobante;
                        pago.ComplementoPago.TipoCambio = pagoXml.TipoCambioP;
                        pago.ComplementoPago.Moneda = pagoXml.MonedaP;
                        pago.ComplementoPago.Monto = pagoXml.Monto;
                        pago.ComplementoPago.Serie = cfdi.Serie;
                        pago.ComplementoPago.Folio = cfdi.Folio;
                        pago.ComplementoPago.Uuid = cfdi.TimbreFiscalDigital.UUID;
                        pago.ComplementoPago.FechaPago = pagoXml.FechaPago.ToString("dd/MM/yyyy");
                    }
                    pago.Detalle_Validacion = _validaPagos.ValidaComplementoPago(pago, cfdi);
                }
                pago.Procesado = true;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", String.Format("Error: {0}", ex.Message));
                return View(pagoDR);
            }
            return View(pago);
        }

        public ActionResult EstadoPago(PagosDR pagoDR)
        {
            try
            {
                var pago = _db.PagoDr.Find(pagoDR.Id);
                var usuario = _db.Usuarios.Find(ObtenerUsuario());
                var documentoRecibido = _db.DocumentoRecibidoDr.Find(pago.ComplementoPagoRecibido_Id);

                documentoRecibido.EstadoPago = c_EstadoPago.Completado;
                documentoRecibido.AprobacionesDR.FechaCompletaPagos = DateTime.Now;
                documentoRecibido.AprobacionesDR.UsuarioCompletaPagos_id = usuario.Id;

                _db.Entry(documentoRecibido).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("ComplementosPagosCargados");
            }
            catch
            {
                return View("CargaComplementoPago", pagoDR.Id);
            }
        }

        // GET: DocumentosPagos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DocumentosPagos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Pagos");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteConfirmed(int id)
        {
            var pago = _db.PagoDr.Find(id);
            if (pago == null)
            {
                // Manejar el caso en el que no se encuentra ningún pago con el ID proporcionado
                return HttpNotFound(); // O puedes devolver una vista de error personalizada
            }

            // Buscar y eliminar los registros en DocumentosPagados asociados al pago
            var documentosPagados = _db.DocumentoPagadoDr.Where(doc => doc.Pago_Id == id);
            _db.DocumentoPagadoDr.RemoveRange(documentosPagados);

            // Actualizar los documentos recibidos relacionados a los documentos pagados eliminados
            foreach (var documentoPagado in documentosPagados)
            {
                var documentoRecibido = _db.DocumentoRecibidoDr.FirstOrDefault(doc => doc.CfdiRecibidos_UUID == documentoPagado.UUID);
                if (documentoRecibido != null)
                {
                    //Regresa el estado de Pagado a En Revisión
                    documentoRecibido.EstadoPago = c_EstadoPago.EnRevision;
                }
            }

            _db.PagoDr.Remove(pago);
            _db.SaveChanges();

            return RedirectToAction("Pagos");
        }

        #region Validaciones

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        public static string FechaFormat(string fecha)
        {
            DateTime fechaConvertDate = Convert.ToDateTime(fecha);
            string fechaFormat = fechaConvertDate.ToString("dd/MM/yyyy");
            return fechaFormat;
        }

        public ConfiguracionesDR ConfiguracionEmpresa()
        {
            var sucursalId = _db.Sucursales.Find(ObtenerSucursal());
            var configuracion = _db.config.FirstOrDefault(c => c.Sucursal_Id == sucursalId.Id);

            if (configuracion == null)
            {
                return null;
            }

            return configuracion;
        }

        private void PopulaEstadoComercial()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "En Revision", Value = "0", Selected = true });
            items.Add(new SelectListItem { Text = "Aprobado", Value = "1" });
            items.Add(new SelectListItem { Text = "Rechazado", Value = "2" });
            ViewBag.estadoComercial = items;
        }

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private int ObtenerUsuario()
        {
            return Convert.ToInt32(Session["UsuarioId"]);
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

                    var pathDestino = Path.Combine(Server.MapPath("~/Archivos/DocumentosProveedores/"), archivo.FileName);
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

        public ActionResult DescargaXml(int id)
        {
            byte[] archivoFisico = new byte[255];
            var documentoRecibido = _db.DocumentoRecibidoDr.Find(id);
            archivoFisico = documentoRecibido.RecibidosXml.Archivo;

            MemoryStream ms = new MemoryStream(archivoFisico, 0, 0, true, true);
            string nameArchivo = documentoRecibido.CfdiRecibidos_Serie + "-" + documentoRecibido.CfdiRecibidos_Folio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Response.AddHeader("content-disposition", "attachment;filename= " + nameArchivo + ".xml");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, "application/xml");
        }

        public ActionResult DescargaPdf(int id)
        {
            byte[] archivoFisico = new byte[255];
            var documentoRecibido = _db.DocumentoRecibidoDr.Find(id);
            archivoFisico = documentoRecibido.RecibidosPdf.Archivo;

            MemoryStream ms = new MemoryStream(archivoFisico, 0, 0, true, true);
            string nameArchivo = documentoRecibido.CfdiRecibidos_Serie + "-" + documentoRecibido.CfdiRecibidos_Folio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Response.AddHeader("content-disposition", "attachment;filename= " + nameArchivo + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public ActionResult DescargaAdjunto(int id)
        {
            var documentoRecibido = _db.DocumentoRecibidoDr.Find(id);
            byte[] archivoFisicoXml = documentoRecibido.RecibidosXml.Archivo;
            byte[] archivoFisicoPdf = documentoRecibido.RecibidosPdf.Archivo;
            string nameArchivo = documentoRecibido.CfdiRecibidos_Serie + "-" + documentoRecibido.CfdiRecibidos_Folio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                AddFileToZip(archive, archivoFisicoXml, nameArchivo + ".xml");
                AddFileToZip(archive, archivoFisicoPdf, nameArchivo + ".pdf");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            var fileStreamResult = new FileStreamResult(memoryStream, "application/zip")
            {
                FileDownloadName = "adjuntos.zip"
            };

            return fileStreamResult;
        }

        private void AddFileToZip(ZipArchive archive, byte[] fileBytes, string entryName)
        {
            var entry = archive.CreateEntry(entryName);
            using (var entryStream = entry.Open())
            {
                entryStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        #endregion Validaciones

    }
}