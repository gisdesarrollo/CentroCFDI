using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using API.Enums;
using API.Catalogos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Operaciones.Expedientes
{
    [Table("ExpedientesFiscales")]
    public class ExpedienteFiscal 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Meses Mes { get; set; }

        [DisplayName("Año")]
        public int Anio { get; set; }

        public int SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Usuario")]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Vigencia { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaDocumentoCsf { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaDocumentoOcof { get; set; }

        [NotMapped]
        public HttpPostedFileBase ArchivoConstanciaSituacionFiscal { get; set; }

        [NotMapped]
        public HttpPostedFileBase ArchivoOpinionCumplimientoSAT { get; set; }

        public String PathConstanciaSituacionFiscal { get; set; }

        public String PathOpinionCumplimientoSAT { get; set; }
    }
}
