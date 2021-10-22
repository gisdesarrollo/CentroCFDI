using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using APBox.Context;
using API.Operaciones.ComplementosPagos;
using System;
using APBox.Control;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones;
using System.Web;
using Aplicacion.LogicaPrincipal.GeneracionComplementosPagos;
using API.Models.Operaciones;
using Aplicacion.LogicaPrincipal.ComplementosPagos;
using API.Enums;
using Utilerias.LogicaPrincipal;
using System.IO;
using Aplicacion.LogicaPrincipal.CargasMasivas.CSV;
using System.Collections.Generic;
using API.Models.ComplementosPagos;
using System.Linq;
using CFDI.API.Enums.CFDI33;

namespace APBox.Controllers.ComplementosPago
{
    [SessionExpire]
    public class ComplementosPagosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly PagosManager _pagosManager = new PagosManager();
        private readonly LogicaFacadeFacturas _logicaFacadeFacturas = new LogicaFacadeFacturas();
        private readonly CargarComplementosPagos _cargarComplementosPagos = new CargarComplementosPagos();
        private readonly AcondicionarComplementosPagos _acondicionarComplementosPagos = new AcondicionarComplementosPagos();

        #endregion

        // GET: Facturas
        public ActionResult Index()
        {
            var complementosPagosModel = new ComplementosPagosModel
            {
                Mes = (Meses)(DateTime.Now.Month),
                Anio = DateTime.Now.Year
            };

            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var fechaFinal = DateTime.Now;

            complementosPagosModel.ComplementosPago = _logicaFacadeFacturas.Filtrar(fechaInicial, fechaFinal, ObtenerSucursal());
            return View(complementosPagosModel);
        }

