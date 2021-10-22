using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace API.Models
{
    public class Archivos
    {
        [NotMapped]
        [DisplayName("Archivo")]
        public HttpPostedFileBase Archivo { get; set; }

        public String NombreArchivo { get; set; }

        [DisplayName("Archivo")]
        public byte[] ArchivoFisico { get; set; }
    }
}
