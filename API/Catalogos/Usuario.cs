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
    [Table("Usuarios")]
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

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Nombre de Usuario")]
        public String NombreUsuario { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Grupo")]
        public int GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }

        [DisplayName("SocioComercial")]
        public int? SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        [DisplayName("Proveedor")]
        public bool esProveedor { get; set; }

        [DisplayName("Mostrar todas las Sucursales")]
        public bool TodasSucursales { get; set; }
        [NotMapped]
        public virtual UsuarioSucursal Sucursal { get; set; }
        public virtual List<UsuarioSucursal> Sucursales { get; set; }

        [DisplayName("Departamento")]
        public int? DepartamentoId { get; set; }
        [ForeignKey("DepartamentoId")]
        public virtual Departamento Departamento { get; set; }

    }
}