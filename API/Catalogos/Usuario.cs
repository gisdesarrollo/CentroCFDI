using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Enums;
using API.Operaciones.OperacionesProveedores;
using API.Relaciones;

namespace API.Catalogos
{
    [Table("cat_usuarios")]
    public class Usuario
    {
        //Kardex
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Clave { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Nombre { get; set; }

        [DisplayName("Apellido Paterno")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String ApellidoPaterno { get; set; }

        [DisplayName("Apellido Materno")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String ApellidoMaterno { get; set; }

        [NotMapped]
        [DisplayName("Nombre Completo")]
        public String NombreCompleto
        {
            get { return Nombre + " " + ApellidoPaterno + " " + ApellidoMaterno; }
        }

        [DisplayName("Celular")]
        public String Celular { get; set; }

        [RegularExpression(@"^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "El formato del correo electrónico no es válido.")]
        [DisplayName("E-Mail")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Email { get; set; }

        [DisplayName("RFC")]
        public String Rfc { get; set; }

        //General
        //[Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Perfil")]
        public int? PerfilId { get; set; }

        [ForeignKey("PerfilId")]
        public virtual Perfil Perfil { get; set; }

        public Status Status { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "El nombre de usuario solo puede contener letras, números y guiones bajos.")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Nombre de Usuario")]
        public String NombreUsuario { get; set; }

        #region Grupo

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Grupo")]
        public int GrupoId { get; set; }

        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }

        #endregion Grupo

        #region SociosComercialesId

        [DisplayName("SocioComercial")]
        public int? SocioComercialID { get; set; }

        [ForeignKey("SocioComercialID")]
        public virtual SocioComercial SocioComercial { get; set; }

        #endregion SociosComercialesId

        #region Proveedor

        [DisplayName("Proveedor")]
        public bool esProveedor { get; set; }

        #endregion Proveedor

        #region Sucursales

        [DisplayName("Mostrar todas las Sucursales")]
        public bool TodasSucursales { get; set; }

        [NotMapped]
        public virtual UsuarioSucursal Sucursal { get; set; }

        public virtual List<UsuarioSucursal> Sucursales { get; set; }

        #endregion Sucursales

        #region Departamento

        [DisplayName("Departamento")]
        public int? Departamento_Id { get; set; }

        [ForeignKey("Departamento_Id")]
        public virtual Departamento Departamento { get; set; }

        #endregion Departamento
    }
}