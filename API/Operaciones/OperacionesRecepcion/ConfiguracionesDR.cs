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

    [Table("Configuraciones")]
    public  class ConfiguracionesDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set;}

        [DisplayName("Sucursal Id")]
        public int SucursalId { get; set;}
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        public bool AprobacionGastosObligatoria { get; set; }

        public bool ValidacionDocumentosObligatoria { get; set; }

        public int NumeroSolicitudGastos { get; set; }

        public int DiasPosterioresGastos { get; set; }

        public bool RecibirFacturasMesCorriente { get; set; }

        public int DiasVigenciaExpedienteFiscal { get; set; }

        public bool ReferenciaDocumentosRecibidosObligatoria { get; set; }

        public bool AprobacionComercialAutomatica { get; set; }

        public bool AprobacionPagosAutomatica { get; set; }

        public bool RecepcionCfdi { get; set; }

        public bool RecepcionNoFiscales { get; set; }

        public bool RecepcionFacturasExtranjeras { get; set; }
    }
}
