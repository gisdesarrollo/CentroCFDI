using API.Models.Dto;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComprobantesCfdi;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace API.Operaciones.RelacionesCfdi
{
    [Table("ori_cfdirelacionado")]
    public class CfdiRelacionado: CfdiRelacionadoDto
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Tipo Relacion")]
        public String TipoRelacion { get; set; }
        [DisplayName("UUID Relacionado")]
        public String UUIDCfdiRelacionado { get; set; }

        public int? ComplementoPagoId { get; set; }
        [ForeignKey("ComplementoPagoId")]
        public virtual ComplementoPago ComplementoPago { get; set; }
        public int? ComprobanteId { get; set; }
        [ForeignKey("ComprobanteId")]
        public virtual ComprobanteCfdi ComprobanteCfdi { get; set; }

    }
}
