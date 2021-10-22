using API.Catalogos;
using API.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("ori_solicitudesaccesos")]
    public class SolicitudAcceso
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        #region Datos de la solicitud

        [DisplayName("Llave de Grupo")]
        public Guid LlaveGrupo { get; set; }

        [NotMapped]
        public bool Autorizar { get; set; }

        public bool Procesado { get; set; }

        public String Notas { get; set; }

        #endregion

        #region Datos del Proveedor

        [DisplayName("Razón Social")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String RazonSocial { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Rfc { get; set; }

        public String Email { get; set; }

        [DisplayName("Página Web")]
        public String PaginaWeb { get; set; }

        [DisplayName("Teléfono 1")]
        public String Telefono1 { get; set; }

        [DisplayName("Teléfono 2")]
        public String Telefono2 { get; set; }

        public Status Status { get; set; }

        public DateTime FechaAlta { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public Double Saldo { get; set; }

        [DisplayName("Código Postal")]
        [RegularExpression("[\\s]{0,3}([0-9]{5})[\\s]{0,3}", ErrorMessage = "El código postal tiene que conformarse de 5 caracteres numéricos")]
        public String CodigoPostal { get; set; }

        [DisplayName("País")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Pais { get; set; }
        
        #endregion

        #region Objetos

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Grupo")]
        public int GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }

        #endregion
    }
}