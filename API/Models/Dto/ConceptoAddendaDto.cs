using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    /*Nodo:Cuerpo*/
    public class ConceptoAddendaDto
    {
        [DisplayName("Descripcion")]
        public String Descripcion { get; set; }

        public Decimal Cantidad { get; set; }
        public Decimal PUnitario { get; set; }
        public Decimal Subtotal { get; set; }
        public Decimal Importe { get; set; }
        public Decimal Iva { get; set; }
        public Decimal IvaPCT { get; set; }
        [DisplayName("Centros de Costos")]
        public String CentroCosto { get; set; }
        [DisplayName("Unidad")]
        public String Unidad { get; set; }
        [DisplayName("Renglon Nota")]
        public String RenglonNota { get; set; }
        [DisplayName("Nota Recepción")]
        public String FolioNotaRec { get; set; }
        [DisplayName("Orden de Compra")]
        public String FolioOrdenCompra { get; set; }
    }
}
