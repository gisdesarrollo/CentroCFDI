using API.Enums;
using API.Enums.Addenda;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace API.Models.Dto
{
    /*Nodo:Encabezado*/
    public class AddendaDto
    {
        [DisplayName("Tipo Documento")]
        public String Tipodocumento { get; set; }

        [DisplayName("Emisor Registro Interfactura")]
        public String EmisorRegInter { get; set; }
        [DisplayName("Receptor Registor Interfactura")]
        public String ReceptorREgInter { get; set; }
        [DisplayName("Numero de Proveedor")]
        public String NumProveedor { get; set; }
        
        public ProveedorEKT TipoProveedorEKT { get; set; }
        public String Fecha { get; set; }

        public Decimal Subtotal { get; set; }
        public Decimal Total { get; set; }
        public Decimal Iva { get; set; }
        public int IvaPCT { get; set;}
        
        [DisplayName("Moneda")]
        public MonedaAddenda Moneda { get; set; }
        
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
        public String CentroCosto { get; set; }

        [DisplayName("Renglon Nota Recepcion")]
        public String RenglonNotaRecepcion { get; set; }
        [DisplayName("Folio Nota Recepcion")]
        public String FolioNotaRecepcion { get; set; }

        public String Observaciones { get; set; }
        [DisplayName("Archivo")]
        public HttpPostedFileBase Archivo { get; set; }

        public String PathArchivo { get; set; }

        public bool Procesado { get; set; }

        public ConceptoAddendaDto ConceptoAddenda { get; set; }
        public virtual List<ConceptoAddendaDto> ConceptosAddenda { get; set; }
    }
}
