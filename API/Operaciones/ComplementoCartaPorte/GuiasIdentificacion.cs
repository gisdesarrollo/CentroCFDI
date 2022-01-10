using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_GuiasIdentificacion")]
    public class GuiasIdentificacion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Numero Guia de Identificación")]
       //[Required(ErrorMessage = "Campo Obligatorio")]
        public string NumeroGuiaIdentificacion { get; set; }

        [DisplayName("Descripción Guia Identificación")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public string DescripGuiaIdentificacion { get; set; }

        [DisplayName("Peso Guia de Identificación")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Decimal PesoGuiaIdentificacion { get; set; }

        public int? Mercancia_Id { get; set; }
        [ForeignKey("Mercancia_Id")]
        public virtual Mercancia Mercancia { get; set; }

    }
}
