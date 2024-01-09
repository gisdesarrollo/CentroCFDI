using API.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.OperacionesProveedores
{

    [Table("configuraciones")]
    public  class ConfiguracionesDR
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set;}

        [DisplayName("Sucursal Id")]
        public int Sucursal_Id { get; set;}

        [ForeignKey("Sucursal_Id")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Nombre de Usuario")]
        public bool aprobacionGastosObligatoria { get; set; }

        [DisplayName("Nombre de Usuario")]
        public bool validacionDocumentosObligatoria { get; set; }

        [DisplayName("Nombre de Usuario")]
        public int numeroSolicitudGastos { get; set; }

        [DisplayName("Nombre de Usuario")]
        public int diasPosterioresGastos { get; set; }

        [DisplayName("Nombre de Usuario")]
        public bool recibirFacturasMesCorriente { get; set; }



    }
}
