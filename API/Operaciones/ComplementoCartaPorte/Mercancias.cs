using API.CatalogosCartaPorte;
using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
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
        [Required(ErrorMessage = "Campo Obligatorio Total Mercancia")]
        public int NumTotalMercancias { get; set; }

        [DisplayName("Peso Bruto Total")]
        [Required(ErrorMessage ="Campo Obligatorio Peso BrutoTotal Mercancia")]
        public Decimal PesoBrutoTotal { get; set; }

        [DisplayName("Peso Neto Total")]
        public Decimal PesoNetoTotal { get; set; }

        [StringLength(3)]
        //[Required(ErrorMessage = "Campo Obligatorio Clave unida")]
        [DisplayName("Unidad de Peso")]
        public String ClaveUnidadPeso_Id { get; set; }
        [ForeignKey("ClaveUnidadPeso_Id")]
        public virtual ClaveUnidadPeso ClaveUnidadPeso { get; set; }
        [NotMapped]
        [DisplayName("Unidad de Peso")]
        public String UnidadPeso { get; set; }

        public int? AutoTransporte_Id { get; set; }
        [ForeignKey("AutoTransporte_Id")]
        public virtual AutoTransporte AutoTransporte { get; set; }

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

      //  [NotMapped]
        public  virtual List<Mercancia> Mercanciass { get; set; }
    }
}
