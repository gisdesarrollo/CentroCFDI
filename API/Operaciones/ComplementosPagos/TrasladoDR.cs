using API.Enums.CartaPorteEnums;
using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_trasladodocrel")]
    public class TrasladoDR 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.000000}", ApplyFormatInEditMode = true)]
        public Double Base { get; set; }

        public string Impuesto { get; set; }

        [DisplayName("Tipo de Factor")]
        public c_TipoFactor TipoFactor { get; set; }

        [DisplayName("Tasa o Cuota")]
        [DisplayFormat(DataFormatString = "{0:0.000000}", ApplyFormatInEditMode = true)]
        public Decimal TasaOCuota { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.000000}", ApplyFormatInEditMode = true)]
        public Double Importe { get; set; }

        [DisplayName("DocRelacionado")]
        public int? DocRelacionadoId { get; set; }
        [ForeignKey("DocRelacionadoId")]
        public virtual DocumentoRelacionado DocRelacionado { get; set; }
    }
}
