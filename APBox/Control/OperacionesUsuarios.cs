using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Web.Security;
using Aplicacion.Context;
using APBox.Models;

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

            var user = new ApplicationUser() { UserName = nombreUsuario };

            try
            {
                userManager.Create(user, password);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se pudo crear el usuario: {0}", ex.Message));
            }
        }

        public void EliminarUsuario(string clave)
        {
            //TODO: Arreglar Esto
            Membership.Provider.DeleteUser(clave, true);
        }

        public void Reseteo(string nombreUsuario)
        {
            _db.Database.ExecuteSqlCommand(String.Format("UPDATE AspNetUsers SET PasswordHash = 'AHYKBw50VZUCbIizHV3RvvDXFzD1Pqu87mToM/uIvcTSYMQR8nf1PykV0FBQA+t3ZA==' WHERE UserName = '{0}'", nombreUsuario));
        }
    }
}