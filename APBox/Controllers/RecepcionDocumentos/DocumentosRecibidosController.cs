using APBox.Context;
using APBox.Control;
using API.Catalogos;
using API.Enums;
using API.Models.DocumentosRecibidos;
using API.Models.Dto;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.OperacionesProveedores;
using Aplicacion.LogicaPrincipal.Correos;
using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using Aplicacion.LogicaPrincipal.Facturas;
using Aplicacion.RecepcionDocumentos;
using SW.Services.Authentication;
using SW.Services.Validate;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Web.Mvc;
using Utilerias.LogicaPrincipal;

namespace APBox.Controllers.Operaciones
{
    [SessionExpire]
    public class DocumentosRecibidosController : Controller
    {
        #region variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly ProcesaDocumentoRecibido _procesaDocumentoRecibido = new ProcesaDocumentoRecibido();
        private readonly Decodificar _decodifica = new Decodificar();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();
        private readonly OperacionesDocumentosRecibidos _operacionesDocumentosRecibidos = new OperacionesDocumentosRecibidos();

        #endregion variables

        #region Consultas

        //Metodo de Validacion E-Mail para usuarios solicitantes
        [HttpPost]
        public ActionResult ValidadorEmail(DocumentosRecibidos documentoRecibidoDr)
        {
            string email = documentoRecibidoDr.VerificarEmail;
            var grupoid = ObtenerGrupo();
            var usuarioSolicitante = _db.Usuarios.Where(c => c.Email == email && c.GrupoId == grupoid).FirstOrDefault();

            if (usuarioSolicitante == null)
            {
                return Json(new { success = false, message = "El Email no fue encontrado, favor de verificar" });
            }
            else
            {
                var usuarioSolicitanteNombre = usuarioSolicitante.NombreCompleto;
                var usuarioSolicitanteDepartamento = usuarioSolicitante.Departamento.Nombre;

                Session["AprobadorId"] = usuarioSolicitante.Id;
                Session["DepartamentoId"] = usuarioSolicitante.Departamento.Id;

                return Json(new
                {
                    success = true,
                    UsuarioSolicitanteEmail = email,
                    UsuarioSolicitanteNombre = usuarioSolicitanteNombre,
                    UsuarioSolicitanteDepartamento = usuarioSolicitanteDepartamento
                });
            }
        }

        #endregion Consultas

        #region Vistas

        // GET: DocumentosRecibidos/Index
        public ActionResult Index()
        {
            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Índice";
            ViewBag.Button = "CargaDocumentoRecibido";
            ViewBag.Title = "Documentos Recibidos";

            //get usaurio

            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            var documentosRecibidosModel = new DocumentosRecibidosModel();
            var fechaInicial = DateTime.Today.AddDays(-10);
            var fechaFinal = DateTime.Today.AddDays(1).AddTicks(-1);

            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;

            documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, sucursal.Id, usuario.Id);

            return View(documentosRecibidosModel);
        }

        // POST: DocumentosRecibidos/Index
        [HttpPost]
        public ActionResult Index(DocumentosRecibidosModel documentosRecibidosModel)
        {
            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "CargaDocumentoRecibido";
            ViewBag.Title = "Documentos Recibidos";
            //get usaurio
            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            var fechaInicial = DateTime.Today.AddDays(-10);
            var fechaFinal = DateTime.Today.AddDays(1).AddTicks(-1);

            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;

            documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.Filtrar(fechaInicial, fechaFinal, sucursal.Id, usuario.Id);

            return View(documentosRecibidosModel);
        }

        // GET: DocumentosRecibidos/CargaCfdi
        public ActionResult CargaCfdi(int? comprobacionGastoId, int? complementoPagoId)
        {
            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "CargaCfdi";
            ViewBag.NameHere = "Carga de CFDI";
            ViewBag.ActionES = "CargaCfdi";

            if (comprobacionGastoId.HasValue)
            {
                Session["ComprobacionGastosId"] = comprobacionGastoId;
            }
            if (complementoPagoId.HasValue) {
                Session["ComplementoPagoId"] = complementoPagoId;
            }

            // Obtener el usuario
            var usuario = _db.Usuarios.Find(ObtenerUsuario());

            // Crear un nuevo objeto DocumentosRecibidos
            DocumentosRecibidos documentoRecibidoDr = new DocumentosRecibidos()
            {
                Validaciones = new ValidacionesDR()
            };

            documentoRecibidoDr.Procesado = false;

            // Si hay un ComprobacionGastosId válido, establecerlo en el objeto
            if (comprobacionGastoId.HasValue)
            {
                documentoRecibidoDr.ComprobacionGastoId = comprobacionGastoId.Value;
            }
            // si hay un complementopagoId cargado , establecerlo  en el objeto
            PagosDR pago = new PagosDR();
            if (complementoPagoId.HasValue)
            {
                documentoRecibidoDr.PagosId = (int)complementoPagoId;
            }

            return View(documentoRecibidoDr);
        }

