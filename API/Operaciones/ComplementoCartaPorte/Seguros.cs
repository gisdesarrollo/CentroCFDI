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
    [Table("cp_Seguros")]
    public class Seguros
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Nombre de la Aseguradora")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public string AseguraRespCivil { get; set; }

        [DisplayName("Número de Póliza de Seguro")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public string PolizaRespCivil { get; set; }

        [DisplayName("Aseguradora de Daños al medio ambiente")]
        public string AseguraMedAmbiente { get; set; }

        [DisplayName("Número de póliza de seguro por daños al medio ambiente")]
        public  string PolizaMedAmbiente { get; set; }

        [DisplayName("Aseguradora de la carga transportada")]
        public string AseguraCarga { get; set; }

        [DisplayName("Número de póliza de seguro de la carga transportada")]
        public string PolizaCarga { get; set; }

        [DisplayName("Valor de la prima del seguro")]
        public string PrimaSeguro { get; set; }
    }
}
