using API.CatalogosCartaPorte;
using API.Enums;
using API.Enums.CartaPorteEnums;
using API.Relaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Catalogos
{
    [Table("SociosComerciales")]
    public class SocioComercial
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [RegularExpression(@"^[^\s].*[^\s]$", ErrorMessage = "El campo de RazonSocial no puede terminar en un espacio.")]
        [DisplayName("Razón Social")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String RazonSocial { get; set; }

        [RegularExpression(@"^(?!.*(Ñ|ñ))[A-Z&]{3,4}(?!0000)[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[A-Z0-9]{2}[0-9A]$", ErrorMessage = "RFC inválido")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Rfc { get; set; }

        //[Required]
        public int? GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }

        [Required]
        public int SucursalId { get; set; }

        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Clave")]
        public String Clave { get; set; }

        [RegularExpression(@"^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "El formato del correo electrónico no es válido.")]
        public String Email { get; set; }

        [DisplayName("Código Postal")]
        [RegularExpression("[\\s]{0,3}([0-9]{5})[\\s]{0,3}", ErrorMessage = "El código postal tiene que conformarse de 5 caracteres numéricos")]
        public String CodigoPostal { get; set; }

        [DisplayName("Régimen Fiscal")]
        [Required(ErrorMessage = "Campo obligatorio")]
        public c_RegimenFiscal RegimenFiscal { get; set; }

        [DisplayName("Uso Cfdi")]
        public c_UsoCfdiCP UsoCfdi { get; set; }

        [DisplayName("País")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public c_Pais Pais { get; set; }

        [DisplayName("Numero Registro Identificación Tributaria")]
        public string NumRegIdTrib { get; set; }

        [DisplayName("Teléfono 1")]
        public string Telefono1 { get; set; }

        [DisplayName("Teléfono 2")]
        public string Telefono2 { get; set; }

        [DisplayName("Fecha de Alta")]
        public DateTime FechaAlta { get; set; }

        public Status Status { get; set; }

        public string Observaciones { get; set; }

        #region Grupo

        [NotMapped]
        public virtual BancoSocioComercial Banco { get; set; }

        public virtual List<BancoSocioComercial> Bancos { get; set; }

        [NotMapped]
        [DisplayName("RFC - Razón Social")]
        public string RfcRazonSocial
        { get { return string.Format("{0} - {1}", Rfc, RazonSocial); } }

        #endregion Grupo
    }
}