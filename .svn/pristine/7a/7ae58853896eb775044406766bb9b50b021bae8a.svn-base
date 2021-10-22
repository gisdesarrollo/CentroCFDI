using API.Models.Reportes;
using System;
using System.Web.Mvc;

namespace APBox.Controllers.Reportes
{
    [APBox.Control.SessionExpire]
    public class ReportesController : Controller
    {
        public ActionResult EstadisticasUsuario()
        {
            var horasModel = new HorasModel
            {
                FechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                FechaFinal = DateTime.Now,
                HoraInicial = "00:00",
                HoraFinal = "23:59"
            };

            return View(horasModel);
        }

        public ActionResult EstadisticasCliente()
        {
            var horasModel = new HorasModel
            {
                FechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                FechaFinal = DateTime.Now,
                HoraInicial = "00:00",
                HoraFinal = "23:59"
            };

            return View(horasModel);
        }

        public ActionResult EstadisticasProveedor()
        {
            var horasModel = new HorasModel
            {
                FechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                FechaFinal = DateTime.Now,
                HoraInicial = "00:00",
                HoraFinal = "23:59"
            };

            return View(horasModel);
        }
    }
}