using APBox.Context;
using API.Enums;
using API.Models.Operaciones;
using API.Operaciones.Facturacion;
using Aplicacion.LogicaPrincipal.Validacion;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilerias.LogicaPrincipal;

namespace APBox.Controllers.Operaciones
{
    [APBox.Control.SessionExpire]
    public class ValidacionesRapidasController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();

        private readonly GuardaFacturas _guarda = new GuardaFacturas();
        private readonly DecodificaFacturas _decodificaFacturas = new DecodificaFacturas();
        private readonly ValidacionesFacturas _validacionesFacturas = new ValidacionesFacturas();

        #endregion

        public ActionResult Validar()
        {
            var sucursalId = ObtenerSucursal();

            var facturasModel = new FacturasModel
            {
                FacturasRecibidas = _db.FacturasRecibidas.Where(fp => fp.ReceptorId == sucursalId).OrderBy(fp => fp.Emisor.RazonSocial).ThenByDescending(vf => vf.Fecha).ToList()
            };

            return View(facturasModel);
        }

        [HttpPost]
        public ActionResult Validar(FacturasModel facturasModel)
        {
            ModelState.Remove("Archivos");

            var sucursalId = ObtenerSucursal();
            var sucursal = _db.Sucursales.Find(sucursalId);
            facturasModel = new FacturasModel
            {
                FacturasRecibidas = _db.FacturasRecibidas.Where(fp => fp.ReceptorId == sucursalId).OrderBy(fp => fp.Emisor.RazonSocial).ThenByDescending(vf => vf.Fecha).ToList()
            };

            if (ModelState.IsValid)
            {
                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("", "Sin archivos para procesar");
                    return View(facturasModel);
                }

                try
                {
                    var facturaRecibida = new FacturaRecibida();

                    PopularArchivos(ref facturaRecibida);
                    var pathFactura = Path.Combine(Server.MapPath("~/Archivos/Validaciones/"), facturaRecibida.NombreArchivoXml);
                    _operacionesStreams.ByteArrayArchivo(facturaRecibida.ArchivoFisicoXml, pathFactura);

                    _decodificaFacturas.DecodificarFactura(ref facturaRecibida, pathFactura);

                    var rfcReceptor = facturaRecibida.Receptor.Rfc;
                    facturaRecibida.Receptor = _db.Sucursales.FirstOrDefault(s => s.Rfc == rfcReceptor);

                    //Validaciones
                    _validacionesFacturas.Negocios(facturaRecibida, ObtenerSucursal());
                    facturaRecibida.Validacion = _validacionesFacturas.Sat(pathFactura, facturaRecibida.Receptor.Rfc);

                    facturaRecibida.UsuarioId = ObtenerUsuario();
                    //Guardar factura
                    try
                    {
                        _guarda.GuardarFacturaRecibida(facturaRecibida, ObtenerSucursal(), TiposGastos.Personal);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", String.Format("No se pudo guardar la factura {0}", ex.Message));
                        return View(facturasModel);
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(facturasModel);
                }

                return RedirectToAction("Validar");
            }

            return View(facturasModel);
        }

        public ActionResult Reporte(int id)
        {
            var facturaRecibida = _db.FacturasRecibidas.Find(id);
            return View(facturaRecibida);
        }

        #region PopulaForma

        private int ObtenerUsuario()
        {
            return Convert.ToInt32(Session["UsuarioId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion

        #region Archivos

        private void PopularArchivos(ref FacturaRecibida facturaRecibida)
        {
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i].ContentLength > 0)
                    {
                        var operacionesStreams = new OperacionesStreams();
                        var nombreArchivo = Path.GetFileName(Request.Files[i].FileName);

                        var pathDestino = Path.Combine(Server.MapPath("~/Archivos/Validaciones/"), Request.Files[i].FileName);
                        Stream fileStream = Request.Files[i].InputStream;
                        var mStreamer = new MemoryStream();
                        mStreamer.SetLength(fileStream.Length);
                        fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
                        mStreamer.Seek(0, SeekOrigin.Begin);
                        var arreglo = operacionesStreams.StreamByteArray(mStreamer);
                        operacionesStreams.StreamArchivo(mStreamer, pathDestino);

                        switch (Path.GetExtension(Request.Files[i].FileName).ToLower())
                        {
                            case ".xml":
                                facturaRecibida.NombreArchivoXml = Request.Files[i].FileName;
                                facturaRecibida.ArchivoFisicoXml = arreglo;
                                break;
                            default:
                                throw new Exception("Tipo de archivo Inválido");
                        }

                        mStreamer.Close();
                    }
                }
            }
        }

        public ActionResult ObtenerArchivos(int id, bool xml)
        {
            var entidad = _db.FacturasRecibidas.Find(id);
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", entidad.NombreArchivoXml));
            string mimeType = MimeMapping.GetMimeMapping(entidad.NombreArchivoXml);
            return File(entidad.ArchivoFisicoXml, mimeType);
        }

        #endregion
    }
}