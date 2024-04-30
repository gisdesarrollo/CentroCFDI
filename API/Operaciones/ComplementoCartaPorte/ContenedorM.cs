using API.CatalogosCartaPorte;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_contenedorm")]
    public class ContenedorM
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Matricula del Contenedor")]
        public String MatriculaContenedor { get; set; }
        
        [DisplayName("Número de Precinto")]
        public String NumPrecinto { get; set; }

        [DisplayName("Tipo de Contenedor")]
        public String ContenedorMaritimo_Id { get; set; }
        [ForeignKey("ContenedorMaritimo_Id")]
        public virtual ContenedorMaritimo ContenedorMaritimo { get; set; }
        //campos nuevos version 3.0 carta porte
        public String IdCCPRelacionado { get; set; }
        public String PlacaVMCCP { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCertificacionCCP { get; set; }
        //
        public int? TransporteMaritimo_Id { get; set; }
        [ForeignKey("TransporteMaritimo_Id")]
        public virtual TransporteMaritimo TransporteMaritimo { get; set; }
    }
}
