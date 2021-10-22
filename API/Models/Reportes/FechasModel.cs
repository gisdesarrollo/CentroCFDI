using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Reportes
{
    public class FechasModel
    {
        [DisplayName("Fecha Inicial")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicial { get; set; }

        [DisplayName("Fecha Final")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinal { get; set; }
    }
}