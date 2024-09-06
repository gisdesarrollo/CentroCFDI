using API.Catalogos;
using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using API.Operaciones.OperacionesProveedores;

namespace API.Operaciones.OperacionesRecepcion
{
    [Table("documentoasociadodr")]
    public class DocumentoAsociadoDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        public int? SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        [DisplayName("Referencia a Dcoumento de Socio Comercial")]
        public string ReferenciaDocumentoSocioComercial { get; set; }

        [DisplayName("Tipo de Documento Asociado")]
        public TipoDocumentoAsociado TipoDocumentoAsociado { get; set; }

        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        [DisplayName("Fecha de Solicitud")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaSolicitud { get; set; }

        public decimal Monto { get; set; }

        [DisplayName("Moneda")]
        public c_Moneda? MonedaId { get; set; }

        [DisplayName("Fecha de Entrega")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaEntrega { get; set; }

        [NotMapped]
        [DisplayName("Nombre")]
        public string Nombre
        { get { return string.Format("{0} - {1} - {2}", TipoDocumentoAsociado, Monto, MonedaId); } }
    }
}
