using CFDI.API.Enums.CFDI33;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_retenciones")]
    public class Retencion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Impuesto")]
        public c_Impuesto NombreImpuesto { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public double Importe { get; set; }

        //Objetos
        [DisplayName("Impuesto")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ImpuestoId { get; set; }
        [ForeignKey("ImpuestoId")]
        public virtual Impuesto Impuesto { get; set; }
    }
}
