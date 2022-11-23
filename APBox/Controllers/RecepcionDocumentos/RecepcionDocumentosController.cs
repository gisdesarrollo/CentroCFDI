using BusApBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace APBox.Controllers.RecepcionDocumentos
{
    public class RecepcionDocumentosController : Controller
    {
        
        // GET: RecepcionDocumentos
        public ActionResult CargarDocumentosExternos()
        {
            ViewBag.Controller = "RecepcionDocumentos";
            ViewBag.Action = "CargarDocumentosExternos";
            ViewBag.ActionES = "Carga Documentos Externos";
            ViewBag.NameHere = "cfdi";
            return View();
        }

        [HttpPost, ActionName("CargarDocumentosExternos")]
        public ActionResult CargarDocumentosExternosPost()
        {

            Program bus = new Program();
            if (ModelState.IsValid)
            {

                var archivos = SubeArchivos();
                if (archivos.Count > 0)
                {
                    try
                    {
                        bus.ProcesarArchivoExterno(archivos);
                        ModelState.AddModelError("", "Comando realizado con éxito");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("",String.Format("Error: {0}",ex.Message));
                    }
                
                }
                else {
                    ModelState.AddModelError("", "No Se Encontraron Archivos XML");

                }
            }
            ViewBag.Controller = "RecepcionDocumentos";
            ViewBag.Action = "CargarDocumentosExternos";
            ViewBag.ActionES = "Carga Documentos Externos";
            ViewBag.NameHere = "cfdi";
            return View();
        }

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

                            var path = Path.Combine(Server.MapPath("~/Archivos/RecibosExternos/"), fileName);
                            file.SaveAs(path);
                            paths.Add(path);
                        }
                        catch (Exception ex)
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