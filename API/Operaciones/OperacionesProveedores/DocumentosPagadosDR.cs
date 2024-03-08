using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("DocumentosPagos")]
    public class DocumentosPagadosDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? Pago_Id { get; set; }
        [ForeignKey("Pago_Id")]
        public virtual PagosDR Pagos { get; set; }

        [DisplayName("Fecha Documento")]
        public DateTime FechaDocumento { get; set; }

        public double? Total { get; set; }

        public c_Moneda? Moneda { get; set; }

        [DisplayName("Tipo de Cambio")]
        public double? TipoCambio { get; set; }

        public String Serie { get; set; }

        public String Folio { get; set; }

        public String UUID { get; set; }

        public c_MetodoPago? MetodoPago { get;set; }

        public String FormaPago { get; set; }
    }
}