        // POST: DocumentosRecibidos/CargaCfdi
        [HttpPost]
        public ActionResult CargaCfdi(DocumentosRecibidos documentoRecibidoDr)
        {
            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "CargaCfdi";
            ViewBag.NameHere = "Documentos Recibidos";

            //get usaurio
            PathArchivosDto archivo;
            ValidateXmlResponse responseValidacion = new ValidateXmlResponse();
            ComprobanteCFDI cfdi = new ComprobanteCFDI();
            int? compPagoId = (int?)Session["ComplementoPagoId"];
            try
            {
                archivo = SubeArchivo();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                return View(documentoRecibidoDr);
            }

            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            documentoRecibidoDr.DetalleArrays = new List<String>();
            cfdi = _procesaDocumentoRecibido.DecodificaXML(archivo.PathDestinoXml);
            var timbreFiscalDigital = _decodifica.DecodificarTimbre(cfdi, null);
            var dataValidar = new ValidacionesComerciales.DataValidar();
            var dataValidarPagos = new ValidacionesPagos.DataValidar();
            if (compPagoId == null)
            {
                //se crea una instancia con los datos a validar en una petición
                dataValidar = new ValidacionesComerciales.DataValidar
                {
                    Cfdi = cfdi,
                    TimbreFiscalDigital = timbreFiscalDigital,
                    Sucursal = _db.Sucursales.Find(ObtenerSucursal()),
                    SocioComercial = cfdi.Emisor.Equals(null) ? null : _db.SociosComerciales.Where(s => s.Rfc == cfdi.Emisor.Rfc).FirstOrDefault(),
                    Usuario = usuario,
                    ConfiguracionEmpresa = ConfiguracionEmpresa(),
                    DocumentoRecibidoDr = documentoRecibidoDr,
                    Archivo = archivo
                };

                ValidacionesComerciales validacionesComerciales = new ValidacionesComerciales();

                //Se manda validar al grupo de valiadciones de stock para verificar que el documento pueda procesar las validaciones de configuraciones
                try
                {
                    validacionesComerciales.ValidacionesNegocio(dataValidar);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", String.Format("Error: {0}", ex.Message));
                    return View(documentoRecibidoDr);
                }

                try
                {
                    validacionesComerciales.ValidacionesConfiguraciones(dataValidar);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", String.Format("Error: {0}", ex.Message));
                    return View(documentoRecibidoDr);
                }
            }
            else
            {
                ValidacionesPagos validacionesPagos = new ValidacionesPagos();
                dataValidarPagos = new ValidacionesPagos.DataValidar
                {
                    Cfdi = cfdi,
                    Pago = _db.PagoDr.Find(compPagoId),
                    TimbreFiscalDigital = timbreFiscalDigital,
                    Sucursal = _db.Sucursales.Find(ObtenerSucursal()),
                    SocioComercial = cfdi.Emisor.Equals(null) ? null : _db.SociosComerciales.Where(s => s.Rfc == cfdi.Emisor.Rfc).FirstOrDefault(),
                    Usuario = usuario,
                    ConfiguracionEmpresa = ConfiguracionEmpresa(),
                    DocumentoRecibidoDr = documentoRecibidoDr,
                    Archivo = archivo
                };
                try
                {
                    validacionesPagos.ValidaComplementoPago(dataValidarPagos);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", String.Format("Error: {0}", ex.Message));
                    return View(documentoRecibidoDr);
                }
                try
                {
                    validacionesPagos.ValidacionesConfiguraciones(dataValidarPagos);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", String.Format("Error: {0}", ex.Message));
                    return View(documentoRecibidoDr);
                }

            }
            //Manda datos leidos a la pantalla para confirmación
            try
            {
                ViewBag.MetodoPago = cfdi.MetodoPago;
                ViewBag.FormaPago = cfdi.FormaPago;
                ViewBag.TipoComprobante = cfdi.TipoDeComprobante.ToString();
                ViewBag.TipoCambio = cfdi.TipoCambio;
                ViewBag.Moneda = cfdi.Moneda;
                ViewBag.UsoCFDI = cfdi.Receptor.UsoCFDI;
                ViewBag.Usuario = usuario.NombreCompleto;
                ViewBag.Emisor = cfdi.Emisor.Nombre;
                ViewBag.Receptor = cfdi.Receptor.Nombre;
                ViewBag.CompPagoId = compPagoId;
            }
            catch (Exception ex)
            {
                var errores = ex.Message.Split('|');
                foreach (var error in errores)
                {
                    ModelState.AddModelError("", error);
                }
                //delete Files
                if (archivo.PathDestinoXml != null)
                {
                    System.IO.File.Delete(archivo.PathDestinoXml);
                }
                if (archivo.PathDestinoPdf != null)
                {
                    System.IO.File.Delete(archivo.PathDestinoPdf);
                }
                documentoRecibidoDr.Procesado = false;
                documentoRecibidoDr.DetalleArrays = null;
            }
            if (compPagoId == null)
            {
                return View(dataValidar.DocumentoRecibidoDr);
            }
            else
            {
                return View(dataValidarPagos.DocumentoRecibidoDr);
            }
        }

