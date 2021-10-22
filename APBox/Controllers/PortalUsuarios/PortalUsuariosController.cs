using API.Models.Reportes;
using System;
using System.Web.Mvc;

namespace APBox.Controllers.PortalUsuarios
{
    [APBox.Control.SessionExpire]
    public class PortalUsuariosController : Controller
    {
        // GET: PortalUsuarios
        public ActionResult EstadosCuenta()
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