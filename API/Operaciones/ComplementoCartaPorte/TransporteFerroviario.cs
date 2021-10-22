﻿using API.CatalogosCartaPorte;
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

        public string TipoDeServicio_Id { get; set; }
        [ForeignKey("TipoDeServicio_Id")]
        public virtual TipoDeServicio TipoDeServicio{ get; set; }      
        [NotMapped]
        [DisplayName("Tipo de Servicio")]
        public String TipoDeServicios { get; set; }

        [NotMapped]
        public virtual Carro Carro { get; set; }

        [NotMapped]
        public virtual DerechosDePasos DerechosDePasos { get; set; }

    }
}
