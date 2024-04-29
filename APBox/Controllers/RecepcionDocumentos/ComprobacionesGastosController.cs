using APBox.Context;
using APBox.Control;
using API.Models.Dto;
using API.Operaciones.OperacionesProveedores;
using API.Operaciones.OperacionesRecepcion;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using Utilerias.LogicaPrincipal;

namespace APBox.Controllers.RecepcionDocumentos
{
    [SessionExpire]
    public class ComprobacionesGastosController : Controller
    {
        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        // GET: ComprobacionesGastos
        public ActionResult Index()
        {
            ViewBag.Controller = "ComprobacionesGastos";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Title = "Comprobaciones de Gastos";

            var sucursalId = ObtenerSucursal();
            var comprobacionesGastos = _db.ComprobacionGasto.Where(c => c.SucursalId == sucursalId).ToList();
            return View(comprobacionesGastos);
        }

        // GET: ComprobacionesGastos/Create
        public ActionResult Create()
        {
            ViewBag.Controller = "ComprobacionesGastos";
            ViewBag.Action = "Index";
            ViewBag.Title = "Comprobaciones de Gastos";

            var sucursalId = ObtenerSucursal();
            var usuarioId = ObtenerUsuario();
            PopulaProyectos();
            PopulaEventos();
            var ultimoFolioSucursal = _db.ComprobacionGasto.Where(c => c.SucursalId == sucursalId).OrderByDescending(c => c.Folio).FirstOrDefault();
            
            //Se manda prellenar el folio con el siguiente numero
            int? folio;
            if (ultimoFolioSucursal == null)
            {
                folio = 1;
            }
            else
            {
                folio = ultimoFolioSucursal.Folio + 1;
            }

            var comprobacionGasto = new ComprobacionGasto
            {
                SucursalId = sucursalId,
                Folio = folio,
                Fecha = DateTime.Now,
                UsuarioId = usuarioId,
                MonedaId = API.Enums.c_Moneda.MXN,
            };

            return View(comprobacionGasto);
        }

        // POST: ComprobacionesGastos/Create
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

        // GET: ComprobacionesGastos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComprobacionesGastos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ComprobacionesGastos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComprobacionesGastos/Delete/5
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

        public static string FechaFormat(string fecha)
        {
            DateTime fechaConvertDate = Convert.ToDateTime(fecha);
            string fechaFormat = fechaConvertDate.ToString("dd/MM/yyyy");
            return fechaFormat;
        }

        private void PopulaProyectos()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.Proyectos = popularDropDowns.PopulaProyectos(ObtenerSucursal());
        }

        private void PopulaEventos()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.Eventos = popularDropDowns.PopulaEventos(ObtenerSucursal());
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

        #endregion Validaciones

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
