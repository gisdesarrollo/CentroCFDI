using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using APBox.Control;
using System.Linq;
using API.Enums;
using APBox.Context;
using APBox.Models;

namespace APBox.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel();

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    TiposUsuarios tipoUsuario;
                    try
                    {
                        tipoUsuario = TipoUsuario(model.UserName);
                        Session["TipoUsuario"] = tipoUsuario;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        return View(model);
                    }

                    var usuarioId = AddRoles(user.Id, model.UserName, tipoUsuario);
                    await SignInAsync(user, false);

                    Session["UsuarioId"] = usuarioId;

                    try
                    {
                        var grupoId = SeleccionarGrupo(tipoUsuario, usuarioId);
                        Session["GrupoId"] = grupoId;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        return View(model);
                    }

                    return RedirectToAction("SeleccionarSucursal");

                }
                else
                {
                    ModelState.AddModelError("", "Login Incorrecto");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult SeleccionarSucursal()
        {
            var tipoUsuario = (TiposUsuarios)Session["TipoUsuario"];

            if(tipoUsuario == TiposUsuarios.Usuario)
            {
                var usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                var usuario = _db.Usuarios.Find(usuarioId);

                var loginSucursal = new LoginSucursal
                {
                    UsuarioId = usuarioId,
                    GrupoId = usuario.GrupoId
                };

                var sucursales = _db.Sucursales.Where(s => s.GrupoId == usuario.GrupoId).ToList();
                if (sucursales.Count == 1)
                {
                    Session["SucursalId"] = sucursales.First().Id;
                    Session["GrupoId"] = sucursales.First().GrupoId;
                    return RedirectToAction("Index", "Home");
                }

                var popularDropDowns = new PopularDropDowns(loginSucursal.GrupoId, true);
                ViewBag.SucursalId = popularDropDowns.PopulaSucursalesUsuarios(null, usuarioId);
                return View(loginSucursal);
            }
            else
            {
                var proveedorId = Convert.ToInt32(Session["UsuarioId"]);
                var proveedor = _db.Proveedores.Find(proveedorId);

                var loginSucursal = new LoginSucursal
                {
                    ProveedorId = proveedorId,
                    GrupoId = proveedor.GrupoId
                };

                var sucursales = _db.Sucursales.Where(s => s.GrupoId == proveedor.GrupoId).ToList();
                if (sucursales.Count == 1)
                {
                    Session["SucursalId"] = sucursales.First().Id;
                    Session["GrupoId"] = sucursales.First().GrupoId;
                    return RedirectToAction("Index", "Home");
                }

                var popularDropDowns = new PopularDropDowns(loginSucursal.GrupoId, true);
                ViewBag.SucursalId = popularDropDowns.PopulaSucursalesProveedores(null, proveedorId);
                return View(loginSucursal);
            }
        }

        [HttpPost]
        public ActionResult SeleccionarSucursal(LoginSucursal loginSucursal)
        {
            var tipoUsuario = (TiposUsuarios)Session["TipoUsuario"];

            var popularDropDowns = new PopularDropDowns(loginSucursal.GrupoId, true);

            if(tipoUsuario == TiposUsuarios.Usuario)
            {
                ViewBag.SucursalId = popularDropDowns.PopulaSucursalesUsuarios(loginSucursal.GrupoId);


                var usuario = _db.Usuarios.Find(loginSucursal.UsuarioId);

                if (!usuario.TodasSucursales)
                {
                    var sucursalesLigadas = _db.UsuariosSucursales.FirstOrDefault(us => us.SucursalId == loginSucursal.SucursalId && us.UsuarioId == usuario.Id);
                    if (sucursalesLigadas == null)
                    {
                        ModelState.AddModelError("", "Usuario no ligado a esa sucursal");
                        return View(loginSucursal);
                    }
                }

                Session["GrupoId"] = usuario.GrupoId;
                Session["SucursalId"] = loginSucursal.SucursalId;

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.SucursalId = popularDropDowns.PopulaSucursalesProveedores(loginSucursal.GrupoId);
                var proveedor = _db.Proveedores.Find(loginSucursal.ProveedorId);

                var sucursalesLigadas = _db.ProveedoresSucursales.FirstOrDefault(us => us.SucursalId == loginSucursal.SucursalId && us.ProveedorId == proveedor.Id);
                if (sucursalesLigadas == null)
                {
                    ModelState.AddModelError("", "Proveedor no ligado a esa sucursal");
                    return View(loginSucursal);
                }

                Session["GrupoId"] = proveedor.GrupoId;
                Session["SucursalId"] = loginSucursal.SucursalId;

                return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Su password ha sido cambiado."
                : message == ManageMessageId.SetPasswordSuccess ? "Su password ha sido establecido."
                : message == ManageMessageId.RemoveLoginSuccess ? "Su entrada externa ha sido removida."
                : message == ManageMessageId.Error ? "Ocurrió un error."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #region Funciones Identidad

        private int SeleccionarGrupo(TiposUsuarios tipoUsuario, int usuarioId)
        {
            switch (tipoUsuario)
            {
                case TiposUsuarios.Usuario:
                    return _db.Usuarios.Find(usuarioId).GrupoId;
                case TiposUsuarios.Proveedor:
                    return _db.Proveedores.Find(usuarioId).GrupoId;
                default:
                    throw new Exception(String.Format("Usuario no encontrado: {0} - {1}", tipoUsuario, usuarioId));
            }
        }

        public TiposUsuarios TipoUsuario(string nombreUsuario)
        {
            var usuario = _db.Usuarios.FirstOrDefault(a => a.NombreUsuario == nombreUsuario);
            if (usuario != null)
            {
                return TiposUsuarios.Usuario;
            }

            var proveedor = _db.Proveedores.FirstOrDefault(u => u.Rfc == nombreUsuario);
            if (proveedor != null)
            {
                return TiposUsuarios.Proveedor;
            }

            throw new Exception("Usuario no encontrado");
        }

        private int AddRoles(string userId, string nombreUsuario, TiposUsuarios tipoUsuario)
        {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ClearUserRoles(um, userId);

            if (tipoUsuario == TiposUsuarios.Proveedor)
            {
                var proveedor = _db.Proveedores.First(u => u.Rfc == nombreUsuario && u.Status == Status.Activo);
                AddRole(um, userId, true, "PORTALPROVEEDORES");
                return proveedor.Id;
            }

            var usuario = _db.Usuarios.First(u => u.NombreUsuario == nombreUsuario && u.Status == Status.Activo);
            var perfil = usuario.Perfil;
            
            //Catalogos
            AddRole(um, userId, perfil.Grupos, "GRUPOS");
            AddRole(um, userId, perfil.Sucursales, "SUCURSALES");
            AddRole(um, userId, perfil.Departamentos, "DEPARTAMENTOS");
            AddRole(um, userId, perfil.CentrosCostos, "CENTROSCOSTOS");
            AddRole(um, userId, perfil.Perfiles, "PERFILES");
            AddRole(um, userId, perfil.Usuarios, "USUARIOS");
            AddRole(um, userId, perfil.Bancos, "BANCOS");
            AddRole(um, userId, perfil.Proveedores, "PROVEEDORES");
            AddRole(um, userId, perfil.Clientes, "CLIENTES");

            //Operaciones Catalogos
            AddRole(um, userId, perfil.Consulta, "CONSULTA");
            AddRole(um, userId, perfil.Insercion, "INSERCION");
            AddRole(um, userId, perfil.Edicion, "EDICION");
            AddRole(um, userId, perfil.Borrado, "BORRADO");

            //Recepción            
            AddRole(um, userId, perfil.ValidacionRapida, "VALIDACIONRAPIDA");
            AddRole(um, userId, perfil.SolicitudesAcceso, "SOLICITUDESACCESO");

            //Complementos de recepción de pagos
            AddRole(um, userId, perfil.FacturasEmitidas, "FACTURASEMITIDAS");
            AddRole(um, userId, perfil.GeneracionManual, "GENERACIONMANUAL");
            AddRole(um, userId, perfil.GeneracionLayout, "GENERACIONLAYOUT");

            //Portal de Usuarios
            AddRole(um, userId, perfil.PortalUsuarios, "PORTALUSUARIOS");

            //Reportes
            AddRole(um, userId, perfil.ReporteDocumentos, "REPORTEDOCUMENTOS");
            AddRole(um, userId, perfil.ReporteEstadisticasPorUsuario, "REPORTEESTADISTICASPORUSUARIO");
            AddRole(um, userId, perfil.ReporteEstadisticasPorProveedor, "REPORTEESTADISTICASPORPROVEEDOR");
            AddRole(um, userId, perfil.ReporteEstadisticasComplementosPago, "REPORTECOMPLEMENTOSPAGO");

            return usuario.Id;
        }

        public void ClearUserRoles(UserManager<ApplicationUser> um, string userId)
        {
            var user = UserManager.FindByIdAsync(userId);
            UserManager.RemoveFromRolesAsync(userId, UserManager.GetRoles(userId).ToArray());
        }

        public void AddRole(UserManager<ApplicationUser> um, String userId, bool propiedadPerfil, String rol)
        {
            if (propiedadPerfil)
            {
                um.AddToRole(userId, rol);
            }
        }

        #endregion

    }
}