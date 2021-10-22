using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Reportes
{
    public class HorasModel : FechasModel
    {
        [DisplayName("Hora Inicial")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Sintaxis Incorrecta")]
        public String HoraInicial { get; set; }

        [DisplayName("Hora Final")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Sintaxis Incorrecta")]
        public String HoraFinal { get; set; }
    }
}