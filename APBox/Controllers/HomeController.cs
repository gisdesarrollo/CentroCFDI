using APBox.Context;
using API.Catalogos;
using API.Models.Control;
using Aplicacion.LogicaPrincipal.Control;
using System;
using System.Web.Mvc;

namespace APBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();

        public ActionResult Index()
        {
            var homeModel = new HomeModel();
            if (Request.IsAuthenticated)
            {
                var popularHome = new PopularHome((int)ObtenerSucursal(), (int)ObtenerUsuario(), (int)ObtenerSocioComercial(), (int)ObtenerDepartamento());
                popularHome.Popular(ref homeModel);
            }
            ViewBag.Controller = "Home";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Inicio";
            ViewBag.NameHere = "Home";

            return View(homeModel);
        }

        #region Popula Forma

        private int? ObtenerSucursal()
        {
            var sucursalId = Session["SucursalId"];
            return sucursalId != null ? Convert.ToInt32(sucursalId) : 0;
        }
        private int? ObtenerUsuario()
        {
            var usuarioId = Session["UsuarioId"];
            return usuarioId != null ? Convert.ToInt32(usuarioId) : 0;
        }
        private int? ObtenerSocioComercial()
        {
            var usuarioId = Session["UsuarioId"];
            if (usuarioId == null)
            {
                return 0;
            }

            // obtener el usuario de la base de datos
            var usuario = _db.Usuarios.Find(Convert.ToInt32(usuarioId));
            if (usuario == null)
            {
                return 0;
            }

            var socioComercialId = usuario.SocioComercialID;

            return socioComercialId != null ? Convert.ToInt32(socioComercialId) : 0;       
        }
        private int? ObtenerDepartamento()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
            {
                return null;
            }

            // obtener el usuario de la base de datos
            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null)
            {
                return null;
            }

            // Manejar la posibilidad de que el usuario no tenga departamento asignado
            if (usuario.Departamento_Id == null)
            {
                return 0;
            }

            var departamentoId = usuario.Departamento_Id;
            return departamentoId != null ? Convert.ToInt32(departamentoId) : 0;
        }

        #endregion

    }
}