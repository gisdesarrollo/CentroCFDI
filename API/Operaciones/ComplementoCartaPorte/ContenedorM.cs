using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_ContenedorM")]
    public class ContenedorM
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Matricula del Contenedor")]
       // [Required(ErrorMessage = "Campo obligatorio")]
        public String MatriculaContenedor { get; set; }
        
        [DisplayName("Número de Precinto")]
        public String NumPrecinto { get; set; }

        [DisplayName("Tipo de Contenedor")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public String ContenedorMaritimo_Id { get; set; }
        [ForeignKey("ContenedorMaritimo_Id")]
        public virtual ContenedorMaritimo ContenedorMaritimo { get; set; }

        public int? TransporteMaritimo_Id { get; set; }
        [ForeignKey("TransporteMaritimo_Id")]
        public virtual TransporteMaritimo TransporteMaritimo { get; set; }
    }
}
