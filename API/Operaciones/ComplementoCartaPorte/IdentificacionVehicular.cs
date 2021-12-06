using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_IdentificacionVehicular")]
    public class IdentificacionVehicular
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Año Modelo")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public int AnioModeloVM { get; set; }

        //[Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Tipo Configuración Vehicular")]
        public String ConfigAutotransporte_Id { get; set; }
        [ForeignKey("ConfigAutotransporte_Id")]
        public virtual ConfigAutotransporte ConfigAutotransporte { get; set; }
        
        /*[NotMapped]
        [DisplayName("Configuración Vehicular (Clave de Nomeclatura)")]
        public String ConfigVehicular { get; set; }*/
        
        /*[NotMapped]
        [DisplayName("Descripción de Configuración Vehicular")]
        public String ConfigVehicularDescripcion { get; set; }*/

        [DisplayName("Placa Vehiculo Motor")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public String PlacaVM { get; set; }
    }
}
