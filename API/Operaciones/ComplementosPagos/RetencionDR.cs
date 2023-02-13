using API.Enums.CartaPorteEnums;
using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_retenciondocrel")]
    public class RetencionDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Double Base { get; set; }

        public string Impuesto { get; set; }

        [DisplayName("Tipo de Factor")]
        public c_TipoFactor TipoFactor { get; set; }

        [DisplayName("Tasa o Cuota")]
        public Decimal TasaOCuota { get; set; }

        public Double Importe { get; set; }


        [DisplayName("DocRelacionado")]
        public int? DocRelacionadoId { get; set; }
        [ForeignKey("DocRelacionadoId")]
        public virtual DocumentoRelacionado DocRelacionado { get; set; }
    }
}
