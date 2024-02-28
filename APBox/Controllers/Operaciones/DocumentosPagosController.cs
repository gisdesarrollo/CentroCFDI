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


namespace APBox.Controllers.Operaciones
{
    [SessionExpire]
    public class DocumentosPagosController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();
        private readonly ProcesaDocumentoRecibido _procesaDocumentoRecibido = new ProcesaDocumentoRecibido();
        private readonly Decodificar _decodifica = new Decodificar();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();

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

            if (usuario.esProveedor)
            {
                ViewBag.isProveedor = "Proveedor";
            }
            else
            {
                ViewBag.isProveedor = "Usuario";
            }


            var documentosRecibidosModel = new DocumentosRecibidosModel();
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var fechaFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;
            if (usuario.esProveedor)
            {
                documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, usuario.Id, (int)usuario.SocioComercialID);
            }
            else
            {
                documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, usuario.Id, null);
            }
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
            if (usuario.esProveedor)
            {
                ViewBag.isProveedor = "Proveedor";
            }
            else
            {
                ViewBag.isProveedor = "Usuario";
            }
            DateTime fechaI = documentosRecibidosModel.FechaInicial;
            DateTime fechaF = documentosRecibidosModel.FechaFinal;

            var fechaInicial = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 0, 0, 0);
            var fechaFinal = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 59);
            if (usuario.esProveedor)
            {
                documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, usuario.Id, (int)usuario.SocioComercialID);
            }
            else
            {
                documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, usuario.Id, null);
            }

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

        // GET: DocumentosRecibidos/Edit/5
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
            TempData["AprobadorId"] = null;
            TempData["DepartamentoId"] = null;

            return View(documentoRecibido);
        }

        // POST: DocumentosRecibidos/Edit/5
        [HttpPost]
        public ActionResult Revision(DocumentosRecibidosDR documentoRecibidoEdit)
        {
            try
            {
                var usuario = _db.Usuarios.Find(ObtenerUsuario());
                var documentoRecibido = _db.DocumentoRecibidoDr.Find(documentoRecibidoEdit.Id);
                if (usuario.esProveedor)
                {
                    // Obtener los datos guardados en TempData
                    var aprobadorId = TempData["AprobadorId"] as int?;
                    var departamentoId = TempData["DepartamentoId"] as int?;
                    if (aprobadorId != null && departamentoId != null)
                    {
                        documentoRecibido.Aprobador_Id = aprobadorId.Value;
                        documentoRecibido.Departamento_Id = departamentoId.Value;
                        _db.Entry(documentoRecibido).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
                else
                {
                    if (documentoRecibidoEdit.EstadoComercial == c_EstadoComercial.Rechazado)
                    {
                        documentoRecibido.MotivoRechazo = documentoRecibidoEdit.MotivoRechazo;
                    }
                    documentoRecibido.Solicitudes = null;
                    documentoRecibido.Solicitud_Id = null;
                    documentoRecibido.EstadoComercial = documentoRecibidoEdit.EstadoComercial;
                    _db.Entry(documentoRecibido).State = EntityState.Modified;
                    _db.SaveChanges();

                    //envio email rechazo
                    if (documentoRecibidoEdit.EstadoComercial == c_EstadoComercial.Rechazado)
                    {
                        _envioEmail.SendEmailNotifications(null, documentoRecibido, true, (int)ObtenerSucursal());
                    }
                }
                return RedirectToAction("Index", "DocumentosRecibidos");
            }
            catch
            {
                return View(documentoRecibidoEdit);
            }
        }

        // GET: DocumentosRecibidos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DocumentosRecibidos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        #region Validaciones

        public ActionResult AprobarEstadoComercial(int id)
        {
            // Obtener el documento recibido con el ID proporcionado 
            // y asignarlo a la variable tuObjeto
            var tuObjeto = _db.DocumentoRecibidoDr.Find(id);

            // Verificar si el objeto no es nulo
            if (tuObjeto != null)
            {
                // Cambiar la propiedad EstadoComercial a aprobado
                tuObjeto.EstadoComercial = c_EstadoComercial.Aprobado;

                // Guardar los cambios 
                _db.SaveChanges();

                // Redirigir al método Index
                return RedirectToAction("Index");
            }

            // Manejar el caso en que el objeto no se encuentre
            return HttpNotFound();
        }

        public ActionResult RechazarEstadoComercial(int id)
        {
            // Obtener el documento recibido con el ID proporcionado 
            // y asignarlo a la variable tuObjeto
            var tuObjeto = _db.DocumentoRecibidoDr.Find(id);

            // Verificar si el objeto no es nulo
            if (tuObjeto != null)
            {
                // Cambiar la propiedad EstadoComercial a aprobado
                tuObjeto.EstadoComercial = c_EstadoComercial.Rechazado;

                // Guardar los cambios 
                _db.SaveChanges();

                // Redirigir al método Index
                return RedirectToAction("Index");
            }

            // Manejar el caso en que el objeto no se encuentre
            return HttpNotFound();
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
        private PathArchivosDto SubeArchivo()
        {
            PathArchivosDto pathArchivos = new PathArchivosDto();
            if (Request.Files.Count > 0)
            {
                var operacionesStreams = new OperacionesStreams();
                for (var r = 0; r < Request.Files.Count; r++)
                {
                    var archivo = Request.Files[r];
                    var extencion = Path.GetExtension(archivo.FileName);
                    if (extencion == ".xml" || extencion == ".XML")
                    {
                        var pathDestinoXml = Path.Combine(Server.MapPath("~/Archivos/DocumentosProveedores/"), archivo.FileName);
                        Stream fileStream = archivo.InputStream;
                        var mStreamer = new MemoryStream();
                        mStreamer.SetLength(fileStream.Length);
                        fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
                        mStreamer.Seek(0, SeekOrigin.Begin);
                        operacionesStreams.StreamArchivo(mStreamer, pathDestinoXml);

                        pathArchivos.PathDestinoXml = pathDestinoXml;
                    }
                    if (extencion == ".pdf" || extencion == ".PDF")
                    {
                        var pathDestinoPdf = Path.Combine(Server.MapPath("~/Archivos/DocumentosProveedores/"), archivo.FileName);
                        Stream fileStream = archivo.InputStream;
                        var mStreamer = new MemoryStream();
                        mStreamer.SetLength(fileStream.Length);
                        fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
                        mStreamer.Seek(0, SeekOrigin.Begin);
                        operacionesStreams.StreamArchivo(mStreamer, pathDestinoPdf);

                        pathArchivos.PathDestinoPdf = pathDestinoPdf;
                    }

                }

                return pathArchivos;
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
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
