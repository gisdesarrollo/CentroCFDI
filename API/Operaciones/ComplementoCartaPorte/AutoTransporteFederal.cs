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
    [Table("cp_AutoTransporteFederal")]
    public class AutoTransporteFederal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Nombre de la Aseguradora")]
        public String NombreAseg { get; set; }

        [DisplayName("Número de Permiso SCT")]
        public String NumPermisoSCT { get; set; }

        [DisplayName("Número de Póliza de Seguro")]
        public String NumPolizaSeguro { get; set; }

        public String TipoPermiso_Id { get; set; }
        [ForeignKey("TipoPermiso_Id")]
        public virtual TipoPermiso TipoPermiso{ get; set; }
        
        [NotMapped]
        [DisplayName("Permiso SCT")]
        public String PermSCT { get; set; }

        [NotMapped]
        [DisplayName("Descripción Permisos SCT")]
        public String DescripcionPermSCT { get; set; }

        public int IdentificacionVehicular_Id { get; set; }
        [ForeignKey ("IdentificacionVehicular_Id")]
        public virtual IdentificacionVehicular IdentificacionVehicular { get; set; }

        public int? Remolques_Id { get; set; }
        [ForeignKey("Remolques_Id")]
        public virtual Remolques Remolques { get; set; }

        [NotMapped]
        public virtual Operador Operador { get; set; }

    }
}
