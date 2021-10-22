using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APBox.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password Actual")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} debe de ser de por lo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuevo Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nuevo Password")]
        [Compare("NewPassword", ErrorMessage = "Los Passwords no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [DisplayName("Recordarme?")]
        public bool RememberMe { get; set; }

    }

    public class LoginSucursal
    {
        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }

        public int UsuarioId { get; set; }

        public int ProveedorId { get; set; }

        public int GrupoId { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} debe de ser de por lo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nuevo Password")]
        [Compare("Password", ErrorMessage = "Los Password no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
