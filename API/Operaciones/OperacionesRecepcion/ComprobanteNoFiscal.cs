using API.Catalogos;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("ComprobantesNoFiscales")]
    public class ComprobanteNoFiscal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DocumentoRecibidoId { get; set; }
        [ForeignKey("DocumentoRecibidoId")]
        public virtual DocumentoRecibido DocumentoRecibido { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        public string PathS3 { get; set; }

        public string Referencia { get; set; }

        [DisplayName("Fecha de Creacion")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }

        [NotMapped]
        public HttpPostedFileBase ArchivoComprobanteNoFiscal { get; set; }

    }
}
