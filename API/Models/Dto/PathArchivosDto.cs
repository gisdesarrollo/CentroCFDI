using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class PathArchivosDto
    {
        public string PathDestinoXml { get; set; }
        public string PathDestinoPdf { get; set; }
        public string PathDestinoNoFiscales { get; set; }
        public string PathDestinoExtranjeros { get; set; }
        public string PathDestinoAdjunto { get; set; }
    }
}
