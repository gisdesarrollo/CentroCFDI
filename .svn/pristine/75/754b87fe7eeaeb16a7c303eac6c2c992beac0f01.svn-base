using APBox.Context;
using APBox.Control;
using API.Enums;
using API.Operaciones.OperacionesProveedores;
using Aplicacion.LogicaPrincipal.SolicitudesAccesos;
using System;
using System.Linq;
using System.Web.Mvc;

namespace APBox.Controllers.Operaciones
{
    public class SolicitudesAccesosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperarSolicitudes _operarSolicitudes = new OperarSolicitudes();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();

        #endregion

        public ActionResult Solicitar()
        {
            var solicitudAcceso = new SolicitudAcceso
            {
                FechaAlta = DateTime.Now,
                Pais = "MEXICO",
                Status = Status.Activo
            };

            return View(solicitudAcceso);
        }

        [HttpPost]
        public ActionResult Solicitar(SolicitudAcceso solicitudAcceso)
        {
            ModelState.Remove("GrupoId");

            if (ModelState.IsValid)
            {
                var grupoBuscar = _db.Grupos.FirstOrDefault(g => g.Llave == solicitudAcceso.LlaveGrupo);
                if(grupoBuscar == null)
                {
                    ModelState.AddModelError("LlaveGrupo", "Esta llave de grupo no es válida");
                    return View(solicitudAcceso);
                }

                solicitudAcceso.GrupoId = grupoBuscar.Id;
                _db.SolicitudesAccesos.Add(solicitudAcceso);
                _db.SaveChanges();

                if (grupoBuscar.AutorizacionAutomaticaSolicitudes)
                {
                    _operarSolicitudes.Autorizar(solicitudAcceso.Id);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(solicitudAcceso);
        }

        [APBox.Control.SessionExpire]
        public ActionResult Index()
        {
            var grupoId = ObtenerGrupo();
            var solicitudes = _db.SolicitudesAccesos.Where(sa => !sa.Procesado && sa.GrupoId == grupoId && sa.Status == Status.Activo).ToList();
            return View(solicitudes);
        }

        [APBox.Control.SessionExpire]
        public ActionResult Operar(int id, bool autorizar)
        {
            var solicitudAcceso = _db.SolicitudesAccesos.Find(id);
            solicitudAcceso.Autorizar = autorizar;
            return View(solicitudAcceso);
        }
        
        [HttpPost]
        [APBox.Control.SessionExpire]
        public ActionResult Operar(SolicitudAcceso solicitudAcceso)
        {
            if (ModelState.IsValid)
            {
                if (solicitudAcceso.Autorizar)
                {
                    var usuario = _operarSolicitudes.Autorizar(solicitudAcceso.Id);
                    _operacionesUsuarios.Crear(usuario);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _operarSolicitudes.Rechazar(solicitudAcceso.Id);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(solicitudAcceso);
        }

        #region Popula Forma

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        #endregion
    }
}