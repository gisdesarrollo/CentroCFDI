using APBox.Context;
using APBox.Control;
using API.Catalogos;
using API.Enums;
using API.Models.DocumentosRecibidos;
using API.Models.Dto;
using API.Operaciones.OperacionesProveedores;
using API.Operaciones.OperacionesRecepcion;
using Aplicacion.LogicaPrincipal.Correos;
using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using Aplicacion.LogicaPrincipal.Expedientes;
using Aplicacion.LogicaPrincipal.Facturas;
using Aplicacion.RecepcionDocumentos;
using Aplicacion.Utilidades;
using AWS;
using SW.Services.Validate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utilerias.LogicaPrincipal;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml;
using API.Integraciones.Clientes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DTOs.Correos;

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
        private readonly AmazonS3Uploader _s3Uploader;
        private readonly AmazonS3Downloader _s3Downloader;
        private readonly ProcesaExpediente _procesaExpediente = new ProcesaExpediente();
        private readonly AmazonS3Helper _s3Helper;

        public DocumentosRecibidosController()
        {
            // Obtener valores de configuración
            var awsAccessKeyId = ConfigurationManager.AppSettings["AWSAccessKeyId"];
            var awsSecretAccessKey = ConfigurationManager.AppSettings["AWSSecretAccessKey"];
            var region = ConfigurationManager.AppSettings["AWSRegion"];
            var bucketName = ConfigurationManager.AppSettings["BucketName"];
            var cloudFrontDomain = ConfigurationManager.AppSettings["CloudFrontDomain"];

            // Inicializar AmazonS3Uploader con los valores de configuración
            _s3Uploader = new AmazonS3Uploader(awsAccessKeyId, awsSecretAccessKey, region, bucketName);
            _s3Helper = new AmazonS3Helper(awsAccessKeyId, awsSecretAccessKey, region, bucketName, cloudFrontDomain);
            _s3Downloader = new AmazonS3Downloader(awsAccessKeyId, awsSecretAccessKey, region, bucketName);
        }

        #endregion variables

        #region Consultas

        //Metodo de Validacion E-Mail para usuarios solicitantes
        [HttpPost]
        public ActionResult ValidadorEmail(DocumentoRecibido documentoRecibidoDr)
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
                    UsuarioSolicitanteId = usuarioSolicitante.Id,
                    UsuarioSolicitanteEmail = email,
                    UsuarioSolicitanteNombre = usuarioSolicitanteNombre,
                    UsuarioSolicitanteDepartamento = usuarioSolicitanteDepartamento
                });
            }
        }

        #endregion Consultas

        #region Vistas

        public ActionResult Index(string q)
        {
            ViewBag.Controller = "DocumentoRecibido";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Índice";
            ViewBag.Button = "CargaDocumentoRecibido";
            ViewBag.Title = "Documentos Recibidos";

            ViewBag.Query = q;

            //get usaurio

            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            var documentosRecibidosModel = new DocumentosRecibidosModel();
            var fechaInicial = DateTime.Today.AddDays(-5);
            var fechaFinal = DateTime.Today.AddDays(1).AddTicks(-1);

            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;

            documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.FiltrarDocumentos(fechaInicial, fechaFinal, sucursal.Id, usuario.Id, ViewBag.Query);

            return View(documentosRecibidosModel);
        }

        [HttpPost]
        public ActionResult Index(DocumentosRecibidosModel documentosRecibidosModel, string q)
        {
            ViewBag.Controller = "DocumentoRecibido";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "CargaDocumentoRecibido";
            ViewBag.Title = "Documentos Recibidos";

            ViewBag.Query = q;

            // Obtén el usuario y la sucursal
            var usuario = _db.Usuarios.Find(ObtenerUsuario());
            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            // Asigna fechas iniciales y finales por defecto si no están definidas
            var fechaInicial = documentosRecibidosModel.FechaInicial != default(DateTime)
                ? documentosRecibidosModel.FechaInicial
                : DateTime.Today.AddDays(-5);
            var fechaFinal = documentosRecibidosModel.FechaFinal != default(DateTime)
                ? documentosRecibidosModel.FechaFinal
                : DateTime.Today.AddDays(1).AddTicks(-1);

            // Asigna las fechas al modelo
            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;

            // Filtra los documentos usando las fechas proporcionadas
            documentosRecibidosModel.DocumentosRecibidos = _procesaDocumentoRecibido.FiltrarDocumentos(fechaInicial, fechaFinal, sucursal.Id, usuario.Id, ViewBag.Query);

            return View(documentosRecibidosModel);
        }

        public ActionResult CargaCfdi(int? comprobacionGastoId, int? complementoPagoId)
        {
            ViewBag.Controller = "DocumentoRecibido";
            ViewBag.Action = "Carga Comprobantes";
            ViewBag.Title = "Carga de Comprobantes";

            if (comprobacionGastoId.HasValue)
            {
                Session["ComprobacionGastosId"] = comprobacionGastoId;
            }
            if (complementoPagoId.HasValue)
            {
                Session["ComplementoPagoId"] = complementoPagoId;
            }

            // Obtener el usuario
            var usuario = _db.Usuarios.Find(ObtenerUsuario());

            // Crear un nuevo objeto DocumentoRecibido
            DocumentoRecibido documentoRecibidoDr = new DocumentoRecibido()
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

            //prellenar datos para comprobantes no fiscales
            documentoRecibidoDr.MonedaId = c_Moneda.MXN;
            documentoRecibidoDr.FechaComprobante = DateTime.Now;

            return View(documentoRecibidoDr);
        }

        [HttpPost]
        public ActionResult CargaCfdi(DocumentoRecibido documentoRecibidoDr)
        {
            ViewBag.Controller = "DocumentoRecibido";
            ViewBag.Action = "Carga Comprobantes";
            ViewBag.Title = "Carga de Comprobantes";

            int? compPagoId = (int?)Session["ComplementoPagoId"];
            int? compGast = (int?)Session["ComprobacionGastosId"];
            var usuario = _db.Usuarios.Find(ObtenerUsuario());

            documentoRecibidoDr.SucursalId = ObtenerSucursal();
            documentoRecibidoDr.ComprobacionGastoId = compGast;
            documentoRecibidoDr.PagosId = compPagoId;

            switch (documentoRecibidoDr.TipoDocumentoRecibido)
            {
                case c_TipoDocumentoRecibido.CFDI:
                    PathArchivosDto archivo;
                    ValidateXmlResponse responseValidacion = new ValidateXmlResponse();
                    ComprobanteCFDI cfdi = new ComprobanteCFDI();

                    try
                    {
                        archivo = SubeArchivo();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", String.Format("No se pudo cargar el archivo: {0}", ex.Message));
                        return View(documentoRecibidoDr);
                    }
                    try
                    {
                        _procesaDocumentoRecibido.ValidaEstructuraCfdi(archivo.PathDestinoXml);
                    }
                    catch (Exception ex)
                    {
                        ManejarErrores(ex, archivo, documentoRecibidoDr);
                        return View(documentoRecibidoDr);
                    }
                    cfdi = _procesaDocumentoRecibido.DecodificaXML(archivo.PathDestinoXml);
                    var timbreFiscalDigital = _decodifica.DecodificarTimbre(cfdi, null);
                    documentoRecibidoDr.DetalleArrays = new List<String>();

                    var dataValidar = CrearDataValidar(cfdi, timbreFiscalDigital, usuario, documentoRecibidoDr, archivo, compPagoId);

                    try
                    {
                        ValidarDatos(dataValidar, compPagoId);
                        SetViewBag(documentoRecibidoDr, cfdi, usuario, compPagoId);
                    }
                    catch (Exception ex)
                    {
                        ManejarErrores(ex, archivo, documentoRecibidoDr);
                        return View(documentoRecibidoDr);
                    }

                    TempData["DocumentoRecibido"] = dataValidar.DocumentoRecibidoDr;
                    TempData["dataValidar"] = dataValidar;
                    return RedirectToAction("Create");

                case c_TipoDocumentoRecibido.ComprobanteNoFiscal:
                    SubirDocumento(documentoRecibidoDr);
                    //var archivoComprobanteNoFiscalByte =ConvertByteFiles(documentoRecibidoDr.ArchivoAdjuntoDR);
                    SetViewBag(documentoRecibidoDr, null, usuario, null);
                    documentoRecibidoDr.FechaComprobante = documentoRecibidoDr.FechaComprobante;
                    documentoRecibidoDr.FechaEntrega = DateTime.Now;
                    //TempData["archivoBytes"] = archivoComprobanteNoFiscalByte;
                    TempData["DocumentoRecibido"] = documentoRecibidoDr;
                    return RedirectToAction("Create");

                case c_TipoDocumentoRecibido.ComprobanteExtranjero:
                    SubirDocumento(documentoRecibidoDr);
                    TempData["DocumentoRecibido"] = documentoRecibidoDr;
                    return RedirectToAction("Create");

                default:
                    ModelState.AddModelError("", "Tipo de documentoRecibido no reconocido");
                    return View(documentoRecibidoDr);
            }
        }

        public ActionResult Create()
        {
            ViewBag.Controller = "DocumentoRecibido";
            ViewBag.Action = "Carga Comprobantes";
            ViewBag.Title = "Carga de Comprobantes";

            // Recuperar el documentoRecibido desde TempData
            var documentoRecibidoDr = TempData["DocumentoRecibido"] as DocumentoRecibido;
            //byte[] archivoBytes = TempData["archivoBytes"] as byte[];

            // Si no hay documentoRecibido en TempData, crear uno nuevo
            if (documentoRecibidoDr == null)
            {
                documentoRecibidoDr = new DocumentoRecibido();
            }
            var dataValidar = TempData["dataValidar"] as Validaciones.DataValidar;
            //SetViewBagDv(dataValidar);
            PopulaDocumentosAsociados(documentoRecibidoDr);

            return View(documentoRecibidoDr);
        }

        [HttpPost]
        public async Task<ActionResult> Create(DocumentoRecibido documentoRecibidoDr)
        {
            ViewBag.Controller = "DocumentoRecibido";
            ViewBag.Action = "Create";
            ViewBag.NameHere = "Documentos Recibidos";

            try
            {
                //Obtener los valores de sesión y vaciarlos
                //estas variables guardan si el DocumentoRecibido está asociado a
                //una comprobación de gastos o un complemento de pago
                int? compGastosId = (int?)Session["ComprobacionGastosId"];
                int? compPagoId = (int?)Session["ComplementoPagoId"];
                Session.Remove("ComprobacionGastosId");
                Session.Remove("ComplementoPagoId");

                //Crear variables para el proceso
                var sucursal = _db.Sucursales.Find(ObtenerSucursal());
                var usuario = _db.Usuarios.Find(ObtenerUsuario());
                var configuraciones = _db.ConfiguracionesDR.FirstOrDefault(c => c.SucursalId == sucursal.Id);

                switch (documentoRecibidoDr.TipoDocumentoRecibido)
                {
                    case c_TipoDocumentoRecibido.CFDI:

                        ProcesarCFDI(documentoRecibidoDr, usuario, sucursal, compGastosId, compPagoId);

                        break;

                    case c_TipoDocumentoRecibido.ComprobanteNoFiscal:
                        ProcesarComprobanteNoFiscal(documentoRecibidoDr, usuario, sucursal, compGastosId, compPagoId);

                        break;

                    case c_TipoDocumentoRecibido.ComprobanteExtranjero:
                        ProcesarComprobanteExtranjero(documentoRecibidoDr, usuario, sucursal, compGastosId, compPagoId);

                        break;

                    default:
                        ModelState.AddModelError("", "Tipo de documentoRecibido no reconocido");
                        return View(documentoRecibidoDr);
                }

                documentoRecibidoDr.CfdiRecibidosId = null;
                documentoRecibidoDr.PagosId = null;
                documentoRecibidoDr.Referencia = documentoRecibidoDr.Referencia;
                documentoRecibidoDr.SucursalId = sucursal.Id;

                ProcesarAprobaciones(documentoRecibidoDr, configuraciones, usuario);

                ProcesarComprobacionGastos(documentoRecibidoDr, compGastosId);

                ProcesarComplementoPago(documentoRecibidoDr, compPagoId, usuario);
                //aqui empieza el bloque que se modifica para la demo de COFCO, se refactorizará después.
                await ProcesarCustomIntegrationCOFCO(documentoRecibidoDr);
                documentoRecibidoDr.DocumentoAsociadoDR = null;

                _db.DocumentosRecibidos.Add(documentoRecibidoDr);

                _db.SaveChanges();

                await CargaAdjuntos(documentoRecibidoDr);

                await CargaComprobante(documentoRecibidoDr);

                return RedireccionarDespuesDeGuardado(compGastosId, compPagoId);
            }
            catch (Exception
            ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(documentoRecibidoDr);
            }
        }

        private Validaciones.DataValidar CrearDataValidar(ComprobanteCFDI cfdi, TimbreFiscalDigital timbreFiscalDigital, Usuario usuario, DocumentoRecibido documentoRecibidoDr, PathArchivosDto archivo, int? compPagoId)
        {
            var sucursal = _db.Sucursales.Find(ObtenerSucursal());
            var socioComercial = cfdi.Emisor == null ? null : _db.SociosComerciales.FirstOrDefault(s => s.Rfc == cfdi.Emisor.Rfc && s.SucursalId == sucursal.Id);

            var dataValidar = new Validaciones.DataValidar
            {
                Cfdi = cfdi,
                Pago = _db.PagoDr.Find(compPagoId),
                TimbreFiscalDigital = timbreFiscalDigital,
                Sucursal = sucursal,
                SocioComercial = socioComercial,
                Usuario = usuario,
                ConfiguracionEmpresa = ConfiguracionEmpresa(),
                DocumentoRecibidoDr = documentoRecibidoDr,
                Archivo = archivo
            };
            return (dataValidar);
        }

        private void ValidarDatos(Validaciones.DataValidar dataValidar, int? compPagoId)
        {
            var validaciones = new Validaciones();

            if (compPagoId == null)
            {
                validaciones.ValidacionesNegocio(dataValidar);
                validaciones.ValidacionesConfiguraciones(dataValidar);
            }
            else
            {
                validaciones.ValidaComplementoPago(dataValidar);
                validaciones.ValidacionesConfiguracionesPagos(dataValidar);
            }
        }

        private void SetViewBag(DocumentoRecibido documentoRecibidoDr, ComprobanteCFDI cfdi, Usuario usuario, int? compPagoId)
        {
            switch (documentoRecibidoDr.TipoDocumentoRecibido)
            {
                case c_TipoDocumentoRecibido.CFDI:
                    TempData["MetodoPago"] = cfdi.MetodoPago;
                    TempData["FormaPago"] = cfdi.FormaPago;
                    TempData["TipoComprobante"] = cfdi.TipoDeComprobante.ToString();
                    TempData["TipoCambio"] = cfdi.TipoCambio;
                    TempData["Moneda"] = cfdi.Moneda;
                    TempData["UsoCFDI"] = cfdi.Receptor.UsoCFDI;
                    TempData["Usuario"] = usuario.NombreCompleto;
                    TempData["Emisor"] = cfdi.Emisor.Nombre;
                    TempData["Receptor"] = cfdi.Receptor.Nombre;
                    TempData["CompPagoId"] = compPagoId;
                    break;

                case c_TipoDocumentoRecibido.ComprobanteNoFiscal:
                    TempData["TipoComprobante"] = "Comprobante No Fiscal";
                    TempData["Moneda"] = documentoRecibidoDr.MonedaId;
                    TempData["Usuario"] = usuario.NombreCompleto;
                    break;

                case c_TipoDocumentoRecibido.ComprobanteExtranjero:
                    break;

                default:
                    break;
            }
        }

        private void ManejarErrores(Exception ex, PathArchivosDto archivo, DocumentoRecibido documentoRecibidoDr)
        {
            var errores = ex.Message.Split('|');
            foreach (var error in errores)
            {
                ModelState.AddModelError("", error);
            }
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

        private DocumentoRecibido ProcesarCFDI(DocumentoRecibido documentoRecibidoDr, Usuario usuario, Sucursal sucursal, int? compGastosId, int? compPagoId)
        {
            ComprobanteCFDI cfdi = new ComprobanteCFDI();
            cfdi = _procesaDocumentoRecibido.DecodificaXML(documentoRecibidoDr.PathArchivoXml);
            var timbreFiscalDigital = _decodifica.DecodificarTimbre(cfdi, null);
            documentoRecibidoDr.RecibidosXml = new RecibidosXMLDR();
            documentoRecibidoDr.RecibidosPdf = new RecibidosPDFDR();

            // Insert files
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

            // Lógica específica para CFDI
            // Table recibidosComprobante
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

            _procesaDocumentoRecibido.DecodificaXML(documentoRecibidoDr.PathArchivoXml);

            return (documentoRecibidoDr);
        }

        private DocumentoRecibido ProcesarComprobanteNoFiscal(DocumentoRecibido documentoRecibidoDr, Usuario usuario, Sucursal sucursal, int? compGastosId, int? compPagoId)
        {
            documentoRecibidoDr = new DocumentoRecibido()
            {
                TipoDocumentoRecibido = c_TipoDocumentoRecibido.ComprobanteNoFiscal,
                FechaComprobante = DateTime.Now,
                MonedaId = documentoRecibidoDr.MonedaId,
                Monto = documentoRecibidoDr.Monto,
                Referencia = documentoRecibidoDr.Referencia,
                SucursalId = sucursal.Id
            };
            return (documentoRecibidoDr);
        }

        private DocumentoRecibido ProcesarComprobanteExtranjero(DocumentoRecibido documentoRecibidoDr, Usuario usuario, Sucursal sucursal, int? compGastosId, int? compPagoId)
        {
            return (documentoRecibidoDr);
        }

        private void ProcesarAprobaciones(DocumentoRecibido documentoRecibidoDr, ConfiguracionesDR configuraciones, Usuario usuario)
        {
            var usuarioSolicitante = _db.Usuarios.Find(documentoRecibidoDr.IdUsuarioSolicitante);
            var usuarioSolicitanteId = usuarioSolicitante.Id;
            var usuarioSolicitanteDepartamentoId = usuarioSolicitante.DepartamentoId;

            documentoRecibidoDr.AprobacionesId = null;
            documentoRecibidoDr.AprobacionesDR = new Aprobaciones();
            documentoRecibidoDr.EstadoComercial = c_EstadoComercial.EnRevision;
            documentoRecibidoDr.EstadoPago = c_EstadoPago.EnRevision;
            documentoRecibidoDr.AprobacionesDR.UsuarioEntrega_Id = usuario.Id;
            documentoRecibidoDr.AprobacionesDR.UsuarioSolicitante_Id = usuarioSolicitante.Id;
            documentoRecibidoDr.AprobacionesDR.DepartamentoUsuarioSolicitante_Id = usuarioSolicitante.DepartamentoId;
            documentoRecibidoDr.AprobacionesDR.FechaSolicitud = DateTime.Now;
            if (configuraciones.AprobacionComercialAutomatica)
            {
                documentoRecibidoDr.EstadoComercial = c_EstadoComercial.Aprobado;
                documentoRecibidoDr.AprobacionesDR.UsuarioAprobacionComercial_id = usuario.Id;
                documentoRecibidoDr.AprobacionesDR.FechaAprobacionComercial = DateTime.Now;
                documentoRecibidoDr.AprobacionesDR.FechaSolicitud = DateTime.Now;
            }

            if (configuraciones.AprobacionPagosAutomatica)
            {
                documentoRecibidoDr.EstadoPago = c_EstadoPago.Completado;
                documentoRecibidoDr.AprobacionesDR.UsuarioAprobacionPagos_id = usuario.Id;
                documentoRecibidoDr.AprobacionesDR.FechaAprobacionPagos = DateTime.Now;
            }
        }

        private void ProcesarComprobacionGastos(DocumentoRecibido documentoRecibidoDr, int? compGastosId)
        {
            if (compGastosId.HasValue)
            {
                documentoRecibidoDr.ComprobacionGastoId = compGastosId;
                var comprobacionGastos = _db.ComprobacionesGastos.Find(compGastosId);
                double montoAnterior = comprobacionGastos.Monto;
                double montoActualizado = montoAnterior + (double)documentoRecibidoDr.Monto;
                comprobacionGastos.Monto = montoActualizado;
                _db.Entry(comprobacionGastos).State = EntityState.Modified;
            }
        }

        private void ProcesarComplementoPago(DocumentoRecibido documentoRecibidoDr, int? compPagoId, Usuario usuario)
        {
            if (compPagoId.HasValue)
            {
                documentoRecibidoDr.EstadoComercial = c_EstadoComercial.Aprobado;
                documentoRecibidoDr.EstadoPago = c_EstadoPago.Completado;
                documentoRecibidoDr.PagosId = compPagoId.Value;
                documentoRecibidoDr.AprobacionesId = null;
                documentoRecibidoDr.AprobacionesDR.UsuarioCompletaPagos_id = usuario.Id;
                documentoRecibidoDr.AprobacionesDR.FechaCompletaPagos = DateTime.Now;
            }
        }

        private Task ProcesarCustomIntegrationCOFCO(DocumentoRecibido documentoRecibidoDr)
        {
            if (documentoRecibidoDr.DocumentoAsociadoDR != null)
            {
                var precioContrato = Request.Form["PrecioContrato"];
                var moneda = c_Moneda.MXN;
                var tipoCambio = Request.Form["TipoCambio"];
                var pesoOrigen = Request.Form["PesoOrigen"];
                var pesoDestino = Request.Form["PesoDestino"];
                var kgMermaExcedente = Request.Form["KgMermaExcedente"];
                var mermaPorcentaje = Request.Form["MermaPorcentaje"];
                var montoNC = Request.Form["MontoNC"];
                decimal MontoNC = Convert.ToDecimal(montoNC);

                // Si todo está bien, asignar
                var ciCofcoReferencias = new Custom_Cofco_FacturasRecibidas_Referencias()
                {
                    SucursalId = documentoRecibidoDr.SucursalId,
                    SocioComercialId = documentoRecibidoDr.SocioComercialId,
                    PrecioContrato = decimal.TryParse(precioContrato, out var precio) ? precio : default(decimal),
                    Moneda = moneda,
                    TipoCambio = decimal.TryParse(tipoCambio, out var tipoCambioDecimal) ? tipoCambioDecimal : default(decimal),
                    PesoOrigen = decimal.TryParse(pesoOrigen, out var pesoOrigenDecimal) ? pesoOrigenDecimal : default(decimal),
                    PesoDestino = decimal.TryParse(pesoDestino, out var pesoDestinoDecimal) ? pesoDestinoDecimal : default(decimal),
                    KgMermaExcedente = decimal.TryParse(kgMermaExcedente, out var kgMermaExcedenteDecimal) ? kgMermaExcedenteDecimal : default(decimal)
                };
                decimal mermaPorcentajeDecimal;
                bool isValid = decimal.TryParse(mermaPorcentaje, out mermaPorcentajeDecimal);

                if (isValid && mermaPorcentajeDecimal > 0.2m)
                {
                    var documentoAsociadoDR = new DocumentoAsociadoDR()
                    {
                        SucursalId = documentoRecibidoDr.SucursalId,
                        SocioComercialId = documentoRecibidoDr.SocioComercialId,
                        TipoDocumentoAsociado = TipoDocumentoAsociado.NotaCredito,
                        Descripcion = "Merma excedente",
                        FechaSolicitud = DateTime.Now,
                        Monto = MontoNC,
                        MonedaId = c_Moneda.MXN
                    };
                    _db.DocumentoAsociadoDR.Add(documentoAsociadoDR);
                }
                _db.Custom_Cofco_FacturasRecibidas_Referencias.Add(ciCofcoReferencias);
            }
            else
            {
                var documentoAsociado = _db.DocumentoAsociadoDR.Find(documentoRecibidoDr.DocumentoAsociadoDRId);

                documentoAsociado.FechaEntrega = DateTime.Now;
            }

            _db.SaveChanges();

            return Task.CompletedTask;
        }

        private ActionResult RedireccionarDespuesDeGuardado(int? compGastosId, int? compPagoId)
        {
            if (compGastosId.HasValue)
            {
                return RedirectToAction("Revision", "ComprobacionesGastos", new { id = compGastosId.Value });
            }
            else if (compPagoId.HasValue)
            {
                return RedirectToAction("Pagos", "DocumentosPagos");
            }
            else
            {
                return RedirectToAction("Index", "DocumentosRecibidos");
            }
        }

        public ActionResult Revision(int id, int? comprobacionGastoId)
        {
            ViewBag.Controller = "DocumentoRecibido";
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
                documentoRecibido.IsProveedor = true;
                ViewBag.isProveedor = "Proveedor";
            }
            else
            {
                documentoRecibido.IsProveedor = false;
                ViewBag.isProveedor = "Usuario";
            }
            // Splitting the string into lines
            if (documentoRecibido.TipoDocumentoRecibido == c_TipoDocumentoRecibido.CFDI)
            {
                string[] lines = documentoRecibido.ValidacionesDetalle.Split('\n');
                documentoRecibido.DetalleArrays = lines.ToList();
            }

            TempData["AprobadorId"] = null;
            TempData["DepartamentoId"] = null;

            return View(documentoRecibido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revision(DocumentoRecibido documentoRecibidoEdit)
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
                    documentoRecibido.EstadoPago = c_EstadoPago.Rechazado;

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

        public ActionResult Delete(int id)
        {
            return View();
        }

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

        #region Upload a S3

        [HttpPost]
        private async Task CargaComprobante(DocumentoRecibido documentoRecibido)
        {
            var sucursalId = ObtenerSucursal();
            var grupoId = ObtenerGrupo();

            switch (documentoRecibido.TipoDocumentoRecibido)
            {
                case c_TipoDocumentoRecibido.CFDI:

                    break;

                case c_TipoDocumentoRecibido.ComprobanteNoFiscal:

                    var basePath = $"DocumentosRecibidos/ComprobanteNoFiscal/{grupoId}/{sucursalId}/{documentoRecibido.Id}/{documentoRecibido.FechaEntrega.Year}/{(int)documentoRecibido.FechaEntrega.Month}/{(int)documentoRecibido.FechaEntrega.Day}";

                    PathArchivosDto pathArchivos = new PathArchivosDto();
                    string directoryPath = Server.MapPath("~/Archivos/DocumentosRecibidos");

                    var nombreArchivo = documentoRecibido.PathNoFiscal;
                    var key = $"{basePath}/{nombreArchivo}";
                    string pathCompletoNoFiscal = directoryPath + "/" + nombreArchivo;
                    HttpPostedFileBase ArchivoComprobanteNoFiscal = new FileFromPath(pathCompletoNoFiscal);

                    await UploadFileToS3(ArchivoComprobanteNoFiscal, key);

                    var comprobanteNoFiscal = new ComprobanteNoFiscal
                    {
                        DocumentoRecibidoId = documentoRecibido.Id,
                        SucursalId = documentoRecibido.SucursalId,
                        Referencia = documentoRecibido.Referencia,
                        FechaCreacion = DateTime.Now,
                        PathS3 = key
                    };

                    // Guardar el resto de la información del expedienteFiscal en la base de datos
                    _db.ComprobanteNoFiscal.Add(comprobanteNoFiscal);
                    await _db.SaveChangesAsync();

                    //eliminar archivo local del path directoryPath
                    if (System.IO.File.Exists(directoryPath + "/" + nombreArchivo))
                    {
                        System.IO.File.Delete(directoryPath + "/" + nombreArchivo);
                    }

                    break;

                case c_TipoDocumentoRecibido.ComprobanteExtranjero:

                    break;

                default:
                    break;
            }

            Session.Remove("socComlId");
        }

        [HttpPost]
        private async Task CargaAdjuntos(DocumentoRecibido documentoRecibido)
        {
            // Verificar que se ha subido un archivo
            if (documentoRecibido.AdjuntoDR != null && documentoRecibido.AdjuntoDR.ContentLength > 0)
            {
                try
                {
                    var sucursalId = ObtenerSucursal();
                    var grupoId = ObtenerGrupo();

                    var basePath = $"DocumentosRecibidos/DocumentosAdjuntos/{grupoId}/{sucursalId}/{documentoRecibido.Id}/{documentoRecibido.FechaEntrega.Year}/{(int)documentoRecibido.FechaEntrega.Month}/{(int)documentoRecibido.FechaEntrega.Day}";

                    //PathArchivosDto pathArchivos = new PathArchivosDto();
                    //string directoryPath = Server.MapPath("~/Archivos/DocumentosAdjuntos");
                    // Obtener el nombre del archivo
                    var nombreArchivo = Path.GetFileName(documentoRecibido.AdjuntoDR.FileName);
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(documentoRecibido.AdjuntoDR, key);
                    var archivoAdjuntoDR = new AdjuntoDR
                    {
                        DocumentoRecibidoId = documentoRecibido.Id,
                        SucursalId = documentoRecibido.SucursalId,
                        SocioComercialId = documentoRecibido.SocioComercialId,
                        FechaCreacion = DateTime.Now,
                        PathS3Adjunto = key
                    };

                    // Guardar el resto de la información del expedienteFiscal en la base de datos
                    _db.AdjuntoDr.Add(archivoAdjuntoDR);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new Exception("Error no encontro los archivos adjuntos: null");
            }
        }

        private async Task UploadFileToS3(HttpPostedFileBase file, string key)
        {
            using (var stream = file.InputStream)
            {
                await _s3Uploader.UploadFileAsync(stream, key, file.ContentType);
            }
        }

        public ActionResult Download(string filePath)
        {
            // Generar URL de CloudFront pre-firmada
            var preSignedUrl = _s3Helper.GenerateCloudFrontPreSignedURL(filePath);

            using (var client = new WebClient())
            {
                try
                {
                    var fileBytes = client.DownloadData(preSignedUrl);
                    var fileName = System.IO.Path.GetFileName(filePath);

                    return File(fileBytes, "application/octet-stream", fileName);
                }
                catch (WebException ex)
                {
                    // Manejar el error de descarga
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        #endregion Upload a S3

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
            var configuracion = _db.ConfiguracionesDR.FirstOrDefault(c => c.SucursalId == sucursalId.Id);

            if (configuracion == null)
            {
                return null;
            }

            return configuracion;
        }

        private void PopulaDocumentosAsociados(DocumentoRecibido documentoRecibido)
        {
            // Suponiendo que tienes un contexto de base de datos llamado '_db'
            var documentosAsociados = _db.DocumentoAsociadoDR
                .AsEnumerable() // Cambiamos a LINQ to Objects para usar propiedades no mapeadas
                .Select(d => new
                {
                    d.Id,
                    Nombre = string.Format("{0} - {1} - {2}", d.TipoDocumentoAsociado, d.Monto, d.MonedaId)
                }).ToList();

            ViewBag.DocumentosAsociados = new SelectList(documentosAsociados, "Id", "Nombre");
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

        public DocumentoRecibido SubirDocumento(DocumentoRecibido documentoRecibido)
        {
            PathArchivosDto pathArchivos = new PathArchivosDto();
            string directoryPath = Server.MapPath("~/Archivos/DocumentosRecibidos");

            if (documentoRecibido.ArchivoComprobanteCfdiXml != null && documentoRecibido.ArchivoComprobanteCfdiXml.ContentLength > 0 ||
                documentoRecibido.ArchivoComprobanteCfdiPdf != null && documentoRecibido.ArchivoComprobanteCfdiPdf.ContentLength > 0 ||
                documentoRecibido.ArchivoComprobanteNoFiscal != null && documentoRecibido.ArchivoComprobanteNoFiscal.ContentLength > 0 ||
                documentoRecibido.ArchivoComprobanteExtranjero != null && documentoRecibido.ArchivoComprobanteExtranjero.ContentLength > 0 ||
                documentoRecibido.ArchivoAdjuntos != null && documentoRecibido.ArchivoAdjuntos.ContentLength > 0)
            {
                try
                {
                    // Guardar cada archivo si está presente
                    documentoRecibido.PathArchivoXml = SaveFile(documentoRecibido.ArchivoComprobanteCfdiXml, directoryPath, "ArchivoComprobanteCfdiXml");
                    documentoRecibido.PathArchivoPdf = SaveFile(documentoRecibido.ArchivoComprobanteCfdiPdf, directoryPath, "ArchivoComprobanteCfdiPdf");
                    documentoRecibido.PathNoFiscal = SaveFile(documentoRecibido.ArchivoComprobanteNoFiscal, directoryPath, "ArchivoComprobanteNoFiscal");
                    documentoRecibido.PathExtranjero = SaveFile(documentoRecibido.ArchivoComprobanteExtranjero, directoryPath, "ArchivoComprobanteExtranjero");
                    documentoRecibido.PathAdjunto = SaveFile(documentoRecibido.ArchivoAdjuntos, directoryPath, "ArchivoAdjuntos");

                    return (documentoRecibido);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ha ocurrido un error al guardar el registro: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Por favor, selecciona al menos un archivo para subir.");
            }

            return (documentoRecibido);
        }

        private string SaveFile(HttpPostedFileBase file, string directoryPath, string a)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Generar un nombre único para el archivo
                    string extension = Path.GetExtension(file.FileName);
                    string newFileName = $"{Guid.NewGuid()}{extension}";

                    // Definir la ruta de almacenamiento
                    string path = Path.Combine(directoryPath, newFileName);

                    // Crear el directorio si no existe
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Guardar el archivo
                    file.SaveAs(path);

                    return (newFileName);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ha ocurrido un error al subir el archivo: " + ex.Message);
                    return null;
                }
            }
            return null;
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

        public async Task<ActionResult> DescargaComprobanteNoFiscal(int id)
        {
            var comprobanteNoFiscal = _db.ComprobanteNoFiscal.Where(nf => nf.DocumentoRecibidoId == id).FirstOrDefault();
            string fileName = Path.GetFileName(comprobanteNoFiscal.PathS3);

            var stream = await _s3Downloader.DownloadFileAsync(comprobanteNoFiscal.PathS3);
            string contentType = GetContentType(fileName);

            return File(stream, contentType, fileName);
        }

        private void AddFileToZip(ZipArchive archive, byte[] fileBytes, string entryName)
        {
            var entry = archive.CreateEntry(entryName);
            using (var entryStream = entry.Open())
            {
                entryStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        private string GetContentType(string fileName)
        {
            // Example logic to determine content type based on file extension
            string extension = Path.GetExtension(fileName);

            switch (extension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";

                case ".xml":
                    return "application/xml";

                case ".csv":
                    return "text/csv";

                case ".xlsx":
                case ".xls":
                    return "application/vnd.ms-excel";

                case ".xslt":
                case ".xsl":
                    return "application/xslt+xml";

                default:
                    return "application/octet-stream"; // Default content type if not recognized
            }
        }

        #endregion Validaciones

        #region Aprobaciones Ajax

        [HttpPost]
        public ActionResult AprobarDocumento(int id)
        {
            // Obtén el documentoRecibido por su ID
            var documentoRecibido = _db.DocumentosRecibidos.Find(id);
            if (documentoRecibido == null)
            {
                return HttpNotFound();
            }

            // Cambia el estado del documentoRecibido
            documentoRecibido.EstadoComercial = c_EstadoComercial.Aprobado;
            documentoRecibido.AprobacionesDR.UsuarioAprobacionComercial_id = ObtenerUsuario();
            documentoRecibido.AprobacionesDR.FechaAprobacionComercial = DateTime.Now;

            _db.SaveChanges();

            return Json(new { success = true, estado = documentoRecibido.EstadoComercial.ToString() });
        }

        #endregion Aprobaciones Ajax

        public ActionResult GetPDF(int? id)
        {
            if (id == null)
            {
                return HttpNotFound("El ID proporcionado no es válido.");
            }
            var recibidoPdf = _db.RecibidoPdfDr.Find(id.Value);
            var documentoRecibido = _db.DocumentosRecibidos.Where(d => d.CfdiRecibidosPdfId == recibidoPdf.Id).FirstOrDefault();

            // Convierte el arreglo de bytes a un MemoryStream
            var stream = new MemoryStream(recibidoPdf.Archivo);

            // Devuelve el archivo como un FileStreamResult
            return new FileStreamResult(stream, "application/pdf");
        }

        public ActionResult GetPDfAdjunto(int? id)
        {
            var documentoRecibido = _db.DocumentosRecibidos.Find(id.Value);
            if (documentoRecibido == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Documento no encontrado");
            }
            var abjunto = _db.AdjuntoDr.Where(a => a.DocumentoRecibidoId == documentoRecibido.Id).FirstOrDefault();
            if (abjunto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Adjunto no encontrado");
            }
            string filePath = abjunto.PathS3Adjunto;

            // Generar URL de CloudFront pre-firmada
            var preSignedUrl = _s3Helper.GenerateCloudFrontPreSignedURL(filePath);
            var client = new WebClient();

            try
            {
                var fileBytes = client.DownloadData(preSignedUrl);
                var stream = new MemoryStream(fileBytes);

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (WebException ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                client.Dispose();
            }
        }

        public ActionResult GetImage()
        {
            var imagePath = @"C:\\Users\\Admin\\Downloads\\image-20240730-164626.png";
            var imageFile = System.IO.File.ReadAllBytes(imagePath);
            var stream = new MemoryStream(imageFile);
            return new FileStreamResult(stream, "image/png");
        }

        public ActionResult GetExcel()
        {
            // Ruta del archivo Excel
            var filePath = @"C:\\Users\\Admin\\Downloads\\catCFDI_V_4_20240731.xls";

            // Verifica si el archivo existe
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("El archivo no se encontró.");
            }

            // Abre el archivo como FileStream sin usar 'using'
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Devuelve el archivo como un FileStreamResult
            return new FileStreamResult(stream, "application/vnd.ms-excel");
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