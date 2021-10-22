using API.Catalogos;
using API.Models.Facturas;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.Facturacion
{
    [Table("ori_facturasemitidas")]
    public class FacturaEmitida : Factura
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Emisor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int EmisorId { get; set; }
        [ForeignKey("EmisorId")]
        public virtual Sucursal Emisor { get; set; }

        [DisplayName("Receptor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ReceptorId { get; set; }
        [ForeignKey("ReceptorId")]
        public virtual Cliente Receptor { get; set; }
        [NotMapped]
        public string FolioComplementoPago { get; set; }
        [NotMapped]
        public string SerieComplementoPago { get; set; }

        [NotMapped]
        public bool FacturaEmitidaPagada { get; set; }

        [NotMapped]
        public int FacturaComplementoPagoId { get; set;}
    }
}
