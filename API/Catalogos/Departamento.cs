﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Enums;
using System.ComponentModel;

namespace API.Catalogos
{
    [Table("Departamentos")]
    public class Departamento
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        
        public String Nombre { get; set; }

        public String Clave { get; set; }

        #region Sucursal

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Sucursal")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        #endregion
    }
}
