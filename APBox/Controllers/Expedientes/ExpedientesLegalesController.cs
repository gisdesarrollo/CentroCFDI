using APBox.Context;
using API.Catalogos;
using API.Enums;
using API.Models.Expedientes;
using API.Models.ExpedientesFiscales;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.Expedientes;
using Aplicacion.LogicaPrincipal.Expedientes;
using Aplicacion.Utilidades;
using AWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace APBox.Controllers.Expedientes
{
    [APBox.Control.SessionExpire]
    public class ExpedientesLegalesController : Controller
    {
        #region Variables

        private readonly AmazonS3Uploader _s3Uploader;
        private readonly APBoxContext _db = new APBoxContext();
        private readonly ProcesaExpediente _procesaExpediente = new ProcesaExpediente();
        private readonly AmazonS3Helper _s3Helper;

        #endregion Variables
        // GET: ExpedientesLegales
        public ActionResult Index(int? socioComercialId)
        {
            var socioComercial = _db.SociosComerciales.Find(socioComercialId);
            ViewBag.Controller = "ExpedientesLegales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Expediente Legal: " + socioComercial.RazonSocial;
            ViewBag.SocioComercialId = socioComercialId;
            Session["socComlId"] = socioComercialId;

            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            var expedientesLegalesModel = new ExpedientesLegalesModel();
            var fechaInicial = DateTime.Today.AddMonths(-60);
            var fechaFinal = DateTime.Today.AddDays(1).AddTicks(-1);

            expedientesLegalesModel.FechaInicial = fechaInicial;
            expedientesLegalesModel.FechaFinal = fechaFinal;

            var expedientesLegales = _procesaExpediente.FiltrarExpedienteLegal(fechaInicial, fechaFinal, sucursal.Id, (int)socioComercialId);

            return View(expedientesLegales);
        }

        [HttpPost]
        public ActionResult Index(ExpedientesLegalesModel expedientesLegalesModel, int? socioComercialId)
        {
            var socioComercial = _db.SociosComerciales.Find(socioComercialId);
            ViewBag.Controller = "ExpedientesLegales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Expediente Legal: " + socioComercial.RazonSocial;
            ViewBag.SocioComercialId = socioComercialId;
            Session["socComlId"] = socioComercialId;


            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            var fechaInicial = expedientesLegalesModel.FechaInicial;
            var fechaFinal = expedientesLegalesModel.FechaFinal;

            expedientesLegalesModel.ExpedientesLegales = _procesaExpediente.FiltrarExpedienteLegal(fechaInicial, fechaFinal, sucursal.Id, (int)socioComercialId);

            return View(expedientesLegalesModel);
        }

        // GET: ExpedientesLegales/Create
        public ActionResult Create(int? socioComercialId)
        {
            ViewBag.Controller = "ExpedientesLegales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Agregar Expedientes Legales";
            ViewBag.SocioComercialId = socioComercialId;
            ExpedienteLegal expedienteLegal = new ExpedienteLegal() { };

            return View(expedienteLegal);
        }

        // POST: ExpedientesLegales/Create
        [HttpPost]
        public async Task<ActionResult> Create(ExpedienteLegal expediente)
        {
            var sucursalId = ObtenerSucursal();
            var grupoId = ObtenerGrupo();
            var socioComercialId = (int)Session["socComlId"];


            if (ModelState.IsValid)
            {

                var basePath = $"ExpedientesLegales/{grupoId}/{sucursalId}/{socioComercialId}";

                if (expediente.ArchivoActaConstitutiva != null && expediente.ArchivoActaConstitutiva.ContentLength > 0)
                {
                    expediente.FechaCreacionActaConstitutiva = DateTime.Now;
                    var fechaformateadoActaConstitutiva = expediente.FechaCreacionActaConstitutiva?.ToString("dd-MM-yyyy") ?? null;
                    string extensionActaConstitutiva = Path.GetExtension(expediente.ArchivoActaConstitutiva.FileName);
                    var nombreArchivo = $"ActaConstitutiva_{fechaformateadoActaConstitutiva}{extensionActaConstitutiva}";
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(expediente.ArchivoActaConstitutiva, key);

                    expediente.PathActaConstitutiva = key;
                    expediente.UsuarioIdActaConstitutiva = ObtenerUsuario();
                    expediente.AprobacionActaConstitutiva = 0;
                }

                if (expediente.ArchivoCedulaIdentificacionFiscal != null && expediente.ArchivoCedulaIdentificacionFiscal.ContentLength > 0)
                {
                    expediente.FechaCreacionCedulaIdentificacionFiscal = DateTime.Now;
                    var fechaformateadoCedulaIdentificacionFiscal = expediente.FechaCreacionCedulaIdentificacionFiscal?.ToString("dd-MM-yyyy") ?? null;
                    string extensionCedulaIdentificacionFiscal = Path.GetExtension(expediente.ArchivoCedulaIdentificacionFiscal.FileName);
                    var nombreArchivo = $"CedulaIdentificacionFiscal_{fechaformateadoCedulaIdentificacionFiscal}{extensionCedulaIdentificacionFiscal}";
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(expediente.ArchivoCedulaIdentificacionFiscal, key);

                    expediente.PathCedulaIdentificacionFiscal = key;
                    expediente.UsuarioIdCedulaIdentificacionFiscal = ObtenerUsuario();
                    expediente.AprobacionCedulaIdentificacionFiscal = 0;
                }
                if (expediente.ArchivoComprobanteDomicilio != null && expediente.ArchivoComprobanteDomicilio.ContentLength > 0)
                {
                    expediente.FechaCreacionComprobanteDomicilio = DateTime.Now;
                    var fechaformateadoComprobanteDomicilio = expediente.FechaCreacionComprobanteDomicilio?.ToString("dd-MM-yyyy") ?? null;
                    string extensionComprobanteDomicilio = Path.GetExtension(expediente.ArchivoComprobanteDomicilio.FileName);
                    var nombreArchivo = $"ComprobanteDomicilio_{fechaformateadoComprobanteDomicilio}{extensionComprobanteDomicilio}";
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(expediente.ArchivoComprobanteDomicilio, key);

                    expediente.PathComprobanteDomicilio = key;
                    expediente.UsuarioIdComprobanteDomicilio = ObtenerUsuario();
                    expediente.AprobacionComprobanteDomicilio = 0;
                }
                if (expediente.ArchivoIdentificacionOficialRepLeg != null && expediente.ArchivoIdentificacionOficialRepLeg.ContentLength > 0)
                {
                    expediente.FechaCreacionIdentificacionOficialRepLeg = DateTime.Now;
                    var fechaformateadoIdentificacionOficialRepLeg = expediente.FechaCreacionIdentificacionOficialRepLeg?.ToString("dd-MM-yyyy") ?? null;
                    string extensionIdentificacionOficialRepLeg = Path.GetExtension(expediente.ArchivoIdentificacionOficialRepLeg.FileName);
                    var nombreArchivo = $"IdentificacionOficialRepLeg_{fechaformateadoIdentificacionOficialRepLeg}{extensionIdentificacionOficialRepLeg}";
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(expediente.ArchivoIdentificacionOficialRepLeg, key);

                    expediente.PathIdentificacionOficialRepLeg = key;
                    expediente.UsuarioIdIdentificacionOficialRepLeg = ObtenerUsuario();
                    expediente.AprobacionIdentificacionOficialRepLeg = 0;
                }
                if (expediente.ArchivoEstadoCuentaBancario != null && expediente.ArchivoEstadoCuentaBancario.ContentLength > 0)
                {
                    expediente.FechaCreacionEstadoCuentaBancario = DateTime.Now;
                    var fechaformateadoEstadoCuentaBancario = expediente.FechaCreacionEstadoCuentaBancario?.ToString("dd-MM-yyyy") ?? null;
                    string extensionEstadoCuentaBancario = Path.GetExtension(expediente.ArchivoEstadoCuentaBancario.FileName);
                    var nombreArchivo = $"EstadoCuentaBancario_{fechaformateadoEstadoCuentaBancario}{extensionEstadoCuentaBancario}";
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(expediente.ArchivoEstadoCuentaBancario, key);

                    expediente.PathEstadoCuentaBancario = key;
                    expediente.UsuarioIdEstadoCuentaBancario = ObtenerUsuario();
                    expediente.AprobacionEstadoCuentaBancario = 0;
                }
                expediente.SucursalId = sucursalId;
                expediente.SocioComercialId = socioComercialId;
                
                // Guardar el resto de la información del expedienteFiscal en la base de datos
                _db.ExpedientesLegales.Add(expediente);
                await _db.SaveChangesAsync();

                Session.Remove("socComlId");
                return RedirectToAction("Index", "ExpedientesLegales", new { id = socioComercialId, socioComercialId = socioComercialId });
            }

            ViewBag.SocioComercialId = socioComercialId;
            return View(expediente);
        }

        public ActionResult Edit(int? socioComercialId)
        {
            ViewBag.Controller = "ExpedientesLegales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Editar Expedientes Legales";
            ViewBag.SocioComercialId = socioComercialId;
            var sucursalId = ObtenerSucursal();
            ExpedienteLegal expedienteLegal = _db.ExpedientesLegales.Where(e=> e.SocioComercialId == socioComercialId && e.SucursalId == sucursalId).FirstOrDefault();
            expedienteLegal.ActaConstitutivaName = Path.GetFileName(expedienteLegal.PathActaConstitutiva) ?? "";
            expedienteLegal.CedulaIdentificacionFiscalName = Path.GetFileName(expedienteLegal.PathCedulaIdentificacionFiscal) ?? "";
            expedienteLegal.IdentificacionOficialRepLegName = Path.GetFileName(expedienteLegal.PathIdentificacionOficialRepLeg) ?? "";
            expedienteLegal.ComprobanteDomicilioName = Path.GetFileName(expedienteLegal.PathComprobanteDomicilio) ?? "";
            expedienteLegal.EstadoCuentaBancarioName = Path.GetFileName(expedienteLegal.PathEstadoCuentaBancario) ?? "";
            return View(expedienteLegal);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ExpedienteLegal expedienteLegal)
        {
            ViewBag.Controller = "ExpedientesLegales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Editar Expedientes Legales";
            ViewBag.SocioComercialId = expedienteLegal.SocioComercialId;

            var grupoId = ObtenerGrupo();
            var sucursalId = expedienteLegal.SucursalId;
            var socioComercialId = expedienteLegal.SocioComercialId;
            var basePath = $"ExpedientesLegales/{grupoId}/{sucursalId}/{socioComercialId}";

            if (expedienteLegal.ArchivoActaConstitutiva != null && expedienteLegal.ArchivoActaConstitutiva.ContentLength > 0)
            {
                expedienteLegal.FechaCreacionActaConstitutiva = DateTime.Now;
                var fechaformateadoActaConstitutiva = expedienteLegal.FechaCreacionActaConstitutiva?.ToString("dd-MM-yyyy") ?? null;
                string extensionActaConstitutiva = Path.GetExtension(expedienteLegal.ArchivoActaConstitutiva.FileName);
                var nombreArchivo = $"ArchivoActaConstitutiva_{fechaformateadoActaConstitutiva}{extensionActaConstitutiva}";
                var key = $"{basePath}/{nombreArchivo}";
                await UploadFileToS3(expedienteLegal.ArchivoActaConstitutiva, key);

                expedienteLegal.PathActaConstitutiva = key;
                expedienteLegal.UsuarioIdActaConstitutiva = ObtenerUsuario();
                
            }

            if (expedienteLegal.ArchivoCedulaIdentificacionFiscal != null && expedienteLegal.ArchivoCedulaIdentificacionFiscal.ContentLength > 0)
            {
                expedienteLegal.FechaCreacionCedulaIdentificacionFiscal = DateTime.Now;
                var fechaformateadoCedulaIdentificacionFiscal = expedienteLegal.FechaCreacionCedulaIdentificacionFiscal?.ToString("dd-MM-yyyy") ?? null;
                string extensionCedulaIdentificacionFiscal = Path.GetExtension(expedienteLegal.ArchivoCedulaIdentificacionFiscal.FileName);
                var nombreArchivo = $"ArchivoCedulaIdentificacionFiscal_{fechaformateadoCedulaIdentificacionFiscal}{extensionCedulaIdentificacionFiscal}";
                var key = $"{basePath}/{nombreArchivo}";
                await UploadFileToS3(expedienteLegal.ArchivoCedulaIdentificacionFiscal, key);

                expedienteLegal.PathCedulaIdentificacionFiscal = key;
                expedienteLegal.UsuarioIdCedulaIdentificacionFiscal = ObtenerUsuario();
                
            }
            if (expedienteLegal.ArchivoComprobanteDomicilio != null && expedienteLegal.ArchivoComprobanteDomicilio.ContentLength > 0)
            {
                expedienteLegal.FechaCreacionComprobanteDomicilio = DateTime.Now;
                var fechaformateadoComprobanteDomicilio = expedienteLegal.FechaCreacionComprobanteDomicilio?.ToString("dd-MM-yyyy") ?? null;
                string extensionComprobanteDomicilio = Path.GetExtension(expedienteLegal.ArchivoComprobanteDomicilio.FileName);
                var nombreArchivo = $"ArchivoComprobanteDomicilio_{fechaformateadoComprobanteDomicilio}{extensionComprobanteDomicilio}";
                var key = $"{basePath}/{nombreArchivo}";
                await UploadFileToS3(expedienteLegal.ArchivoComprobanteDomicilio, key);

                expedienteLegal.PathComprobanteDomicilio = key;
                expedienteLegal.UsuarioIdComprobanteDomicilio = ObtenerUsuario();
                
            }
            if (expedienteLegal.ArchivoIdentificacionOficialRepLeg != null && expedienteLegal.ArchivoIdentificacionOficialRepLeg.ContentLength > 0)
            {
                expedienteLegal.FechaCreacionIdentificacionOficialRepLeg = DateTime.Now;
                var fechaformateadoIdentificacionOficialRepLeg = expedienteLegal.FechaCreacionIdentificacionOficialRepLeg?.ToString("dd-MM-yyyy") ?? null;
                string extensionIdentificacionOficialRepLeg = Path.GetExtension(expedienteLegal.ArchivoIdentificacionOficialRepLeg.FileName);
                var nombreArchivo = $"ArchivoIdentificacionOficialRepLeg_{fechaformateadoIdentificacionOficialRepLeg}{extensionIdentificacionOficialRepLeg}";
                var key = $"{basePath}/{nombreArchivo}";
                await UploadFileToS3(expedienteLegal.ArchivoIdentificacionOficialRepLeg, key);

                expedienteLegal.PathIdentificacionOficialRepLeg = key;
                expedienteLegal.UsuarioIdIdentificacionOficialRepLeg = ObtenerUsuario();
                
            }
            if (expedienteLegal.ArchivoEstadoCuentaBancario != null && expedienteLegal.ArchivoEstadoCuentaBancario.ContentLength > 0)
            {
                expedienteLegal.FechaCreacionEstadoCuentaBancario = DateTime.Now;
                var fechaformateadoEstadoCuentaBancario = expedienteLegal.FechaCreacionEstadoCuentaBancario?.ToString("dd-MM-yyyy") ?? null;
                string extensionEstadoCuentaBancario = Path.GetExtension(expedienteLegal.ArchivoEstadoCuentaBancario.FileName);
                var nombreArchivo = $"ArchivoEstadoCuentaBancario_{fechaformateadoEstadoCuentaBancario}{extensionEstadoCuentaBancario}";
                var key = $"{basePath}/{nombreArchivo}";
                await UploadFileToS3(expedienteLegal.ArchivoEstadoCuentaBancario, key);

                expedienteLegal.PathEstadoCuentaBancario = key;
                expedienteLegal.UsuarioIdEstadoCuentaBancario = ObtenerUsuario();
                
            }
           
            // Guardar el resto de la información del expedienteFiscal en la base de datos
            _db.Entry(expedienteLegal).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "ExpedientesLegales", new { id = socioComercialId, socioComercialId = socioComercialId });
            
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
        public byte[] GetByteFile(string filePath)
        {
            // Generar URL de CloudFront pre-firmada
            var preSignedUrl = _s3Helper.GenerateCloudFrontPreSignedURL(filePath);

            using (var client = new WebClient())
            {
                try
                {
                    var fileBytes = client.DownloadData(preSignedUrl);
                    return fileBytes;
                }
                catch (WebException ex)
                {
                    // Manejar el error de descarga
                    return null;
                }
            }
        }
        #region Consultas
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

        public ExpedientesLegalesController()
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

        }
        #endregion
    }
}
