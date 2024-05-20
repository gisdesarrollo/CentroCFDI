using APBox.Context;
using API.Enums;
using API.Models.ExpedientesFiscales;
using API.Operaciones.Expedientes;
using API.Operaciones.OperacionesProveedores;
using Aplicacion.LogicaPrincipal.Expedientes;
using Aplicacion.Utilidades;
using AWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace APBox.Controllers.Expedientes
{
    [APBox.Control.SessionExpire]
    public class ExpedientesFiscalesController : Controller
    {
        #region Variables

        private readonly AmazonS3Uploader _s3Uploader;
        private readonly APBoxContext _db = new APBoxContext();
        private readonly ProcesaExpediente _procesaExpediente = new ProcesaExpediente();
        private readonly AmazonS3Helper _s3Helper;

        #endregion Variables

        // GET: Expedientes/Index
        public ActionResult Index(int? socioComercialId)
        {
            var socioComercial = _db.SociosComerciales.Find(socioComercialId);
            ViewBag.Controller = "ExpedientesFiscales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Expediente Fiscal: " + socioComercial.RazonSocial;

            Session["socComlId"] = socioComercialId;

            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            var expedientesFiscalesModel = new ExpedientesFiscalesModel();
            var fechaInicial = DateTime.Today.AddDays(-10);
            var fechaFinal = DateTime.Today.AddDays(1).AddTicks(-1);

            expedientesFiscalesModel.FechaInicial = fechaInicial;
            expedientesFiscalesModel.FechaFinal = fechaFinal;

            var expedientesFiscales = _procesaExpediente.Filtrar(fechaInicial, fechaFinal, sucursal.Id, (int)socioComercialId);

            return View(expedientesFiscales);
        }

        // POST: Expedientes/Index
        [HttpPost]
        public ActionResult Index(ExpedientesFiscalesModel expedientesFiscalesModel, int? socioComercialId)
        {
            var socioComercial = _db.SociosComerciales.Find(socioComercialId);
            ViewBag.Controller = "ExpedientesFiscales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Expediente Fiscal: " + socioComercial.RazonSocial;

            Session["socComlId"] = socioComercialId;


            var sucursal = _db.Sucursales.Find(ObtenerSucursal());

            var fechaInicial = expedientesFiscalesModel.FechaInicial;
            var fechaFinal = expedientesFiscalesModel.FechaFinal;

            expedientesFiscalesModel.ExpedientesFiscales = _procesaExpediente.Filtrar(fechaInicial, fechaFinal, sucursal.Id, (int)socioComercialId);

            return View(expedientesFiscalesModel);
        }

        // GET: Expedientes/Createade
        public ActionResult Create(int? socioComercialId)
        {
            ViewBag.Controller = "ExpedientesFiscales";
            ViewBag.Action = "Index";
            ViewBag.Title = "Nuevo Expediente Fiscal";

            ExpedienteFiscal expedienteFiscal = new ExpedienteFiscal()
            {
                Mes = (Meses)DateTime.Now.Month,
                Anio = DateTime.Now.Year,
                FechaCreacion = DateTime.Now
            };

            return View(expedienteFiscal);
        }

        // POST: Expedientes/Create
        [HttpPost]
        public async Task<ActionResult> Create(ExpedienteFiscal expediente)
        {
            var sucursalId = ObtenerSucursal();
            var grupoId = ObtenerGrupo();

            var socioComercialId = (int)Session["socComlId"];


            if (ModelState.IsValid)
            {

                //quiero hacer un try para checar si el registro ya existe, buscando mes, anio, sucursal y socio comercial
                var expedienteExistente = _db.ExpedientesFiscales
                    .FirstOrDefault(e => e.Mes == expediente.Mes &&
                                         e.Anio == expediente.Anio &&
                                         e.SucursalId == sucursalId &&
                                         e.SocioComercialId == socioComercialId);

                if (expedienteExistente != null)
                {
                    TempData["Errores"] = new List<string> { "Ya existe un expediente fiscal para el mes y año seleccionado" };
                    return RedirectToAction("Create", "ExpedientesFiscales", new { socioComercialId });

                }



                var basePath = $"ExpedientesFiscales/{grupoId}/{sucursalId}/{socioComercialId}/{expediente.Anio}/{(int)expediente.Mes}";

                if (expediente.ConstanciaSituacionFiscal != null && expediente.ConstanciaSituacionFiscal.ContentLength > 0)
                {
                    var nombreArchivo = "ConstanciaSituacionFiscal.pdf";
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(expediente.ConstanciaSituacionFiscal, key);

                    expediente.PathConstanciaSituacionFiscal = key;
                }

                if (expediente.OpinionCumplimientoSAT != null && expediente.OpinionCumplimientoSAT.ContentLength > 0)
                {
                    var nombreArchivo = "OpinionCumplimientoSAT.pdf";
                    var key = $"{basePath}/{nombreArchivo}";
                    await UploadFileToS3(expediente.OpinionCumplimientoSAT, key);

                    expediente.PathOpinionCumplimientoSAT = key;
                }

                expediente.SucursalId = sucursalId;
                expediente.UsuarioId = ObtenerUsuario();
                expediente.SocioComercialId = socioComercialId;

                // Guardar el resto de la información del expedienteFiscal en la base de datos
                _db.ExpedientesFiscales.Add(expediente);
                await _db.SaveChangesAsync();

                Session.Remove("socComlId");
                return RedirectToAction("Index", "ExpedientesFiscales", new { id = socioComercialId, socioComercialId = socioComercialId });
            }

            ViewBag.SocioComercialId = socioComercialId;
            return View(expediente);
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

        public ExpedientesFiscalesController()
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
