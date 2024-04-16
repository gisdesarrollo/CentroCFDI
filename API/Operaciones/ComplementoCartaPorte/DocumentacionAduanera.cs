using API.Enums.CartaPorteEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_documentacionaduanera")]
    public class DocumentacionAduanera
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Tipo documento")]
        public c_DocumentoAduanero? TipoDocumento { get; set; }
        [DisplayName("Numero de pedimento")]
        public String NumPedimento { get; set; }
        [DisplayName("Indentificador aduanero")]
        public String IdentDocAduanero { get; set; }
        [DisplayName("RFC importador")]
        public String RfcImpo { get; set; }
        public int? Mercancia_Id { get; set; }
        [ForeignKey("Mercancia_Id")]
        public virtual Mercancia Mercancia { get; set; }

    }
}
