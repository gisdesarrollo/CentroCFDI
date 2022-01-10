using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using CFDI.API.Enums.CFDI33;
using API.Catalogos;
using API.Models.Dto;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_Ubicaciones")]
    public class Ubicacion : UbicacionDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        
        [NotMapped]
        public string IdUbicacionDestino { get; set; }

        public int? Complemento_Id { get; set; }
        [ForeignKey("Complemento_Id")]
        public virtual ComplementoCartaPorte ComplementoCP { get; set; }

    }
}
