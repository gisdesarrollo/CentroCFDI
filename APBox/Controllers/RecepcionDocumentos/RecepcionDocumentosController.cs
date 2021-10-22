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
            return View();
        }

        [HttpPost, ActionName("CargarDocumentosExternos")]
        public ActionResult CargarDocumentosExternosPost()
        {

            Program bus = new Program();
            if (ModelState.IsValid)
            {

                var archivos = SubeArchivos();
                if (archivos.Count > 0) {
                    bus.ProcesarArchivoExterno();
                
                ModelState.AddModelError("", "Comando realizado con éxito");
                }
            }

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