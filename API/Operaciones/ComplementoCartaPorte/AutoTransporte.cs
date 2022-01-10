using API.CatalogosCartaPorte;
using API.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_AutoTransporte")]
    public class AutoTransporte
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /*[DisplayName("Nombre de la Aseguradora")]
        public String NombreAseg { get; set; }*/

        [DisplayName("Número de Permiso SCT")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public String NumPermisoSCT { get; set; }

        /*[DisplayName("Número de Póliza de Seguro")]
        public String NumPolizaSeguro { get; set; }*/
        [DisplayName("Permiso SCT")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public String TipoPermiso_Id { get; set; }
        [ForeignKey("TipoPermiso_Id")]
        public virtual TipoPermiso TipoPermiso{ get; set; }

        /*[NotMapped]
        [DisplayName("Permiso SCT")]
        public String PermSCT { get; set; }
        */
        /*[NotMapped]
        [DisplayName("Descripción Permisos SCT")]
        public String DescripcionPermSCT { get; set; }*/

        [DisplayName("Identificación Vehicular")]
        //[Required(ErrorMessage ="Campo Requerido")]
        public int IdentificacionVehicular_Id { get; set; }
        [ForeignKey ("IdentificacionVehicular_Id")]
        public virtual IdentificacionVehicular IdentificacionVehicular { get; set; }

       /* public int? Remolques_Id { get; set; }
        [ForeignKey("Remolques_Id")]
        public virtual Remolques Remolques { get; set; }
        */
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public int? Seguros_Id { get; set; }
        [ForeignKey("Seguros_Id")]
        public virtual Seguros Seguros { get; set; }

       
        
        [NotMapped]
        public virtual Remolques Remolque { get; set; }

        //[NotMapped]
        public virtual List<Remolques> Remolquess { get; set; }



    }
}
