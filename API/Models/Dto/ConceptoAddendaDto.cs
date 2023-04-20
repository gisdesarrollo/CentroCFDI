using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ConceptoAddendaDto
    {
        [DisplayName("Descripcion")]
        public String Descripcion { get; set; }
        [DisplayName("Unidad")]
        public String Unidad { get; set; }
        [DisplayName("Renglon Nota")]
        public String RenglonNota { get; set; }
        [DisplayName("Folio Nota Rec")]
        public String FolioNotaRec { get; set; }
        [DisplayName("Folio Orden Compra")]
        public String FolioOrdenCompra { get; set; }
    }
}
