using APBox.Context;
using API.Models.DocumentosRecibidos;
using Aplicacion.LogicaPrincipal.Correos;
using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using Aplicacion.LogicaPrincipal.Facturas;
using System;
using System.Web.Mvc;

namespace APBox.Controllers.Reportes
{
    public class ReporteRecibidosController : Controller
    {
        #region variables
        private readonly APBoxContext _db = new APBoxContext();
        private readonly ProcesaDocumentoRecibido _procesaDocumentoRecibido = new ProcesaDocumentoRecibido();
        private readonly Decodificar _decodifica = new Decodificar();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();
        private readonly OperacionesDocumentosRecibidos _operacionesDocumentosRecibidos = new OperacionesDocumentosRecibidos();
        #endregion
        
        // GET: ReporteCfdiRecibidos
        public ActionResult ReporteCfdiRecibidos()
        {
            var sucursalId = ObtenerSucursal();
            var documentosRecibidosModel = new DocumentosRecibidosModel
            {
                FechaInicial = DateTime.Now.AddDays(-5), // SE RESTA 6 DIAS PARA MOSTRAR EL RANGO DE FACTURAS GENERADAS EN UN SEMANA
                FechaFinal = DateTime.Now,
                SucursalId = ObtenerSucursal(),
            };
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, documentosRecibidosModel.FechaInicial.Day, 0, 0, 0);
            var fechaFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;
            _operacionesDocumentosRecibidos.ObtenerFacturas(ref documentosRecibidosModel, sucursalId);

            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "ReporteDocumetosRecibidos";
            ViewBag.ActionES = "Reporte CFDI Recibidos";
            ViewBag.NameHere = "Reporte CFDI Recibidos";

            return View(documentosRecibidosModel);
        }

        // POST: ReporteCfdiRecibidos
        [HttpPost]
        public ActionResult ReporteCfdiRecibidos(DocumentosRecibidosModel documentosRecibidosModel)
        {
            var sucursalId = ObtenerSucursal();
            var fechaI = documentosRecibidosModel.FechaInicial;
            var fechaF = documentosRecibidosModel.FechaFinal;
            var fechaInicial = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 0, 0, 0);
            var fechaFinal = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 59);
            documentosRecibidosModel.FechaInicial = fechaInicial;
            documentosRecibidosModel.FechaFinal = fechaFinal;
            _operacionesDocumentosRecibidos.ObtenerFacturas(ref documentosRecibidosModel, sucursalId);

            ViewBag.Controller = "DocumentosRecibidos";
            ViewBag.Action = "ReporteDocumetosRecibidos";
            ViewBag.ActionES = "Reporte CFDI Recibidos";
            ViewBag.NameHere = "Reporte CFDI Recibidos";

            return View(documentosRecibidosModel);
        }

        #region funciones
        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }
        private int ObtenerUsuario()
        {
            return Convert.ToInt32(Session["UsuarioId"]);
        }
        #endregion
    }
}