using APBox.Context;
using API.Models.Dto;
using Aplicacion.LogicaPrincipal.Addenda;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilerias.LogicaPrincipal;

namespace APBox.Controllers.Operaciones
{
    public class AddendaController : Controller
    {
        #region Variables
            private readonly APBoxContext _db = new APBoxContext();
            private readonly ProcesaAddenda  _procesaAddenda = new ProcesaAddenda();
        #endregion
        // GET: Addenda
        public ActionResult Index()
        {
            var addendaDto = new AddendaDto() {
                Procesado = false,
                ConceptosAddenda = new List<ConceptoAddendaDto>()
            };
            ViewBag.Controller = "Addenda";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.NameHere = "addenda";
            return View(addendaDto);
        }
        [HttpPost]
        public ActionResult Index(AddendaDto addendaDto)
        {
            ViewBag.Controller = "Addenda";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.NameHere = "addenda";

            string archivo;
            ComprobanteCFDI comprobante = new ComprobanteCFDI();
            
            try
            {
                archivo = SubeArchivo(0);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                return View(addendaDto);
            }

            try
            {
                //Deserealiza XML
                comprobante = _procesaAddenda.DecodificaXML(archivo);
                
                //parseo del objeto
                addendaDto = _procesaAddenda.ParseoXml(comprobante);
                addendaDto.Procesado = true;
                addendaDto.PathArchivo = archivo;

            }
            catch (Exception ex)
            {
                var errores = ex.Message.Split('|');
                foreach (var error in errores)
                {
                    ModelState.AddModelError("", error);
                }
            }

        
            return View(addendaDto);

        }

       

        // POST: Addenda/CreateAddenda
        [HttpPost]
        public FileResult Create(AddendaDto addendaDto)
        {
            FacturaInterfactura oFacturaInterfactura = new FacturaInterfactura();
            String pathXml = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Archivos//Addendas//{0}-{1}.xml", "NodoAddenda", DateTime.Now.ToString("yyyyMMddHHmmssfff")); ;

                //add addenda serialize
                oFacturaInterfactura = _procesaAddenda.GeneraNodoAddenda(addendaDto);

                //serializa addenda
                String addendaFile = _procesaAddenda.SerealizaAddenda(oFacturaInterfactura, pathXml, addendaDto.PathArchivo);
                byte[] archivoBytes = System.IO.File.ReadAllBytes(addendaFile);
                string nombreArchivo = System.IO.Path.GetFileName(addendaFile);
            //delete file xml
            System.IO.File.Delete(addendaFile);
            return File(archivoBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivo);
            
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

                    var pathDestino = Path.Combine(Server.MapPath("~/Archivos/Addendas/"), archivo.FileName);
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
