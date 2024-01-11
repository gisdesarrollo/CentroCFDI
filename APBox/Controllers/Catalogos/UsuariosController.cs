using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using API.Catalogos;
using APBox.Context;
using System;
using System.Collections.Generic;
using API.Relaciones;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using APBox.Control;
using System.Net.Mail;

namespace APBox.Controllers.Catalogos
{
    [SessionExpire]
    //[Authorize(Roles = "USUARIOS")]
    public class UsuariosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();
        private readonly AcondicionarUsuarios _acondicionarUsuarios = new AcondicionarUsuarios();

        #endregion

        // GET: Usuarios
        public ActionResult Index()
        {
            var grupoId = ObtenerGrupo();
            var usuarios = _db.Usuarios.Where(u => u.GrupoId == grupoId).ToList();
            
            ViewBag.Controller = "Usuarios";
            ViewBag.Action = "Index";
            ViewBag.ActionES = "Index";
            ViewBag.Button = "Crear";
            ViewBag.NameHere = "sistema";

            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            PopulaForma(usuario.PerfilId);
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            PopulaForma();
            PopulaClientes();
            PopulaDepartamento();
            var usuario = new Usuario
            {
                Status = API.Enums.Status.Activo,
                Sucursales = new List<UsuarioSucursal>(),
                GrupoId = ObtenerGrupo()
            };
            
            ViewBag.Controller = "Usuarios";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "sistema";

            return View(usuario);
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario, bool esProveedor)
        {
            PopulaClientes(usuario.SocioComercialID);
            PopulaForma(usuario.PerfilId);
            PopulaDepartamento(usuario.Departamento_Id);
            if (ModelState.IsValid)
            {
                var entidadExistente = _db.Usuarios.FirstOrDefault(e =>  e.NombreUsuario == usuario.NombreUsuario);
                if (entidadExistente != null)
                {
                   ViewBag.ErrorMessage = "El usuario ya existe";
                   return View(usuario);
                }

                _acondicionarUsuarios.CargaInicial(ref usuario);

                try
                {
                    _operacionesUsuarios.Crear(usuario.NombreUsuario);
                   
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(usuario);
                }

                if (usuario.esProveedor == true)
                {
                    //Asignacion de valor si es Proveedor
                    usuario.esProveedor = esProveedor;
                    usuario.PerfilId = 32;
                    // Envío de correo electrónico de bienvenida
                    //EnviarCorreoBienvenida(usuario);
                }
                // Envío de correo electrónico de bienvenida
                EnviarCorreoBienvenida(usuario);
                _db.Usuarios.Add(usuario);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(usuario);
        }

        
        /*
        * Kevin Enrique 
        * Descripción: Implementación de envio Email al asignar usuarios(
        */
        private void EnviarCorreoBienvenida(Usuario usuario)
        {
            
            string destinatario = usuario.Email;
            string asunto = "Bienvenido a CentroCFDi";
            string cuerpoCorreo = null;

            if (usuario.esProveedor == false) { 
            cuerpoCorreo = $"¡Hola {usuario.Nombre}!\n\n" +
                "Te damos la bienvenida a nuestra plataforma. Estamos emocionados de tenerte con nosotros.\n\n" +
                "Detalles de tu cuenta:\n" +
                $"- Usuario: {usuario.NombreUsuario}\n" +
                "- Contraseña: A12345 \n\n" +
                "Por favor, accede a tu cuenta con el siguiente enlace: http://www.centrocfdi.com\n\n" +
              
                "Si tienes alguna pregunta o necesitas asistencia, no dudes en ponerte en contacto con nuestro equipo de soporte en soporte@gisconsultoria.com\n\n" +
                "Como miembro de nuestro equipo, tendrás acceso a recursos y herramientas que facilitarán tu trabajo diario. Estamos comprometidos a proporcionar un entorno en el que puedas crecer y tener éxito. Si tienes alguna pregunta sobre cómo aprovechar al máximo nuestra plataforma como trabajador, no dudes en ponerte en contacto con nuestro equipo de recursos humanos.\n\n" +
                "¡Bienvenido a nuestro equipo!\n\n" +
                "Saludos cordiales,\n" +
                "Equipo de [GIS Consultoria]\n" +
                "Centro CFDI.";
            }
            else
            {
                 cuerpoCorreo = $"¡Hola {usuario.Nombre}!\n\n" +
                "Te damos la bienvenida a nuestra plataforma. Estamos emocionados de tenerte con nosotros.\n\n" +
                "Detalles de tu cuenta:\n" +
                $"- Usuario: {usuario.NombreUsuario}\n" +
                "- Contraseña: A12345 \n\n" +
                "Por favor, accede a tu cuenta con el siguiente enlace: http://www.centrocfdi.com\n\n" +

                "Si tienes alguna pregunta o necesitas asistencia, no dudes en ponerte en contacto con nuestro equipo de soporte en soporte@gisconsultoria.com\n\n" +
                "Como proveedor, tu participación es fundamental para el éxito de nuestra plataforma. Esperamos que encuentres todas las herramientas necesarias para gestionar tus servicios de manera efectiva. Si tienes alguna pregunta específica sobre cómo utilizar la plataforma como proveedor, no dudes en ponerte en contacto con nuestro equipo de soporte dedicado a proveedores.\n\n" +
                "¡Gracias por unirte a nuestra red!\n\n" +
                "Saludos cordiales,\n" +
                "Equipo de [GIS Consultoria]\n" +
                "Centro CFDI.";

            }

            using (SmtpClient smtpClient = new SmtpClient("mail.gisconsultoria.com", 26))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("facturas.xsa@gisconsultoria.com", "Gisconsul+01");
                smtpClient.EnableSsl = false;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("facturas.xsa@gisconsultoria.com");
                mail.To.Add(destinatario);
                mail.Subject = asunto;
                mail.Body = cuerpoCorreo;

                smtpClient.Send(mail);
            }
        }



        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            PopulaClientes();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            PopulaForma(usuario.PerfilId);
            ViewBag.Controller = "Usuarios";