        // GET: DocumentosRecibidos/Create
        public ActionResult Create()
        {
            ViewBag.NameHere = "Crear";
            //get usaurio
            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            return View();
        }

        // POST: DocumentosRecibidos/Create
        [HttpPost]
        public ActionResult Create(DocumentosRecibidos documentoRecibidoDr)
        {

            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            int? compGastosId = (int?)Session["ComprobacionGastosId"];
            int? compPagoId = (int?)Session["ComplementoPagoId"];
            ComprobanteCFDI cfdi = new ComprobanteCFDI();
            try
            {
                // Recuperar los parámetros de la sesión
                int? comprobacionGastosId = (int?)Session["ComprobacionGastosId"];
                int? complementoPagoId = (int?)Session["ComplementoPagoId"];
                var usuarioSolicitanteId = Session["AprobadorId"];
                var usuarioSolicitanteDepartamentoId = Session["DepartamentoId"];

                // Limpiar los parámetros de la sesión después de usarlos
                Session.Remove("ComprobacionGastosId");
                Session.Remove("AprobadorId");
                Session.Remove("DepartamentoId");
                Session.Remove("ComplementoPagoId");

                cfdi = _procesaDocumentoRecibido.DecodificaXML(documentoRecibidoDr.PathArchivoXml);
                var sucursal = _db.Sucursales.Where(s => s.Rfc == cfdi.Receptor.Rfc).FirstOrDefault();

                var timbreFiscalDigital = _decodifica.DecodificarTimbre(cfdi, null);

                documentoRecibidoDr.RecibidosXml = new RecibidosXMLDR();
                documentoRecibidoDr.RecibidosPdf = new RecibidosPDFDR();
                //insert files
                byte[] xmlFile = System.IO.File.ReadAllBytes(documentoRecibidoDr.PathArchivoXml);
                documentoRecibidoDr.RecibidosXml.Archivo = xmlFile;
                if (documentoRecibidoDr.PathArchivoPdf != null)
                {
                    byte[] pdfFile = System.IO.File.ReadAllBytes(documentoRecibidoDr.PathArchivoPdf);
                    documentoRecibidoDr.RecibidosPdf.Archivo = pdfFile;
                }
                else
                {
                    documentoRecibidoDr.RecibidosPdf = null;
                    documentoRecibidoDr.CfdiRecibidosPdfId = null;
                }
                //table validaciones
                if (documentoRecibidoDr.Validaciones == null)
                {
                    documentoRecibidoDr.Validaciones = null;
                    documentoRecibidoDr.ValidacionesId = null;
                }
                // table recibidosComprobante
                documentoRecibidoDr.RecibidosComprobante = new RecibidosComprobanteDR()
                {
                    SucursalId = sucursal.Id,
                    SocioComercialId = (int)documentoRecibidoDr.SocioComercialId,
                    Fecha = documentoRecibidoDr.FechaComprobante,
                    Serie = cfdi.Serie,
                    Folio = cfdi.Folio,
                    TipoComprobante = cfdi.TipoDeComprobante,
                    Version = cfdi.Version,
                    FormaPago = cfdi.FormaPago,
                    Moneda = cfdi.Moneda,
                    TipoCambio = (double)cfdi.TipoCambio,
                    LugarExpedicion = cfdi.LugarExpedicion,
                    MetodoPago = cfdi.MetodoPago,
                    Descuento = (double)cfdi.Descuento,
                    Subtotal = (double)cfdi.SubTotal,
                    Total = (double)cfdi.Total,
                    Uuid = timbreFiscalDigital.UUID,
                    FechaTimbrado = timbreFiscalDigital.FechaTimbrado
                };
                if (cfdi.Impuestos != null)
                {
                    documentoRecibidoDr.RecibidosComprobante.TotalImpuestosTrasladados = (double)cfdi.Impuestos.TotalImpuestosTrasladados;
                    documentoRecibidoDr.RecibidosComprobante.TotalImpuestosRetenidos = (double)cfdi.Impuestos.TotalImpuestosTrasladados;
                }
                documentoRecibidoDr.CfdiRecibidosId = null;
                documentoRecibidoDr.EstadoComercial = c_EstadoComercial.EnRevision;
                documentoRecibidoDr.EstadoPago = c_EstadoPago.EnRevision;
                documentoRecibidoDr.PagosId = null;
                documentoRecibidoDr.Referencia = documentoRecibidoDr.Referencia;
                documentoRecibidoDr.SucursalId = sucursal.Id;


                //operaciones en caso de que venga de una comprobacion de gastos
                if (compGastosId.HasValue)
                {
                    documentoRecibidoDr.ComprobacionGastoId = comprobacionGastosId;

                    var comprobacionGastos = _db.ComprobacionesGastos.Find(comprobacionGastosId);
                    double montoAnterior = comprobacionGastos.Monto;
                    double montoActualizado = montoAnterior + documentoRecibidoDr.RecibidosComprobante.Total;
                    comprobacionGastos.Monto = montoActualizado;
                    _db.Entry(comprobacionGastos).State = EntityState.Modified;
                }
                //operaciones en caso de que venga de una complemento de pagos
                if (compPagoId.HasValue)
                {
                    documentoRecibidoDr.EstadoComercial = c_EstadoComercial.Aprobado;
                    documentoRecibidoDr.EstadoPago = c_EstadoPago.Completado;
                    documentoRecibidoDr.PagosId = (int)complementoPagoId;
                    //Table Aprobaciones
                    documentoRecibidoDr.AprobacionesId = null;
                    documentoRecibidoDr.AprobacionesDR = new Aprobaciones()
                    {
                        UsuarioCompletaPagos_id = usuario.Id,
                        FechaCompletaPagos = DateTime.Now,
                    };
                }
                else { 
                    //Table Aprobaciones
                    documentoRecibidoDr.AprobacionesId = null;
                    documentoRecibidoDr.AprobacionesDR = new Aprobaciones()
                    {
                       UsuarioEntrega_Id = usuario.Id,
                       UsuarioSolicitante_Id = (int?)usuarioSolicitanteId,
                       DepartamentoUsuarioSolicitante_Id = (int?)usuarioSolicitanteDepartamentoId,
                       FechaSolicitud = DateTime.Now,
                    };
                }
                _db.DocumentosRecibidos.Add(documentoRecibidoDr);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }


            if (compGastosId.HasValue)
            {
                // Si comprobacionGastoId tiene un valor, redirige a la acción ComprobacionesGastos/Revisar/id
                return RedirectToAction("Revision", "ComprobacionesGastos", new { id = compGastosId.Value });
            }
            else if (compPagoId.HasValue)
            {
                // Si complementoPagoId tiene un valor, redirige a la acción Index
                return RedirectToAction("Pagos", "DocumentosPagos");
            }
            else
            {
                // Si comprobacionGastoId es null, redirige a otra acción
                return RedirectToAction("Index", "DocumentosRecibidos");
            }


        }

