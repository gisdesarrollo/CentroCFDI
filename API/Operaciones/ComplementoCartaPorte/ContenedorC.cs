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
    [Table("cp_ContenedorC")]
    public class ContenedorC
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Required(ErrorMessage = "Campo Requerido")]
        public Decimal PesoContenedorVacio { get; set; }

        //[Required(ErrorMessage = "Campo Requerido")]
        public Decimal PesoNetoMercancia { get; set; }


        //[Required(ErrorMessage = "Campo Requerido")]
        [DisplayName("Tipo de contenedor")]
        public String Contenedor_Id { get; set; }
        [ForeignKey("Contenedor_Id")]
        public virtual Contenedor Contenedor { get; set; }

        public int? Carro_Id { get; set; }
        [ForeignKey("Carro_Id")]
        public virtual Carro Carro { get; set; }


    }
}
