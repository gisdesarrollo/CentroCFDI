using API.Control;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto
{
    public class DataRestore
    {
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [MaxWordCount(20)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [MaxWordCount(20)]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [RegularExpression(@"^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo Electrónico")]
        [MaxWordCount(60)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Nombre Usuario")]
        [MaxWordCount(60)]
        public string Username { get; set; }
    }
}
