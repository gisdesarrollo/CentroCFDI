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
using API.Enums.CartaPorteEnums;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_TransporteFerroviario")]
    public class TransporteFerroviario
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Concesionario { get; set; }

        [DisplayName("Nombre de la Aseguradora")]
        public String NombreAseg { get; set; }

        [DisplayName("Número de Poliza de Seguro")]
        public String NumPolizaSeguro { get; set; }
        [DisplayName("Tipo de Trafico")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public c_TipoDeTrafico TipoDeTrafico { get; set; }


        [DisplayName("Tipo de Servicio")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public string TipoDeServicio_Id { get; set; }
        [ForeignKey("TipoDeServicio_Id")]
        public virtual TipoDeServicio TipoDeServicio{ get; set; }      
        /*[NotMapped]
        [DisplayName("Tipo de Servicio")]
        public String TipoDeServicios { get; set; }*/


        [NotMapped]
        public virtual Carro Carro { get; set; }

        //[NotMapped]
        public virtual List<Carro> Carros { get; set; }

        [NotMapped]
        public virtual DerechosDePasos DerechosDePasos { get; set; }

        //[NotMapped]
        public virtual List<DerechosDePasos> DerechosDePasoss { get; set; }


    }
}
