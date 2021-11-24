using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class PartesTransporteDto
    {
        public String ParteTransporte { get; set; }
        public String Calle { get; set; }

        public String CodigoPostal { get; set; }
        public String Colonia { get; set; }

        public String Estado { get; set; }
        public String Localidad { get; set; }
        public String Municipio { get; set; }

        public String NumeroExterior { get; set; }

        public String NumeroInterior { get; set; }

        public String Pais { get; set; }
        public String Referencia { get; set; }
    }
}
