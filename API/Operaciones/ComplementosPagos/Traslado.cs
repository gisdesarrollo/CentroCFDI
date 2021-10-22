using CFDI.API.Enums.CFDI33;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_traslados")]
    public class Traslado
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Impuesto")]
        public c_Impuesto NombreImpuesto { get; set; }

        [DisplayName("Tipo de Factor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public c_TipoFactor TipoFactor { get; set; }

        [DisplayName("Tasa / Cuota")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public double TasaCuota { get; set; }
        
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
