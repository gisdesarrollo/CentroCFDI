using API.Catalogos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.EnterpriseServices.Internal;
using System.Web;

namespace API.Operaciones.Expedientes
{
    [Table("ExpedientesLegales")]
    public class ExpedienteLegal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }
        public int? SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }
        public string Comentarios { get; set; }
        public string PathActaConstitutiva { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCreacionActaConstitutiva { get; set; }
        public int? UsuarioIdActaConstitutiva { get; set; }
        [ForeignKey("UsuarioIdActaConstitutiva")]
        public virtual Usuario UsuarioActaConstitutiva { get; set; }
        public int? AprobacionActaConstitutiva { get; set; }
        public string PathCedulaIdentificacionFiscal { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCreacionCedulaIdentificacionFiscal { get; set; }
        public int? UsuarioIdCedulaIdentificacionFiscal { get; set; }
        [ForeignKey("UsuarioIdCedulaIdentificacionFiscal")]
        public virtual Usuario UsuarioCedulaIdentificacionFiscal { get; set; }
        public int? AprobacionCedulaIdentificacionFiscal { get; set; }
        public string PathComprobanteDomicilio { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCreacionComprobanteDomicilio { get; set; }
        public int? UsuarioIdComprobanteDomicilio { get; set; }
        [ForeignKey("UsuarioIdComprobanteDomicilio")]
        public virtual Usuario UsuarioComprobanteDomicilio { get; set; }
        public int? AprobacionComprobanteDomicilio { get; set; }
        public string PathIdentificacionOficialRepLeg { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCreacionIdentificacionOficialRepLeg { get; set; }
        public int? UsuarioIdIdentificacionOficialRepLeg { get; set; }
        [ForeignKey("UsuarioIdIdentificacionOficialRepLeg")]
        public virtual Usuario UsuarioIdentificacionOficialRepLeg { get; set; }
        public int? AprobacionIdentificacionOficialRepLeg { get; set; }
        public string PathEstadoCuentaBancario { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCreacionEstadoCuentaBancario { get; set; }
        public int? UsuarioIdEstadoCuentaBancario { get; set; }
        [ForeignKey("UsuarioIdEstadoCuentaBancario")]
        public virtual Usuario UsuarioEstadoCuentaBancario { get; set; }
        public int? AprobacionEstadoCuentaBancario { get; set; }
        [NotMapped]
        public HttpPostedFileBase ArchivoActaConstitutiva { get; set; }
        [NotMapped]
        public HttpPostedFileBase ArchivoCedulaIdentificacionFiscal { get; set; }
        [NotMapped]
        public HttpPostedFileBase ArchivoComprobanteDomicilio { get; set; }
        [NotMapped]
        public HttpPostedFileBase ArchivoIdentificacionOficialRepLeg { get; set; }
        [NotMapped]
        public HttpPostedFileBase ArchivoEstadoCuentaBancario { get; set; }
        [NotMapped]
        public string ActaConstitutivaName { get; set; }
        [NotMapped]
        public string CedulaIdentificacionFiscalName {  get; set; }
        [NotMapped]
        public string ComprobanteDomicilioName { get; set; }
        [NotMapped]
        public string IdentificacionOficialRepLegName { get; set; }
        [NotMapped]
        public string EstadoCuentaBancarioName { get; set; }
    }
}
