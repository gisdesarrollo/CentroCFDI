using API.Models.Reportes;
using API.Operaciones.OperacionesProveedores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API.Models.DocumentosPagos
{
    public class DocumentosPagosModel : FechasModel
    {
        [DisplayName("Previsualización")]
        public bool Previsualizacion { get; set; }

        [DisplayName("Archivo")]
        public HttpPostedFileBase Archivo { get; set; }
        public virtual List<PagosDR> Pagos { get; set; }

        
    }
}
