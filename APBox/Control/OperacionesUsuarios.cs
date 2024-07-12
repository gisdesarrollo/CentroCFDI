using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using Aplicacion.Context;
using APBox.Models;
using ApplicationUser = APBox.Models.ApplicationUser;

namespace APBox.Control
{
    public class OperacionesUsuarios
    {

        #region Variables

        private const string password = "A12345";
        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void Crear(String nombreUsuario)
        {
             var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            
            // Establecer la configuración para permitir nombres de usuario no alfanuméricos
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
            var user = new ApplicationUser() { UserName = nombreUsuario };

            try
            {
                var result  = userManager.Create(user, password);
                if (!result.Succeeded)
                {
                    throw new Exception(String.Format("No se pudo crear el usuario: {0}", string.Join(", ", result.Errors)));
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se pudo crear el usuario: {0}", ex.Message));
            }
        }

        /*public void EliminarUsuario(string clave)
        {
            //TODO: Arreglar Esto
            Membership.Provider.DeleteUser(clave, true);
        }*/

        public void Reseteo(string nombreUsuario)
        {
            _db.Database.ExecuteSqlCommand(String.Format("UPDATE aspnetusers SET PasswordHash = 'AHYKBw50VZUCbIizHV3RvvDXFzD1Pqu87mToM/uIvcTSYMQR8nf1PykV0FBQA+t3ZA==' WHERE UserName = '{0}'", nombreUsuario));
        }
        public void ReseteoUsername(string nombreUsuarioAnterior,string nombreUsuarioNuevo)
        {
            _db.Database.ExecuteSqlCommand(String.Format("UPDATE aspnetusers SET UserName = '{0}' WHERE UserName = '{1}'", nombreUsuarioNuevo,nombreUsuarioAnterior));
        }
        public void EliminarUsuario(string nombreUsuario)
        {
            _db.Database.ExecuteSqlCommand(String.Format("DELETE FROM aspnetusers WHERE UserName = '{0}'", nombreUsuario));
        }
    }
}