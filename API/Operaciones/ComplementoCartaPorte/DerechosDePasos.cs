﻿using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_DerechosDePasos")]
    public class DerechosDePasos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Kilometraje Pagado")]
        public Decimal KilometrajePagado { get; set; }
        public String TipoDerechoDePaso_Id { get; set; }
        [ForeignKey("TipoDerechoDePaso_Id")]
        public virtual DerechosDePaso DerechosDepaso { get; set; }
        [NotMapped]
        [DisplayName("Tipo de Derecho de Paso")]
        public String TipoDerechoDePago { get; set; }
    }
}