            ViewBag.Action = "Edit";
            ViewBag.ActionES = "Editar";
            ViewBag.NameHere = "sistema";
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario, bool esProveedor)
        {
            PopulaClientes(usuario.SocioComercialID);
            PopulaForma(usuario.PerfilId);


            var proveedorExistente = _db.Usuarios.FirstOrDefault(e => e.esProveedor == usuario.esProveedor &&  e.Id != usuario.Id);
            if (ModelState.IsValid)
            {
                var entidadExistente = _db.Usuarios.FirstOrDefault(e => e.Nombre == usuario.Nombre && e.ApellidoPaterno == usuario.ApellidoPaterno && e.ApellidoMaterno == usuario.ApellidoMaterno && e.Id != usuario.Id);
                if (entidadExistente != null)
                {
                    ModelState.AddModelError("", "Ese usuario ya existe");
                    return View(usuario);
                }

                if(usuario.esProveedor) { 
                //Asignacion de valor si es Proveedor
                usuario.esProveedor = esProveedor;
                usuario.PerfilId = 32;
                }
                _acondicionarUsuarios.Sucursales(usuario);
                _db.Entry(usuario).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            PopulaForma(usuario.PerfilId);
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = _db.Usuarios.Find(id);
            _db.Usuarios.Remove(usuario);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Reseteo(int id)
        {
            var personal = _db.Usuarios.Find(id);
            _operacionesUsuarios.Reseteo(personal.NombreUsuario);
            return RedirectToAction("Index");
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

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }
        
        private void PopulaForma(int? perfilId = null, int? grupoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), false);
            
            ViewBag.PerfilId = popularDropDowns.PopulaPerfiles(perfilId);
            ViewBag.GrupoId = popularDropDowns.PopulaGrupos(grupoId);

            ViewBag.SucursalId = popularDropDowns.PopulaSucursalesUsuarios(null);
        }

        //DropDown Socios Comerciales
        private void PopulaClientes(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.SocioComercialID = popularDropDowns.PopulaClientes(receptorId);
        }

        //DropDown Departamentos
        private void PopulaDepartamento(int? DepartamentoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.Departamento_Id = popularDropDowns.PopulaDepartamentos(DepartamentoId);
        }


        #endregion
    }
}
