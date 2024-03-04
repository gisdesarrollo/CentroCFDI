using APBox.Context;
using APBox.Control;
using API.Enums;
using API.Models.Cargas;
using API.Models.Facturas;
using API.Operaciones.Facturacion;
using Aplicacion.LogicaPrincipal.Descargas;
using Aplicacion.LogicaPrincipal.Facturas;
using Aplicacion.LogicaPrincipal.GeneracionComplementosPagos;
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
    public class OperacionesCfdisRecibidosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesCfdisRecibidos _operacionesCfdisRecibidos = new OperacionesCfdisRecibidos();
        private readonly PagosManager _pagosManager = new PagosManager();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();

        private readonly GuardaFacturas _guardaFacturas = new GuardaFacturas();
        private readonly DecodificaFacturas _decodifica = new DecodificaFacturas();
        private readonly ValidacionesFacturas _validacionesFacturas = new ValidacionesFacturas();
        private readonly DescargasManager _descargasManager = new DescargasManager();
       
        #endregion

        #region Vistas

        public ActionResult VerTodos()
        {
            var sucursalId = ObtenerSucursal();

            var facturasRecibidasModel = new FacturasRecibidasModel
            {
                FacturasRecibidas = new System.Collections.Generic.List<FacturaRecibida>(),
                FechaInicial = DateTime.Now.AddDays(-6), // SE RESTA 6 DIAS PARA MOSTRAR EL RANGO DE FACTURAS GENERADAS EN UN SEMANA
                FechaFinal = DateTime.Now,
                SucursalId = ObtenerSucursal(),
            };
            /*var sucursalId = ObtenerSucursal();
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFinal = DateTime.Now.Date;
            */
            // _operacionesCfdisRecibidos = new OperacionesCfdisRecibidos(sucursalId, fechaInicial, fechaFinal);
            _operacionesCfdisRecibidos.ObtenerFacturasRecibidas(ref facturasRecibidasModel);

            ViewBag.Controller = "OperacionesCfdisRecibidos";
            ViewBag.Action = "VerTodos";
            ViewBag.ActionES = "Ver Todos";
            ViewBag.NameHere = "cfdi";

            return View(facturasRecibidasModel);
        }

        [HttpPost]
        public ActionResult VerTodos(FacturasRecibidasModel facturaRecibidaModel)
        {
            //var sucursalId = ObtenerSucursal();
            //var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //var fechaFinal = DateTime.Now.Date;

            //_operacionesCfdisRecibidos = new OperacionesCfdisRecibidos(sucursalId, fechaInicial, fechaFinal);
            if (ModelState.IsValid)
            {
                _operacionesCfdisRecibidos.ObtenerFacturasRecibidas(ref facturaRecibidaModel);
            }
            ViewBag.Controller = "OperacionesCfdisRecibidos";
            ViewBag.Action = "VerTodos";
            ViewBag.ActionES = "Ver Todos";
            ViewBag.NameHere = "cfdi";

            return View(facturaRecibidaModel);
        }

        public ActionResult DescargaPDF(int facturaRecibidaId)
        {
            //objetos version 4.0
            ComprobanteCFDI oComprobante = new ComprobanteCFDI();
            //objetos version 3.3
            ComprobanteCFDI33 oComprobante33 = new ComprobanteCFDI33();
            string tipoDocumento = null;
            byte[] archivoFisico = new byte[255];
            var facturaEmitida = _db.FacturasRecibidas.Find(facturaRecibidaId);
            
                
                    //busca version del CFDI del archivo
                    string CadenaXML = System.Text.Encoding.UTF8.GetString(facturaEmitida.ArchivoFisicoXml);
                    string versionCfdi = _decodifica.LeerValorXML(CadenaXML, "Version", "Comprobante");
                    if (versionCfdi == "3.3")
                    {
                        oComprobante33 = _decodifica.DeserealizarXML33(facturaEmitida.ArchivoFisicoXml);
                        tipoDocumento = _decodifica.TipoDocumentoCfdi33(facturaEmitida.ArchivoFisicoXml);
                        archivoFisico = _descargasManager.GeneraPDF33(oComprobante33, tipoDocumento, facturaRecibidaId, false,true);
                    }
                    else
                    {
                        oComprobante = _decodifica.DeserealizarXML40(facturaEmitida.ArchivoFisicoXml);
                        tipoDocumento = _decodifica.TipoDocumentoCfdi40(facturaEmitida.ArchivoFisicoXml);
                        archivoFisico = _descargasManager.GeneraPDF40(oComprobante, tipoDocumento, facturaRecibidaId, false,true);
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

        public ActionResult DescargaXML(int facturaRecibidaId)
        {
            //get xml
            var facturaEmtida = _db.FacturasRecibidas.Find(facturaRecibidaId);
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

        public ActionResult ReporteCfdiRecibidos()
        {
            var sucursalId = ObtenerSucursal();

            var facturasRecibidasModel = new FacturasRecibidasModel
            {
                FacturasRecibidas = new System.Collections.Generic.List<FacturaRecibida>(),
                FechaInicial = DateTime.Now.AddDays(-6), // SE RESTA 6 DIAS PARA MOSTRAR EL RANGO DE FACTURAS GENERADAS EN UN SEMANA
                FechaFinal = DateTime.Now,
                SucursalId = ObtenerSucursal(),
            };
            _operacionesCfdisRecibidos.ObtenerFacturasRecibidas(ref facturasRecibidasModel);

            ViewBag.Controller = "OperacionesCfdisRecibidos";
            ViewBag.Action = "ReporteCfdiRecibidos";
            ViewBag.ActionES = "Reporte Cfdi Recibidos";
            ViewBag.NameHere = "reportes";
            return View(facturasRecibidasModel);
        }

        [HttpPost]
        public ActionResult ReporteCfdiRecibidos(FacturasRecibidasModel facturaRecibidaModel)
        {
            if (ModelState.IsValid)
            {
                _operacionesCfdisRecibidos.ObtenerFacturasRecibidas(ref facturaRecibidaModel);
            }
            ViewBag.Controller = "OperacionesCfdisRecibidos";
            ViewBag.Action = "ReporteCfdiRecibidos";
            ViewBag.ActionES = "Reporte Cfdi Recibidos";
            ViewBag.NameHere = "reportes";
            return View(facturaRecibidaModel);
        }
        public ActionResult ReporteIndividual(int facturaRecibidaId)
        {
            var facturaRecibida = _db.FacturasRecibidas.Find(facturaRecibidaId);
            ViewBag.Controller = "OperacionesCfdisRecibidos";
            ViewBag.Action = "ReporteIndividual";
            ViewBag.ActionES = "Reporte Individual";
            ViewBag.NameHere = "cfdi";
            return View(facturaRecibida);
        }

        /*public ActionResult VerGastosPersonal()
        {
            var sucursalId = ObtenerSucursal();
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFinal = DateTime.Now.Date;

            _operacionesCfdisRecibidos = new OperacionesCfdisRecibidos(sucursalId, fechaInicial, fechaFinal);
            var cfdis = _operacionesCfdisRecibidos.ObtenerFacturasRecibidas(TiposGastos.Personal);

            return View(cfdis);
        }*/

        /*public ActionResult VerGastosProveedores()
        {
            var sucursalId = ObtenerSucursal();
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFinal = DateTime.Now.Date;

            _operacionesCfdisRecibidos = new OperacionesCfdisRecibidos(sucursalId, fechaInicial, fechaFinal);
            var cfdis = _operacionesCfdisRecibidos.ObtenerFacturasRecibidas(TiposGastos.Proveedores);

            return View(cfdis);
        }*/

        #endregion

        #region Validaciones

        public ActionResult CargaGeneral()
        {
            PopulaForma();
            return View();
        }

        [HttpPost]
        public ActionResult CargaGeneral(CargasCfdisModel cargasCfdisModel)
        {
            PopulaForma(cargasCfdisModel.DepartamentoId, cargasCfdisModel.CentroCostoId);
            ModelState.Remove("Archivos");

            var sucursalId = ObtenerSucursal();
            var sucursal = _db.Sucursales.Find(sucursalId);

            if (ModelState.IsValid)
            {
                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("", "Sin archivos para procesar");
                    return View();
                }

                try
                {
                    var facturaRecibida = new FacturaRecibida();

                    PopularArchivos(ref facturaRecibida);
                    var pathFactura = Path.Combine(Server.MapPath("~/Archivos/Validaciones/"), facturaRecibida.NombreArchivoXml);
                    _operacionesStreams.ByteArrayArchivo(facturaRecibida.ArchivoFisicoXml, pathFactura);

                    //_decodificaFacturas.DecodificarFactura(ref facturaRecibida, pathFactura);

                    var rfcReceptor = facturaRecibida.Receptor.Rfc;
                    facturaRecibida.Receptor = _db.Sucursales.FirstOrDefault(s => s.Rfc == rfcReceptor);
                    facturaRecibida.DepartamentoId = cargasCfdisModel.DepartamentoId;
                    facturaRecibida.CentroCostoId = cargasCfdisModel.CentroCostoId;
                    facturaRecibida.UsuarioId = ObtenerUsuario();

                    //Validaciones
                    _validacionesFacturas.Negocios(facturaRecibida, ObtenerSucursal());
                    facturaRecibida.Validacion = _validacionesFacturas.Sat(pathFactura, facturaRecibida.Receptor.Rfc);

                    //Guardar factura
                    try
                    {
                        _guardaFacturas.GuardarFacturaRecibida(facturaRecibida, ObtenerSucursal(), TiposGastos.Personal);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", String.Format("No se pudo guardar la factura {0}", ex.Message));
                        return View();
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }

            }

            return View();
        }

        public ActionResult CargaGastosPersonal()
        {
            PopulaForma();
            return View();
        }

        [HttpPost]
        public ActionResult CargaGastosPersonal(CargasCfdisModel cargasCfdisModel)
        {
            PopulaForma(cargasCfdisModel.DepartamentoId, cargasCfdisModel.CentroCostoId);
            ModelState.Remove("Archivos");

            var sucursalId = ObtenerSucursal();
            var sucursal = _db.Sucursales.Find(sucursalId);

            if (ModelState.IsValid)
            {
                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("", "Sin archivos para procesar");
                    return View();
                }

                try
                {
                    var facturaRecibida = new FacturaRecibida();

                    PopularArchivos(ref facturaRecibida);
                    var pathFactura = Path.Combine(Server.MapPath("~/Archivos/Validaciones/"), facturaRecibida.NombreArchivoXml);
                    _operacionesStreams.ByteArrayArchivo(facturaRecibida.ArchivoFisicoXml, pathFactura);

                   // _decodificaFacturas.DecodificarFactura(ref facturaRecibida, pathFactura);

                    var rfcReceptor = facturaRecibida.Receptor.Rfc;
                    facturaRecibida.Receptor = _db.Sucursales.FirstOrDefault(s => s.Rfc == rfcReceptor);
                    facturaRecibida.DepartamentoId = cargasCfdisModel.DepartamentoId;
                    facturaRecibida.CentroCostoId = cargasCfdisModel.CentroCostoId;
                    facturaRecibida.UsuarioId = ObtenerUsuario();

                    //Validaciones
                    _validacionesFacturas.Negocios(facturaRecibida, ObtenerSucursal());
                    facturaRecibida.Validacion = _validacionesFacturas.Sat(pathFactura, facturaRecibida.Receptor.Rfc);

                    //Guardar factura
                    try
                    {
                        _guardaFacturas.GuardarFacturaRecibida(facturaRecibida, ObtenerSucursal(), TiposGastos.Personal);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", String.Format("No se pudo guardar la factura {0}", ex.Message));
                        return View();
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }

            }

            return View();
        }

        public ActionResult CargaGastosProveedores()
        {
            PopulaForma();
            return View();
        }

        [HttpPost]
        public ActionResult CargaGastosProveedores(CargasCfdisModel cargasCfdisModel)
        {
            PopulaForma(cargasCfdisModel.DepartamentoId, cargasCfdisModel.CentroCostoId);
            ModelState.Remove("Archivos");

            var sucursalId = ObtenerSucursal();
            var sucursal = _db.Sucursales.Find(sucursalId);

            if (ModelState.IsValid)
            {
                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("", "Sin archivos para procesar");
                    return View();
                }

                try
                {
                    var facturaRecibida = new FacturaRecibida();

                    PopularArchivos(ref facturaRecibida);
                    var pathFactura = Path.Combine(Server.MapPath("~/Archivos/Validaciones/"), facturaRecibida.NombreArchivoXml);
                    _operacionesStreams.ByteArrayArchivo(facturaRecibida.ArchivoFisicoXml, pathFactura);

                    //_decodificaFacturas.DecodificarFactura(ref facturaRecibida, pathFactura);

                    var rfcReceptor = facturaRecibida.Receptor.Rfc;
                    facturaRecibida.Receptor = _db.Sucursales.FirstOrDefault(s => s.Rfc == rfcReceptor);
                    facturaRecibida.DepartamentoId = cargasCfdisModel.DepartamentoId;
                    facturaRecibida.CentroCostoId = cargasCfdisModel.CentroCostoId;

                    //Validaciones
                    _validacionesFacturas.Negocios(facturaRecibida, ObtenerSucursal());
                    facturaRecibida.Validacion = _validacionesFacturas.Sat(pathFactura, facturaRecibida.Receptor.Rfc);

                    //Guardar factura
                    try
                    {
                        _guardaFacturas.GuardarFacturaRecibida(facturaRecibida, ObtenerSucursal(), TiposGastos.Proveedores);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", String.Format("No se pudo guardar la factura {0}", ex.Message));
                        return View();
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }
            }

            return View();
        }

        #endregion

        #region Operaciones

        /*public ActionResult Aprobar(int facturaRecibidaId)
        {
            _operacionesCfdisRecibidos = new OperacionesCfdisRecibidos(ObtenerSucursal());
            _operacionesCfdisRecibidos.Autorizar(true, facturaRecibidaId, ObtenerUsuario(), null);
            return RedirectToAction("VerTodos");
        }*/

        public ActionResult Rechazar(int facturaRecibidaId)
        {
            var facturaRecibida = _db.FacturasRecibidas.Find(facturaRecibidaId);

            facturaRecibida.FechaAutorizacion = DateTime.Now;
            facturaRecibida.UsuarioAutorizacionId = ObtenerUsuario();

            return View(facturaRecibida);
        }

        [HttpPost]
        /*public ActionResult Rechazar(FacturaRecibida facturaRecibida)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(facturaRecibida).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                _operacionesCfdisRecibidos = new OperacionesCfdisRecibidos(ObtenerSucursal());
                _operacionesCfdisRecibidos.Autorizar(false, facturaRecibida.Id, ObtenerUsuario(), null);

                return RedirectToAction("VerTodos");
            }

            return View(facturaRecibida);
        }*/

        public ActionResult Descargar(int facturaRecibidaId)
        {
            String archivoPath = null;
            try
            {
                archivoPath = _pagosManager.GenerarZipFacturaRecibida(facturaRecibidaId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            byte[] byteArchivo = System.IO.File.ReadAllBytes(archivoPath);
            string contentType = MimeMapping.GetMimeMapping(archivoPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(archivoPath),
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(byteArchivo, contentType);
        }

        #endregion

        #region PopulaForma

        private int ObtenerUsuario()
        {
            return Convert.ToInt32(Session["UsuarioId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
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

        public ActionResult ObtenerArchivos(int id)
        {
            var entidad = _db.FacturasRecibidas.Find(id);
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", entidad.NombreArchivoXml));
            string mimeType = MimeMapping.GetMimeMapping(entidad.NombreArchivoXml);
            return File(entidad.ArchivoFisicoXml, mimeType);
        }

        #endregion

        #region Popula Forma

        private void PopulaForma(int? departamentoId = null, int? centroCostoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), false);

            ViewBag.DepartamentoId = popularDropDowns.PopulaDepartamentos(departamentoId);
            ViewBag.CentroCostoId = popularDropDowns.PopulaCentroCostos(centroCostoId);
        }

        #endregion

    }
}