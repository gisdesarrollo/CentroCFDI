using API.Models.Control;
using Aplicacion.LogicaPrincipal.Control;
using System;
using System.Web.Mvc;

namespace APBox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var homeModel = new HomeModel();
            if (Request.IsAuthenticated)
            {
                var popularHome = new PopularHome(ObtenerSucursal());
                popularHome.Popular(ref homeModel);
            }
            return View(homeModel);
        }

        #region Popula Forma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion

    }
}