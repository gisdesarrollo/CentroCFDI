using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.RelacionesCartaPorte
{
    [Table("cp_rel_ConceptoSubImpuestoConcepto")]
    public class ConceptoSubImpuestoConcepto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Concepto_Id { get; set; }
        [ForeignKey("Concepto_Id")]
        public virtual Conceptos Conceptos { get; set; }

        public int SubImpuestoConcepto_Id { get; set; }
        [ForeignKey("SubImpuestoConcepto_Id")]
        public virtual SubImpuestoC SubImpuestoConcepto { get; set; }
    }
}
