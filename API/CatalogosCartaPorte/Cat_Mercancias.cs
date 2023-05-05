using API.Catalogos;
using API.CatalogosCartaPorte;
using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{

    [Table("cat_mercancias")]
    public class Cat_Mercancias
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [StringLength(8)]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Clave Bienes Transportados")]
        public String ClaveProdServCP { get; set; }


        [DisplayName("Descripción")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public string Descripcion { get; set; }

        //[Required(ErrorMessage = "Campo Obligatorio")]
        public int Cantidad { get; set; }


        [DisplayName("Clave de Unidad")]
        public String ClavesUnidad { get; set; }


        public String Unidad { get; set; }


        public string Dimensiones { get; set; }


        [DisplayName("Peso en Kilogramos")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public Decimal PesoEnKg { get; set; }


        [DisplayName("Valor de la Mercancía")]
        public String ValorMercancia { get; set; }

        public c_Moneda Moneda { get; set; }


        [DisplayName("Clave de Material Peligroso")]
        public string ClaveMaterialPeligroso { get; set; }
  

        [DisplayName("Tipo Embalaje")]
        public string DescripEmbalaje { get; set; }


        [DisplayName("Clave Embalaje")]
        public string TipoEmbalaje_Id { get; set; }


        [DisplayName("Sucursal")]
        public int SucursalId { get; set; }


        [NotMapped]
        public virtual Mercancias Mercancias { get; set; }

        //[NotMapped]
        //public bool hidden { get; set; }

    }
}
