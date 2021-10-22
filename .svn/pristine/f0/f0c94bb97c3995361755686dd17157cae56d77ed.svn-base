using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Enums;
using API.Relaciones;

namespace API.Catalogos
{
    [Table("CAT_USUARIOS")]
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

        [DisplayName("E-Mail")]
        public String Email { get; set; }

        [DisplayName("RFC")]
        public String Rfc { get; set; }

        //General
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Perfil")]
        public int PerfilId { get; set; }
        [ForeignKey("PerfilId")]
        public virtual Perfil Perfil { get; set; }
        
        public Status Status { get; set; }
        
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Nombre de Usuario")]
        public String NombreUsuario { get; set; }

        #region Grupo

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Grupo")]
        public int GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }

        #endregion

        #region Sucursales

        [DisplayName("Mostrar todas las Sucursales")]
        public bool TodasSucursales { get; set; }

        [NotMapped]
        public virtual UsuarioSucursal Sucursal { get; set; }
        public virtual List<UsuarioSucursal> Sucursales { get; set; }

        #endregion

    }
}