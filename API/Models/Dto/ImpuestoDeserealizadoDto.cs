using API.Enums.CartaPorteEnums;
using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ImpuestoDeserealizadoDto
    {
        public String TipoImpuesto { get; set; }

        public Decimal Base { get; set; }

        public string Impuesto { get; set; }

        public string TipoFactor { get; set; }

        public Decimal TasaOCuota { get; set; }

        public Decimal Importe { get; set; }


    }
}
