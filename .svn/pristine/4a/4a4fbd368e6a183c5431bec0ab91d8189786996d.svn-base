using API.Models.Spei;
using System.Xml;
using System.Xml.Serialization;

namespace Aplicacion.LogicaPrincipal.CargasMasivas.CSV
{
    public class OperacionesSpei
    {
        public SPEI_Tercero Decodificar(string path)
        {
            var serializer = new XmlSerializer(typeof(SPEI_Tercero));
            var speiTercero = (SPEI_Tercero)serializer.Deserialize(new XmlTextReader(path));

            return speiTercero;
        }
    }
}