        // GET: DocumentosRecibidos/Edit/5
        public ActionResult Revision(int id, int? comprobacionGastoId)
        {
            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "Edit";
            ViewBag.ActionES = "Editar";
            ViewBag.NameHere = "Revisión de Comprobante Recibido";

            if (comprobacionGastoId.HasValue)
            {
                Session["ComprobacionGastosId"] = comprobacionGastoId;
            }
            var documentoRecibido = _db.DocumentosRecibidos.Find(id);
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
            string[] lines = documentoRecibido.ValidacionesDetalle.Split('\n');
            documentoRecibido.DetalleArrays = lines.ToList();
            TempData["AprobadorId"] = null;
            TempData["DepartamentoId"] = null;

            return View(documentoRecibido);
        }

        // POST: DocumentosRecibidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revision(DocumentosRecibidos documentoRecibidoEdit)
        {
            int? compGastosId = (int?)Session["ComprobacionGastosId"];

            try
            {
                int? comprobacionGastosId = (int?)Session["ComprobacionGastosId"];
                Session.Remove("ComprobacionGastosId");

                var usuario = _db.Usuarios.Find(ObtenerUsuario());
                var documentoRecibido = _db.DocumentosRecibidos.Find(documentoRecibidoEdit.Id);
                var usuarioEntrega = _db.Usuarios.Find(documentoRecibido.AprobacionesDR.UsuarioEntrega_Id);

                documentoRecibido.EstadoComercial = documentoRecibidoEdit.EstadoComercial;
                documentoRecibido.Referencia = documentoRecibidoEdit.Referencia;

                if (documentoRecibidoEdit.EstadoComercial == c_EstadoComercial.Aprobado)
                {
                    documentoRecibido.AprobacionesDR.FechaAprobacionComercial = DateTime.Now;
                    documentoRecibido.AprobacionesDR.UsuarioAprobacionComercial_id = usuario.Id;

                    documentoRecibido.EstadoComercial = c_EstadoComercial.Aprobado;
                }

                if (documentoRecibidoEdit.EstadoComercial == c_EstadoComercial.Rechazado)
                {
                    documentoRecibido.AprobacionesDR.FechaRechazo = DateTime.Now;
                    documentoRecibido.AprobacionesDR.UsuarioRechazo_id = usuario.Id;
                    documentoRecibido.AprobacionesDR.DetalleRechazo = documentoRecibidoEdit.AprobacionesDR.DetalleRechazo;

                    documentoRecibido.EstadoComercial = c_EstadoComercial.Rechazado;

                    //Notificación al usuario que entrega
                    _envioEmail.NotificacionCambioEstadoComercial(usuarioEntrega, documentoRecibido, c_EstadoComercial.Rechazado, (int)ObtenerSucursal());
                }
                _db.Entry(documentoRecibido).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch
            {
                return View(documentoRecibidoEdit);
            }



            if (compGastosId.HasValue)
            {
                // Si comprobacionGastoId tiene un valor, redirige a la acción ComprobacionesGastos/Revisar/id
                return RedirectToAction("Revision", "ComprobacionesGastos", new { id = compGastosId.Value });
            }
            else
            {
                // Si comprobacionGastoId es null, redirige a otra acción
                return RedirectToAction("Index", "DocumentosRecibidos");
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

        #endregion Vistas

        #region Validaciones

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
            var documentoRecibido = _db.DocumentosRecibidos.Find(id);
            archivoFisico = documentoRecibido.RecibidosXml.Archivo;

            MemoryStream ms = new MemoryStream(archivoFisico, 0, 0, true, true);
            string nameArchivo = documentoRecibido.CfdiRecibidosSerie + "-" + documentoRecibido.CfdiRecibidosFolio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
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
            var documentoRecibido = _db.DocumentosRecibidos.Find(id);
            archivoFisico = documentoRecibido.RecibidosPdf.Archivo;

            MemoryStream ms = new MemoryStream(archivoFisico, 0, 0, true, true);
            string nameArchivo = documentoRecibido.CfdiRecibidosSerie + "-" + documentoRecibido.CfdiRecibidosFolio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
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
            var documentoRecibido = _db.DocumentosRecibidos.Find(id);
            byte[] archivoFisicoXml = documentoRecibido.RecibidosXml.Archivo;
            byte[] archivoFisicoPdf = documentoRecibido.RecibidosPdf.Archivo;
            string nameArchivo = documentoRecibido.CfdiRecibidosSerie + "-" + documentoRecibido.CfdiRecibidosFolio + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

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