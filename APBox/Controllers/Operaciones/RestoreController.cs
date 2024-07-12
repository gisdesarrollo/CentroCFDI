using APBox.Models;
using API.Models.Dto;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using APBox.Context;
using Microsoft.AspNet.Identity.Owin;

namespace APBox.Controllers.Operaciones
{
    public class RestoreController : Controller
    {
        private readonly APBoxContext _db = new APBoxContext();
        private readonly string UserNameEmail = "facturas.xsa@gisconsultoria.com";
        private readonly string passwordEmail = "Gisconsul+01";
        private readonly string ServerEmail = "mail.gisconsultoria.com";
        private readonly int PortEmail = 26;

        // GET: Restore
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            DataRestore dataRestore = new DataRestore();

            return View(dataRestore);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(DataRestore dataRestore)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("Token");
            ModelState.Remove("Password");
            
            if (ModelState.IsValid)
            {
                var usuario = _db.Usuarios.Where(u=> u.Email == dataRestore.Email && u.NombreUsuario == dataRestore.Username).FirstOrDefault();
                if(usuario == null)
                {
                    ViewBag.ErrorMessage = "Error: El email no esta asociada a una cuenta";
                    return View(dataRestore);
                }
                //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = userManager.FindByName(dataRestore.Username);
                if(user == null)
                {
                    ViewBag.ErrorMessage = "Error: El Usuario no existe";
                    return View(dataRestore);
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Restore", new { userId = user.Id, token = token }, protocol: Request.Url.Scheme);
                try
                {
                    await SendPasswordResetEmailAsync(dataRestore.Email, callbackUrl);
                }catch (Exception)
                {
                    string error = $"No se puedo enviar la confirmacion al correo electronico {dataRestore.Email}";
                    return RedirectToAction("Error", "Restore", new { errorMessage = error });
                   
                }
            }
            else
            {
                return View(dataRestore);
            }
            string completado = $"se envio la confirmacion al correo electronico {dataRestore.Email} , recibirás un correo con las instrucciones para restablecer tu contraseña.";
            return RedirectToAction("Completado", "Restore", new { completedMessage = completado });
        }
        private async Task SendPasswordResetEmailAsync(string email, string callbackUrl)
        {
            try
            {
                using (var smtpClient = new SmtpClient(ServerEmail, PortEmail))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(UserNameEmail, passwordEmail);
                    smtpClient.EnableSsl = false;

                    var mailMessage = new MailMessage
                    {
                        Subject = "Restablecer su contraseña",
                        Body = $"Restablezca su contraseña haciendo clic <a href='{callbackUrl}'>aquí</a>",
                        IsBodyHtml = true,
                        From = new MailAddress("soporte@centrocfdi.com")
                    };
                    mailMessage.To.Add(email);
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }catch(Exception ex)
            {
                throw new Exception(String.Format($"Error: {ex.Message}"));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string userId, string token)
        {
            DataRestore dataRestore = new DataRestore();
            dataRestore.UserId = userId;
            dataRestore.Token = token;
            if(userId == null || token == null)
            {
                string error = "link de acceso contiene errores.";
                return RedirectToAction("Error", "Restore", new { errorMessage = error });
            }
             return View(dataRestore);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(DataRestore dataRestore)
        {
            ModelState.Remove("Email");
            ModelState.Remove("Username");
            
            if (!ModelState.IsValid)
            {
                return View(dataRestore);
            }
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            try
            {
                var result = await userManager.ResetPasswordAsync(dataRestore.UserId, dataRestore.Token, dataRestore.Password);

                if (result.Succeeded)
                {
                    string success = "Se ha actualizado correctamente la contraseña, intenta de nuevo ingresar ala plataforma.";
                    return RedirectToAction("Completado", "Restore", new { CompletedMessage = success });
                }
                AddErrors(result);
                return View(dataRestore);
            }catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ha ocurrido un error: {ex.Message}";
                return View(dataRestore);
            }
        }

        [AllowAnonymous]
        public ActionResult Completado(string completedMessage)
        {
            ViewBag.Controller = "Restore";
            ViewBag.Title = "Completado";
            ViewBag.Completed = completedMessage;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error(string errorMessage)
        {
            ViewBag.Controller = "Restore";
            ViewBag.Title = "Error";
            ViewBag.Error = errorMessage;

            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}
