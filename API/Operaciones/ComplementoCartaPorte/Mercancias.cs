using API.CatalogosCartaPorte;
using API.Operaciones.ComplementoCartaPorte;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_Mercancias")]
    public class Mercancias
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [DisplayName("Cargo por Tasación")]
        public Decimal CargoPorTasacion { get; set; }

        [DisplayName("Número Total de Mercancías")]
        public int NumTotalMercancias { get; set; }

        [DisplayName("Peso Bruto Total")]
        public Decimal PesoBrutoTotal { get; set; }

        [DisplayName("Peso Neto Total")]
        public Decimal PesoNetoTotal { get; set; }

        [StringLength(3)]
        public String ClaveUnidadPeso_Id { get; set; }
        [ForeignKey("ClaveUnidadPeso_Id")]
        public virtual ClaveUnidadPeso ClaveUnidadPeso { get; set; }
        [NotMapped]
        [DisplayName("Unidad de Peso")]
        public String UnidadPeso { get; set; }

        public int? AutoTransporteFederal_Id { get; set; }
        [ForeignKey("AutoTransporteFederal_Id")]
        public virtual AutoTransporteFederal AutoTransporteFederal { get; set; }

        public int? TransporteMaritimo_Id { get; set; }
        [ForeignKey("TransporteMaritimo_Id")]
        public virtual TransporteMaritimo TransporteMaritimo { get; set; }

        public int? TransporteAereo_Id { get; set; }
        [ForeignKey("TransporteAereo_Id")]
        public virtual TransporteAereo TransporteAereo { get; set; }

        public int? TransporteFerroviario_Id { get; set; }
        [ForeignKey("TransporteFerroviario_Id")]
        public virtual TransporteFerroviario TransporteFerroviario { get; set; }

        [NotMapped]
        public virtual Mercancia Mercancia{ get; set; }

    }
}
