using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using APBox.Context;
using API.Catalogos;
using System;
using APBox.Control;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using Microsoft.AspNet.Identity;
using Org.BouncyCastle.Crypto.Tls;

using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using SW.Services.Authentication;

namespace APBox.Controllers.Catalogos
{
    [APBox.Control.SessionExpire]
    public class SociosComercialesController : Controller
    {
        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly AcondicionarClientes _acondicionarClientes = new AcondicionarClientes();
        private readonly ProcesaDocumentoRecibido _procesaValidacion = new ProcesaDocumentoRecibido();

        #endregion Variables

        public ActionResult Index()
        {
            PopulaSocioComercial();
            var sucursalId = ObtenerSucursal();
            var clientes = _db.SociosComerciales.Where(c => c.SucursalId == sucursalId).ToList();

            ViewBag.Controller = "SociosComerciales";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.NameHere = "Crea y modifica Socios Comerciales";

            return View(clientes);
        }

        public ActionResult Create()
        {
            PopulaForma();

            var sociocomercial = new SocioComercial
            {
                Status = API.Enums.Status.Activo,
                FechaAlta = DateTime.Now,
                Pais = (API.Enums.c_Pais)c_Pais.MEX,
                SucursalId = ObtenerSucursal(),
                GrupoId = ObtenerGrupo()
            };

            ViewBag.Controller = "SociosComerciales";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "catalogo";

            return View(sociocomercial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SocioComercial sociocomercial)
        {
            ModelState.Remove("Banco.Id");
            ModelState.Remove("Banco.Nombre");
            ModelState.Remove("Banco.NumeroCuenta");
            PopulaForma();
            if (ModelState.IsValid)
            {
                _acondicionarClientes.CargaInicial(ref sociocomercial);
                var receptor = _db.SociosComerciales.Where(c => c.Rfc == sociocomercial.Rfc && c.RazonSocial == sociocomercial.RazonSocial && c.SucursalId == sociocomercial.SucursalId).FirstOrDefault();
                if (receptor != null)
                {
                    ModelState.AddModelError("", "Error RFC o Razon Social Ya Se Encuentra Registrado!!");
                    return View(sociocomercial);
                }
                _db.SociosComerciales.Add(sociocomercial);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.errores = errores;
              
            }

            return View(sociocomercial);
        }

        public ActionResult Edit(int? id)
        {
            var usuario = _db.Usuarios.Find(ObtenerUsuario());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocioComercial sociocomercial = _db.SociosComerciales.Find(id);
            if (sociocomercial == null)
            {
                return HttpNotFound();
            }

            if (usuario.esProveedor && usuario.SocioComercialID != id)
            {
            // Si no coincide, redirigir al usuario a una página de error o denegar el acceso
            // Puedes mostrar un mensaje de error u otra información relevante al usuario
            // Si no coincide, mostrar una alerta al usuario antes de redirigir
            TempData["Mensaje"] = "No tienes permiso para acceder a esta página.";
            // Redirigir al usuario a la página de inicio (Home)
            return RedirectToAction("Index", "Home");
            }

            PopulaForma();
            ViewBag.Controller = "SociosComerciales";
            ViewBag.Action = "Editar";
            ViewBag.NameHere = "Editar Expediente Socio Comercial";
            return View(sociocomercial);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SocioComercial socioComercialEdit)
        {
            ModelState.Remove("Banco.Id");
            ModelState.Remove("Banco.Nombre");
            ModelState.Remove("Banco.NumeroCuenta");

            try
            {
                //var socioComercial = _db.SociosComerciales.Find(socioComercialEdit.Id);
                //socioComercial = socioComercialEdit;
                socioComercialEdit.GrupoId = ObtenerGrupo();
                socioComercialEdit.SucursalId = ObtenerSucursal();

                _db.Entry(socioComercialEdit).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                if (ModelState.IsValid)
                {
                    // Guardar el modelo en la base de datos
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in ModelState.Values)
                    {
                        foreach (var modelError in error.Errors)
                        {
                            // Mostrar el mensaje de error al usuario
                            ModelState.AddModelError("", modelError.ErrorMessage);
                        }
                    }

                    return View(socioComercialEdit);
                }
            }
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocioComercial cliente = _db.SociosComerciales.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            PopulaForma();
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SocioComercial cliente = _db.SociosComerciales.Find(id);
            _db.SociosComerciales.Remove(cliente);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ValidaRfc(string idSocioComercial)
        {
            AuthResponse responseAutenticacion = new AuthResponse();
            var socioComercial = _db.SociosComerciales.Find(idSocioComercial);
            responseAutenticacion = _procesaValidacion.GetToken();
            _procesaValidacion.ValidaRFC(responseAutenticacion.data.token,socioComercial.Rfc);
            return Content("¡Formulario enviado correctamente!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region PopulaForma
        private void PopulaSocioComercial(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.ReceptorId = popularDropDowns.PopulaClientes(receptorId);
        }
        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private void PopulaForma()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), true);

            ViewBag.BancoId = popularDropDowns.PopulaBancos(null);
        }

        #endregion PopulaForma

        private int ObtenerUsuario()
        {
            return Convert.ToInt32(Session["UsuarioId"]);
        }
    }
}