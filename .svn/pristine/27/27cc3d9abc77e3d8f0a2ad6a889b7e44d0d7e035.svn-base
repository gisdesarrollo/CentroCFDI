using API.Catalogos;
using API.Models.Facturas;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.Facturacion
{
    [Table("ORI_FACTURASEMITIDAS")]
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

    }
}
