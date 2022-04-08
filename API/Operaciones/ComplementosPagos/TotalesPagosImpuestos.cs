using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_totalespagosimpuestos")]
    public class TotalesPagosImpuestos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Total Retenciónes IVA")]
        public double TotalRetencionesIVA { get; set; }

        [DisplayName("Total Retenciónes ISR")]
        public double TotalRetencionesISR{ get; set; }

        [DisplayName("Total Retenciónes IEPS")]
        public double TotalRetencionesIEPS { get; set; }

        [DisplayName("Total Traslados Base IVA16")]
        public double TotalTrasladosBaseIVA16 { get; set; }

        [DisplayName("Total Traslados Impuesto IVA16")]
        public double TotalTrasladosImpuestoIVA16 { get; set; }

        [DisplayName("Total Traslados Base IVA8")]
        public double TotalTrasladosBaseIVA8 { get; set; }

        [DisplayName("Total Traslado Impuesto IVA8")]
        public double TotalTrasladosImpuestoIVA8 { get; set; }

        [DisplayName("Total Traslado Base IVA0")]
        public double TotalTrasladosBaseIVA0 { get; set; }

        [DisplayName("Total Traslado Impuesto IVA0")]
        public double TotalTrasladosImpuestoIVA0 { get; set; }

        [DisplayName("Total Traslado Base IVA Exento")]
        public double TotalTrasladosBaseIVAExento { get; set; }

        [DisplayName("Monto Total Pagos")]
        public double MontoTotalPagos { get; set; }
    }
}