        [HttpPost]
        public ActionResult Index(ComplementosPagosModel complementosPagosModel, string actionName)
        {
            if (actionName == "Filtrar")
            {
                var dia = DateTime.DaysInMonth(complementosPagosModel.Anio, (int)complementosPagosModel.Mes);

                var fechaInicial = new DateTime(complementosPagosModel.Anio, (int)complementosPagosModel.Mes, 1, 0, 0, 0);
                var fechaFinal = new DateTime(complementosPagosModel.Anio, (int)complementosPagosModel.Mes, dia, 23, 59, 59);

                complementosPagosModel.ComplementosPago = _logicaFacadeFacturas.Filtrar(fechaInicial, fechaFinal, ObtenerSucursal());
            }
            else if (actionName == "Timbrar")
            {
                var errores = new List<String>();
                foreach (var complementoPago in complementosPagosModel.ComplementosPago.Where(cp => cp.Seleccionado))
                {
                    try
                    {
                        _pagosManager.GenerarComplementoPago(complementoPago.SucursalId, complementoPago.Id, "");
                    }
                    catch (Exception ex)
                    {
                        var complementoPagoInterno = _db.ComplementosPago.Find(complementoPago.Id);
                        errores.Add(String.Format("Error de generación del complemento del receptor {0} con total de montos {1:c}: {2}", complementoPagoInterno.Receptor.RazonSocial, complementoPagoInterno.Pagos.Sum(p => p.Monto), ex.Message));
                    }
                }

                if (errores.Count == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in errores)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            return View(complementosPagosModel);
        }

        // GET: ComplementosPago/Create
        public ActionResult Create()
        {
            PopulaClientes();
            PopulaBancos(ObtenerSucursal());
            PopulaCfdiRelacionado();

            var complementoPago = new ComplementoPago
            {
                Generado = false,
                Status = Status.Activo,
                FechaDocumento = DateTime.Now,
                Mes = (Meses)Enum.ToObject(typeof(Meses), DateTime.Now.Month),
                SucursalId = ObtenerSucursal(),
                Version = "1.0",
                Pago = new Pago
                {
                    FechaPago = DateTime.Now,
                    FormaPago = c_FormaPago.TransferenciaElectronicaDeFondos,
                    Moneda = c_Moneda.MXN,
                    TipoCambio = 1,
                    Monto = 0.0,
                }
            };

            return View(complementoPago);
        }

        // POST: ComplementosPago/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComplementoPago complementoPago)
        {
            ModelState.Remove("Pago.FechaPago");
            ModelState.Remove("Pago.FormaPago");
            ModelState.Remove("Pago.Moneda");
            ModelState.Remove("Pago.TipoCambio");
            ModelState.Remove("Pago.Monto");
            ModelState.Remove("Pago.ArchivoFisico");
            ModelState.Remove("Pago.ComplementoPagoId");
            ModelState.Remove("Pago.SucursalId");
            ModelState.Remove("CfdiRelacionado.FacturaEmitidaId");

            PopulaClientes(complementoPago.ReceptorId);
            PopulaBancos(ObtenerSucursal());
            PopulaCfdiRelacionado(complementoPago.CfdiRelacionadoId);

            if (ModelState.IsValid)
            {
                _acondicionarComplementosPagos.CargaInicial(ref complementoPago);

                try
                {
                    var pagos = complementoPago.Pagos;
                    pagos.ForEach(p => p.ComplementoPago = null);

                    complementoPago.Pagos = null;
                    _db.ComplementosPago.Add(complementoPago);
                    _db.SaveChanges();

                    foreach (var pago in pagos)
                    {
                        pago.ComplementoPago = null;
                        pago.ComplementoPagoId = complementoPago.Id;
                        _db.Pagos.Add(pago);
                    }
                    _db.SaveChanges();

                    return RedirectToAction("DocumentosRelacionados", new { @id = complementoPago.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(complementoPago);
        }

        // GET: ComplementosPago/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplementoPago complementoPago = _db.ComplementosPago.Find(id);

            if (complementoPago == null)
            {
                return HttpNotFound();
            }

            complementoPago.Pago = new Pago
            {
                FechaPago = DateTime.Now,
                FormaPago = c_FormaPago.TransferenciaElectronicaDeFondos,
                Moneda = c_Moneda.MXN,
                TipoCambio = 1,
                Monto = 0.0,
            };

            PopulaClientes(complementoPago.ReceptorId);
            PopulaBancos(ObtenerSucursal());
            PopulaCfdiRelacionado(complementoPago.CfdiRelacionadoId);

            return View(complementoPago);
        }

        // POST: ComplementosPago/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComplementoPago complementoPago)
        {
            ModelState.Remove("Pago.FechaPago");
            ModelState.Remove("Pago.FormaPago");
            ModelState.Remove("Pago.Moneda");
            ModelState.Remove("Pago.TipoCambio");
            ModelState.Remove("Pago.Monto");
            ModelState.Remove("Pago.ArchivoFisico");
            ModelState.Remove("Pago.ComplementoPagoId");
            ModelState.Remove("Pago.SucursalId");

            PopulaClientes(complementoPago.ReceptorId);
            PopulaBancos(ObtenerSucursal());
            PopulaCfdiRelacionado(complementoPago.CfdiRelacionadoId);

            if (ModelState.IsValid)
            {
                _acondicionarComplementosPagos.Pagos(complementoPago);

                complementoPago.Pagos = null;
                _db.Entry(complementoPago).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(complementoPago);
        }

        // GET: ComplementosPago/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplementoPago complementoPago = _db.ComplementosPago.Find(id);
            if (complementoPago == null)
            {
                return HttpNotFound();
            }
            PopulaClientes(complementoPago.ReceptorId);
            PopulaBancos(ObtenerSucursal());
            PopulaCfdiRelacionado(complementoPago.CfdiRelacionadoId);
            PopulaCfdiRelacionado(complementoPago.CfdiRelacionadoId);

            return View(complementoPago);
        }

        // POST: ComplementosPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComplementoPago complementoPago = _db.ComplementosPago.Find(id);
            _db.ComplementosPago.Remove(complementoPago);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Cargas Masivas

        public ActionResult Exportar()
        {
            var pathCompleto = _cargarComplementosPagos.Exportar();
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

        public ActionResult Cargar()
        {
            var cargasComplementosModel = new CargasComplementosModel
            {
                GrupoId = ObtenerGrupo(),
                Previsualizacion = true,
                Mes = (Meses)Enum.ToObject(typeof(Meses), DateTime.Now.Month),
                Detalles = new List<ComplementoPago>()
            };

            return View(cargasComplementosModel);
        }

        [HttpPost]
        public ActionResult Cargar(CargasComplementosModel cargasComplementosModel)
        {
            if (ModelState.IsValid)
            {
                string archivo;
                try
                {
                    archivo = SubeArchivo(0);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                    return View(cargasComplementosModel);
                }

                try
                {
                    cargasComplementosModel.Detalles = _cargarComplementosPagos.Importar(archivo, ObtenerSucursal(), cargasComplementosModel.Mes, cargasComplementosModel.Previsualizacion);
                    if (cargasComplementosModel.Previsualizacion)
                    {
                        ModelState.AddModelError("", "Comando realizado con éxito");
                    }
                    else
                    {
                        return RedirectToAction("Index");
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

            }
            return View(cargasComplementosModel);
        }

        #endregion

        #region Operaciones

        public ActionResult DocumentosRelacionados(int id)
        {
            var complementoPago = _db.ComplementosPago.Find(id);

            PopulaFacturas(complementoPago.ReceptorId);
            PopulaPagos(id);

            complementoPago.Pago = new Pago
            {
                DocumentoRelacionado = new DocumentoRelacionado
                {
                    Moneda = c_Moneda.MXN,
                    TipoCambio = 1,
                    NumeroParcialidad = 1
                },
                SucursalId = ObtenerSucursal()
            };

            return View(complementoPago);
        }

        [HttpPost]
        public ActionResult DocumentosRelacionados(ComplementoPago complementoPago)
        {
            PopulaFacturas(complementoPago.ReceptorId);
            PopulaPagos(complementoPago.Id);

            ModelState.Remove("Receptor.Nombre");
            ModelState.Remove("Receptor.Pais");
            ModelState.Remove("Pago.DocumentoRelacionado.IdDocumento");

            complementoPago.Pago = null;

            if (ModelState.IsValid)
            {
                _acondicionarComplementosPagos.DocumentosRelacionados(complementoPago);

                if(complementoPago.DocumentosRelacionados != null)
                {
                    foreach (var pago in complementoPago.DocumentosRelacionados)
                    {
                        complementoPago.DocumentosRelacionados.ForEach(dr => dr.FacturaEmitida = null);
                        complementoPago.DocumentosRelacionados.ForEach(dr => dr.Pago = null);
                        complementoPago.DocumentosRelacionados.ForEach(dr => _db.DocumentosRelacionados.Add(dr));
                    }
                }

                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(complementoPago);
        }

        public ActionResult Generar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplementoPago complementoPago = _db.ComplementosPago.Find(id);
            if (complementoPago == null)
            {
                return HttpNotFound();
            }
            PopulaClientes(complementoPago.ReceptorId);
            return View(complementoPago);
        }

        [HttpPost]
        public ActionResult Generar(ComplementoPago complementoPago)
        {
            PopulaClientes(complementoPago.ReceptorId);

            if (ModelState.IsValid)
            {
                try
                {
                    var sucursalId = ObtenerSucursal();

                    //Actualizacion Receptor
                    var complementoPagoDb = _db.ComplementosPago.Find(complementoPago.Id);
                    complementoPagoDb.ReceptorId = complementoPago.ReceptorId;
                    _db.Entry(complementoPagoDb).State = EntityState.Modified;
                    _db.SaveChanges();

                    _pagosManager.GenerarComplementoPago(sucursalId, complementoPago.Id, "");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(complementoPago);
        }

        public ActionResult Cancelar(int id)
        {
            try
            {
                _pagosManager.Cancelar(id);
            }
            catch (Exception ex)
            { 
                throw new Exception(ex.Message);
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult Descargar(int id)
        {
            var pathCompleto = _pagosManager.GenerarZipComplementoPago(id);
            byte[] archivoFisico = System.IO.File.ReadAllBytes(pathCompleto);
            string contentType = MimeMapping.GetMimeMapping(pathCompleto);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(pathCompleto),
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(archivoFisico, contentType);
        }

        public ActionResult DescargarAcuse(int id)
        {
            var pathCompleto = _pagosManager.DescargarAcuse(id);
            string contentType = MimeMapping.GetMimeMapping(Path.GetFileName(pathCompleto));
            return File(pathCompleto, contentType);
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

        #region Popula Forma

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private void PopulaBancos(int sucursalId, int? bancoReceptorId = null, int? bancoEmisorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), true);

            ViewBag.BancoOrdenanteId = popularDropDowns.PopulaBancosClientes(0, bancoReceptorId);
            ViewBag.BancoBeneficiarioId = popularDropDowns.PopulaBancosSucursales(sucursalId, bancoEmisorId);
        }

        private void PopulaClientes(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.ReceptorId = popularDropDowns.PopulaClientes(receptorId);
        }

        private void PopulaFacturas(int clienteId, int? facturaId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.FacturaEmitidaId = popularDropDowns.PopulaFacturasEmitidas(true, clienteId, facturaId);
        }

        private void PopulaPagos(int? complementoPagoId = null, int? pagoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.PagoId = popularDropDowns.PopulaPagos(complementoPagoId, pagoId);
        }

        private void PopulaCfdiRelacionado(int? cfdiRelacionadoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.CfdiRelacionadoId = popularDropDowns.PopulaFacturasEmitidas(false, 0, cfdiRelacionadoId);
        }

        #endregion

        #region Operaciones Archivos

        private String SubeArchivo(int indice)
        {
            if (Request.Files.Count > 0)
            {
                var archivo = Request.Files[indice];
                if (archivo.ContentLength > 0)
                {
                    var operacionesStreams = new OperacionesStreams();
                    var nombreArchivo = Path.GetFileName(archivo.FileName);

                    var pathDestino = Path.Combine(Server.MapPath("~/Archivos/CargasMasivas/"), archivo.FileName);
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

        #endregion
    }
}
