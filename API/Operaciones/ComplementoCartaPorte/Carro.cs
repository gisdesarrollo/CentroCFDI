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
    [Table("cp_Carro")]
    public class Carro
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Guías de Carro")]
        //[Required(ErrorMessage = "Campo Requerido")]
        public String GuiaCarro { get; set; }

        [DisplayName("Matrícula de Carro")]
        //[Required(ErrorMessage = "Campo Requerido")]
        public String MatriculaCarro { get; set; }

        [DisplayName("Tipo de Carro")]
        //[Required(ErrorMessage ="Campo Requerido")]
        public String TipoCarro_Id { get; set; }
        [ForeignKey("TipoCarro_Id")]
        public virtual TipoCarro TipoCarro { get; set; }
        /*[NotMapped]
        [DisplayName("Tipo de Carro")]
        public String TipoCarros { get; set; }
        */
        [DisplayName("Toneladas Netas de Carro")]
        //[Required(ErrorMessage = "Campo Requerido")]
        public Decimal ToneladasNetasCarro { get; set; }

        public int? TransporteFerroviario_Id { get; set; }
        [ForeignKey("TransporteFerroviario_Id")]
        public virtual TransporteFerroviario TransporteFerroviario { get; set; }

        [NotMapped]
        public virtual ContenedorC ContenedorC { get; set; }

        //[NotMapped]
        public virtual List<ContenedorC> ContenedoresC { get; set; }
    }
}
