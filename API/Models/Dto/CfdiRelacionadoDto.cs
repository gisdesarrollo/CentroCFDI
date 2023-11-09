using API.Operaciones.ComplementoCartaPorte;
using API.Operaciones.ComprobantesCfdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class CfdiRelacionadoDto
    {
       
        public int? ComplementoCartaPorteId { get; set; }
        [ForeignKey("ComplementoCartaPorteId")]
        public virtual ComplementoCartaPorte ComplementoCartaPorte { get; set; }

    }
}
