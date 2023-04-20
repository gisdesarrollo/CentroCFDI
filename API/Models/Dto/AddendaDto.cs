using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class AddendaDto
    {

        [DisplayName("Emisor Registro Interfactura")]
        public int EmisorRegInter { get; set; }
        [DisplayName("Receptor Registor Interfactura")]
        public int ReceptorREgInter { get; set; }
        [DisplayName("Moneda")]
        public String Moneda { get; set; }
        [DisplayName("Numero de Proveedor")]
        public String NumProveedor { get; set; }
        [DisplayName("Folio orden de Compra")]
        public String FolioOrdenCompra { get; set; }
        [DisplayName("Nombre del Solicitante")]
        public String NombreSolicitante { get; set; }
        [DisplayName("Apellido Paterno Solcitante")]
        public String ApellidoPaterno { get; set; }
        [DisplayName("Apellido Materno Solicitante")]
        public String ApellidoMaterno { get; set; }
        [DisplayName("Produccion")]
        public String Produccion { get; set; }
        [DisplayName("Centros de Costos")]
        public String Centrocosto { get; set; }

        [DisplayName("Renglon Nota Recepcion")]
        public String RenglonNotaRecepcion { get; set; }
        [DisplayName("Folio Nota Recepcion")]
        public String FolioNotaRecepcion { get; set; }

        public virtual List<ConceptoAddendaDto> ConceptosAddenda { get; set; }
    }
}
