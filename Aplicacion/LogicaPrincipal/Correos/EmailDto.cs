using System.Collections.Generic;

namespace Aplicacion.LogicaPrincipal.Email
{
    public class EmailDto
    {
        public string EmailEmisor { get; set; }
        public List<string> EmailsReceptores { get; set; }
        public string EncabezadoCorreo { get; set; }
        public string CuerpoCorreo { get; set; }
        public string NombreSucursal { get; set; }
        public List<ArchivosAdjuntosDto> Archivos { get; set; }
        public string User { get; set; }
        public string Contrasena { get; set; }
        public string Servidor { get; set; }
        public int? Puerto { get; set; }
        public bool? Ssl { get; set; }
    }

}
