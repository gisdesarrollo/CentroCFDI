using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using API.Catalogos;
using APBox.Context;
using System;
using APBox.Control;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using System.IO;
using System.Web;
using Utilerias.LogicaPrincipal;
using CFDI.API.Enums.CFDI33;
using System.Text;

namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    //[Authorize(Roles = "SUCURSALES")]
    public class SucursalesController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();
        private readonly AcondicionarSucursales _acondicionarSucursales = new AcondicionarSucursales();

        // GET: Sucursales
        public ActionResult Index()
        {
            var grupoId = ObtenerGrupo();
            var sucursales = _db.Sucursales.Where(s => s.GrupoId == grupoId).ToList();
            return View(sucursales);
        }

        // GET: Sucursales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sucursal sucursal = _db.Sucursales.Find(id);
            if (sucursal == null)
            {
                return HttpNotFound();
            }

            PopulaForma();
            return View(sucursal);
        }

        // GET: Sucursales/Create
        public ActionResult Create()
        {
            var sucursal = new Sucursal
            {
                Pais = c_Pais.MEX,
                Status = API.Enums.Status.Activo,
                GrupoId = ObtenerGrupo(),

                EncabezadoCorreo = "Complemento de Pago",
                CuerpoCorreo = "A continuación le adjuntamos su complemento de pago, gracias por su preferencia"
            };

            PopulaForma();
            return View(sucursal);
        }

        // POST: Sucursales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sucursal sucursal)
        {
            PopulaForma();

            ModelState.Remove("BancoId");
            ModelState.Remove("Banco.Nombre");
            ModelState.Remove("Banco.NumeroCuenta");

            if (ModelState.IsValid)
            {
                try
                {
                    _acondicionarSucursales.Validaciones(sucursal);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(sucursal);
                }

                try
                {
                    SubirArchivos(ref sucursal);
                }
                catch (Exception)
                {
                }

                _acondicionarSucursales.CargaInicial(ref sucursal);
                _db.Sucursales.Add(sucursal);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sucursal);
        }

        // GET: Sucursales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sucursal sucursal = _db.Sucursales.Find(id);
            if (sucursal == null)
            {
                return HttpNotFound();
            }
            PopulaForma();
            return View(sucursal);
        }

        // POST: Sucursales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sucursal sucursal)
        {
            PopulaForma();

            ModelState.Remove("BancoId");
            ModelState.Remove("Banco.Nombre");
            ModelState.Remove("Banco.NumeroCuenta");
            if (ModelState.IsValid)
            {
                _acondicionarSucursales.Bancos(sucursal);

                try
                {
                    _acondicionarSucursales.Validaciones(sucursal);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(sucursal);
                }

                try
                {
                    SubirArchivos(ref sucursal);
                }
                catch (Exception)
                {
                }

                if(sucursal.Bancos != null)
                {
                    sucursal.Bancos.Clear();
                    sucursal.Bancos = null;
                }

                _db.Entry(sucursal).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sucursal);
        }

        // GET: Sucursales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sucursal sucursal = _db.Sucursales.Find(id);
            if (sucursal == null)
            {
                return HttpNotFound();
            }
            PopulaForma();
            return View(sucursal);
        }

        // POST: Sucursales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sucursal sucursal = _db.Sucursales.Find(id);
            _db.Sucursales.Remove(sucursal);
            _db.SaveChanges();
            return RedirectToAction("Index");
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

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private void PopulaForma()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), true);

            ViewBag.BancoId = popularDropDowns.PopulaBancos(null);
        }

        #endregion

        #region Operaciones Archivos

        private void SubirArchivos(ref Sucursal sucursal)
        {
            var utf8 = new UTF8Encoding();
            if (Request.Files.Count > 1)
            {
                for (int i = 0; i <= 2; i++)
                {
                    var archivo = Request.Files[i];

                    var pathDestino = Server.MapPath("~/Archivos/ArchivosSucursales");

                    var carpeta = string.Empty;
                    switch (Path.GetExtension(archivo.FileName.ToLower()))
                    {
                        case ".key":
                        case ".cer":
                            carpeta = "Certificados";
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            carpeta = "Logos";
                            break;
                        default:
                            continue;
                    }

                    pathDestino = Path.Combine(pathDestino, carpeta, archivo.FileName);
                    var fileStream = archivo.InputStream;
                    var mStreamer = new MemoryStream();
                    mStreamer.SetLength(fileStream.Length);
                    fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
                    mStreamer.Seek(0, SeekOrigin.Begin);
                    _operacionesStreams.StreamArchivo(mStreamer, pathDestino);
                    
                    switch (Path.GetExtension(archivo.FileName.ToLower()))
                    {
                        case ".key":
                            sucursal.NombreArchivoKey = Path.GetFileName(archivo.FileName);

                            byte[] Key = System.IO.File.ReadAllBytes(pathDestino);
                            sucursal.Key = Key;/*_operacionesStreams.ArchivoStream(pathDestino).ToArray();*/
                            break;
                        case ".cer":
                            sucursal.NombreArchivoCer = Path.GetFileName(archivo.FileName);
                            byte[] Cer = System.IO.File.ReadAllBytes(pathDestino);
                            sucursal.Cer = Cer;/*_operacionesStreams.ArchivoStream(pathDestino).ToArray();*/
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            sucursal.NombreLogo = Path.GetFileName(archivo.FileName);
                            sucursal.Logo = _operacionesStreams.ArchivoStream(pathDestino).ToArray();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public ActionResult ObtenerArchivos(int sucursalId, string archivo)
        {
            String mimeType = string.Empty;
            var sucursal = _db.Sucursales.Find(sucursalId);
            switch (archivo)
            {
                case "key":
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", sucursal.NombreArchivoKey));
                    mimeType = MimeMapping.GetMimeMapping(sucursal.NombreArchivoKey);
                    return File(sucursal.Key, mimeType);
                case "cer":
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", sucursal.NombreArchivoCer));
                    mimeType = MimeMapping.GetMimeMapping(sucursal.NombreArchivoCer);
                    return File(sucursal.Cer, mimeType);
                case "logo":
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", sucursal.NombreLogo));
                    mimeType = MimeMapping.GetMimeMapping(sucursal.NombreLogo);
                    return File(sucursal.Logo, mimeType);
                default:
                    return File(new byte[0], "");
            }

        }

        #endregion

    }
}
