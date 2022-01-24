using API.Catalogos;
using API.Enums.CartaPorteEnums;
using CFDI.API.Enums.CFDI33;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cat_SubImpuestoC")]
    public class Cat_SubImpuestoC
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Nombre")]
        public String Nombre { get; set; }

        public String TipoImpuesto { get; set; }

        public int Base { get; set; }

        public c_Impuesto Impuesto { get; set; }

        [DisplayName("Tipo de Factor")]
        public c_TipoFactor TipoFactor { get; set; }

        [DisplayName("Tasa o Cuota")]
        public Decimal TasaOCuota { get; set; }

        public Decimal Importe { get; set; }

        public Decimal TotalImpuestosTR { get; set; }
        [DisplayName("Sucursal")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [NotMapped]
        [DisplayName("Catalogo Impuesto")]
        public string CatImpuesto { get; set; }
    }
}

