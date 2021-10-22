using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Enums;
using CFDI.API.Enums.CFDI33;
using API.Relaciones;
using System.Collections.Generic;
using System.Web;

namespace API.Catalogos
{
    [Table("CAT_SUCURSALES")]
    public class Sucursal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        #region Informacion General

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Nombre { get; set; }

        public Status Status { get; set; }

        [DisplayName("E-Mail de Confirmación")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Sintaxis Incorrecta")]
        public String MailConfirmacion { get; set; }

        [DisplayName("E-Mail Emisor")]
        public String MailEmisor { get; set; }

        [NotMapped]
        [DisplayName("Logo")]
        public HttpPostedFileBase ArchivoLogo { get; set; }

        public String NombreLogo { get; set; }

        public byte[] Logo { get; set; }

        #endregion

        #region Informacion Fiscal

        [DisplayName("Razón Social")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String RazonSocial { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Rfc { get; set; }

        [RegularExpression("[\\s]{0,3}([0-9]{5})[\\s]{0,3}", ErrorMessage = "El código postal tiene que conformarse de 5 caracteres numéricos")]
        [DisplayName("Código Postal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String CodigoPostal { get; set; }

        [DisplayName("País")]
        [Required(ErrorMessage = "Campo obligatorio")]
        public c_Pais Pais { get; set; }

        [DisplayName("Régimen Fiscal")]
        [Required(ErrorMessage = "Campo obligatorio")]
        public c_RegimenFiscal RegimenFiscal { get; set; }

        #endregion

        #region Informacion Correo

        //Correo Electronico
        [DisplayName("Encabezado de Correo")]
        public String EncabezadoCorreo { get; set; }

        [DisplayName("Cuerpo de Correo")]
        public String CuerpoCorreo { get; set; }

        [DisplayName("Password de Correo")]
        public String PasswordCorreo { get; set; }

        [DisplayName("SMTP")]
        public String Smtp { get; set; }

        public int? Puerto { get; set; }

        [DisplayName("SSL")]
        public bool Ssl { get; set; }

        #endregion

        #region Especificaciones Fiscales

        [NotMapped]
        [DisplayName("Archivo Cer")]
        public HttpPostedFileBase ArchivoCer { get; set; }

        public String NombreArchivoCer { get; set; }

        public byte[] Cer { get; set; }

        [NotMapped]
        [DisplayName("Archivo Key")]
        public HttpPostedFileBase ArchivoKey { get; set; }

        public String NombreArchivoKey { get; set; }

        public byte[] Key { get; set; }

        public String PasswordKey { get; set; }

        public String Serie { get; set; }

        public int Folio { get; set; }

        #endregion

        #region Informacion XSA

        [DisplayName("Servicio")]
        public String Servidor { get; set; }

        [DisplayName("Key XSA")]
        public String KeyXsa { get; set; }

        [DisplayName("Fecha Inicial")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicial { get; set; }

        #endregion

        #region Bancos

        [NotMapped]
        public virtual BancoSucursal Banco { get; set; }
        public virtual List<BancoSucursal> Bancos { get; set; }

        #endregion

        #region Grupo

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Grupo")]
        public int GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }

        #endregion
    }
}
