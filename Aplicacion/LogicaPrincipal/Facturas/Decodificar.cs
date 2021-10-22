using CFDI.API.CFDI33.CFDI;
using CFDI.API.Complementos.Timbre11;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Aplicacion.LogicaPrincipal.Facturas
{
    public class Decodificar
    {
        public Comprobante DecodificarComprobante(String path)
        {
            var version = string.Empty;
            try
            {
                version = ObtenerPropiedad(path, "cfdi:Comprobante", "Version");
            }
            catch (Exception)
            {
                version = ObtenerPropiedad(path, "cfdi:Comprobante", "version");
            }

            switch (version)
            {
                case "3.2":
                    throw new Exception("Versión incorrecta de comprobante");
                case "3.3":
                    var serializer = new XmlSerializer(typeof(Comprobante));
                    StreamReader reader = new StreamReader(path);
                    var comprobante = (Comprobante)serializer.Deserialize(reader);
                    reader.Close();
                    reader.Dispose();
                    return comprobante;
                default:
                    throw new Exception("Versión de comprobante Incorrecto");
            }
        }

        public TimbreFiscalDigital DecodificarTimbre(Comprobante comprobante)
        {
            TimbreFiscalDigital timbreFiscalDigital = null;
            foreach (var complemento in comprobante.Complemento)
            {
                XmlElement timbreFiscalDigitalFisico = complemento.Any.FirstOrDefault(p => p.OuterXml.Contains("tfd"));


                if (timbreFiscalDigitalFisico == null)
                {
                    throw new Exception("Documento sin Timbre Fiscal Digital");
                }

                timbreFiscalDigital = ObtenerComplemento<TimbreFiscalDigital>(timbreFiscalDigitalFisico);
            }
            return timbreFiscalDigital;
        }

        private T ObtenerComplemento<T>(XmlElement element)
        {
            var serializador = new XmlSerializer(typeof(T));
            var nodo = (T)serializador.Deserialize(new XmlNodeReader(element));
            return nodo;
        }

        public string ObtenerPropiedad(String pathCompleto, String propiedad, String elemento)
        {
            var valor = string.Empty;
            var xml = new XmlDocument();
            xml.Load(pathCompleto);

            try
            {
                var elemList = xml.GetElementsByTagName(propiedad);

                for (var i = 0; i < elemList.Count; i++)
                {
                    var xmlAttributeCollection = elemList[i].Attributes;
                    if (xmlAttributeCollection != null) valor = xmlAttributeCollection[elemento].Value;
                }
                return valor;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("En el archivo {0} no se pudo obtener el campo {1} - {2} {3}", Path.GetFileName(pathCompleto), propiedad, elemento, ex.Message));
            }

        }

        #region Funciones Internas CFDI 3.2

        private void GenerarComplementoComprobante(ref Comprobante comprobante, TimbreFiscalDigital complemento)
        {
            comprobante.Complemento = new ComprobanteComplemento[1];

            var elementoXml = CrearComplemento(complemento);
            comprobante.Complemento[0] = new ComprobanteComplemento
            {
                Any = new XmlElement[1]
            };

            comprobante.Complemento[0].Any = new XmlElement[]
            {
                    elementoXml
            };
        }

        private XmlElement CrearComplemento(object complemento)
        {
            var namespaceFinal = new XmlSerializerNamespaces();
            namespaceFinal.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            namespaceFinal.Add("tfd", "http://sat.gob.mx/TimbreFiscalDigital");


            var serializador = new XmlSerializer(complemento.GetType());
            var memoryStream = new MemoryStream();
            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            var xmlDocument = new XmlDocument();

            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.IndentChar = ' ';
            serializador.Serialize(xmlTextWriter, complemento, namespaceFinal);
            memoryStream.Seek(0, SeekOrigin.Begin);

            xmlDocument.Load(memoryStream);

            XmlAttribute sustitucion = null;
            foreach (XmlAttribute atributoXml in xmlDocument.DocumentElement.Attributes.AsParallel())
            {
                if (atributoXml.Value.Equals("http://www.w3.org/2001/XMLSchema-instance"))
                {
                    sustitucion = atributoXml;
                }

            }

            if (sustitucion != null)
            {
                xmlDocument.DocumentElement.Attributes.Remove(sustitucion);
            }

            return xmlDocument.DocumentElement;
        }

        #endregion
    }
}
