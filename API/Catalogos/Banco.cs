using API.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Catalogos
{
    [Table("cat_bancos")]
    public class Banco
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Razón Social")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String RazonSocial { get; set; }

        [DisplayName("Nombre Corto")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String NombreCorto { get; set; }

        [DisplayName("RFC")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Rfc { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public bool Nacional { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public Status Status { get; set; }

    }
}
