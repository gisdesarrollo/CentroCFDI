using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    public class ResponseAuthenticacionDto
    {
        public String status { get; set; }
        public DataItem data { get; set; }
    }

    public class DataItem
    {
        public String token { get; set; }
        public String expires_in { get; set; }
        public String tokeny_type { get; set; }
    }
}
