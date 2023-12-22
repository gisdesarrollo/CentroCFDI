using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ResponseErrorDto
    {
        public String message { get; set; }
        public String messageDetail { get; set; }
        public String data { get; set; }
        public String status { get; set; }
    }
}
