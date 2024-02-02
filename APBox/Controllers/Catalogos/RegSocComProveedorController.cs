using APBox.Context;
using APBox.Control;
using API.Catalogos;
using API.Relaciones;
using Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos;
using Aplicacion.LogicaPrincipal.Correos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APBox.Controllers.Catalogos
{
    public class RegSocComProveedorController : Controller
    {
        #region Variables

        private readonly APBoxContext _db = new APBoxContext();
        private readonly OperacionesUsuarios _operacionesUsuarios = new OperacionesUsuarios();
        private readonly AcondicionarUsuarios _acondicionarUsuarios = new AcondicionarUsuarios();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();


        //Seccion Cliente
        private readonly AcondicionarClientes _acondicionarClientes = new AcondicionarClientes();

        // GET: Clientes/Create
        [AllowAnonymous] // Permitir acceso sin autenticación
        public ActionResult Create(Guid id)
        {
            PopulaForma();

            var Grupos = _db.Grupos.FirstOrDefault(c => c.Llave == id);

            TempData["grupoId"] = Grupos.Id;


            var sucursal = _db.Sucursales.FirstOrDefault(e => e.GrupoId == Grupos.Id);

            if (Grupos == null)
            {
                return HttpNotFound("No se encontró ningún grupo con el UUID proporcionado.");
            }

            var cliente = new Cliente
            {
                Status = API.Enums.Status.Activo,
                FechaAlta = DateTime.Now,
                Pais = (API.Enums.c_Pais)c_Pais.MEX,
                SucursalId = sucursal.Id,

            };


            ViewBag.Controller = "RegistrarSocioProveedor";
            ViewBag.Action = "Create";
            ViewBag.ActionES = "Crear";
            ViewBag.NameHere = "catalogo";

            return View(cliente);
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous] // Permitir acceso sin autenticación
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {



            _acondicionarClientes.CargaInicial(ref cliente);
            var receptor = _db.Clientes.Where(c => c.Rfc == cliente.Rfc && c.RazonSocial == cliente.RazonSocial && c.SucursalId == cliente.SucursalId).FirstOrDefault();
            if (receptor != null)
            {
                ModelState.AddModelError("", "Error RFC o Razon Social Ya Se Encuentra Registrado!!");
                return View(cliente);
            }
            // Guardar datos en TempData para asignarlo a otro metodo


            _db.Clientes.Add(cliente);
            _db.SaveChanges();

            TempData["SocioComercialId"] = cliente.Id;



            return View("CreateUsuario");




        }



        [AllowAnonymous] // Permitir acceso sin autenticación
        public ActionResult CreateUsuario()
        {

            //PopulaClientes();

            var usuario = new Usuario
            {
                Status = API.Enums.Status.Activo,
                Sucursales = new List<UsuarioSucursal>(),
                GrupoId = ObtenerGrupo()
            };

            return View(usuario);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUsuario(Usuario usuario)
        {

            var entidadExistente = _db.Usuarios.FirstOrDefault(e => e.NombreUsuario == usuario.NombreUsuario);
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

            //Asignacion de valor si es Proveedor
            usuario.esProveedor = true;
            usuario.PerfilId = 32;
            usuario.Departamento = null;
            usuario.Departamento_Id = null;
            usuario.GrupoId = (int)(TempData["grupoId"] as int?);



            // Obtener los datos guardados en TempData
            var ClienteId = TempData["SocioComercialId"] as int?;
            usuario.SocioComercialID = ClienteId;


            _db.Usuarios.Add(usuario);
            _db.SaveChanges();

            // Envío de correo electrónico de bienvenida
            EnviarCorreoBienvenida(usuario);

            return View("~/Views/Account/Login.cshtml");
        }


        private void EnviarCorreoBienvenida(Usuario usuario)
        {
            var sucursal = _db.Sucursales.Where(s => s.GrupoId == usuario.GrupoId).FirstOrDefault();

            _envioEmail.SendEmailNotifications(usuario, null, false, sucursal.Id);
        }

        private void PopulaClientes(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.SocioComercialID = popularDropDowns.PopulaClientes(receptorId);
        }

        //sub metodos

        private void PopulaForma(int? perfilId = null, int? grupoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), false);

            ViewBag.PerfilId = popularDropDowns.PopulaPerfiles(perfilId);

            ViewBag.GrupoId = popularDropDowns.PopulaGrupos(grupoId);

            ViewBag.SucursalId = popularDropDowns.PopulaSucursalesUsuarios(null);
        }

        private int ObtenerGrupo()
        {
            return Convert.ToInt32(Session["GrupoId"]);
        }

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }


    }

    #endregion
}
