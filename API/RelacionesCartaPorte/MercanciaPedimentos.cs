﻿using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.RelacionesCartaPorte
{
    [Table("cp_rel_MercanciaPedimentos")]
    public class MercanciaPedimentos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Mercancia_Id { get; set; }
        [ForeignKey("Mercancia_Id")]
        public virtual Mercancia Mercancia { get; set; }

        public int Pedimentos_Id { get; set; }
        [ForeignKey("Pedimentos_Id")]
        public virtual Pedimentos Pedimentos { get; set; }
    }
}
