
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
        public ComprobanteCFDI DecodificarComprobante40(String path, String version)
        {
            
            switch (version)
            {
                case "3.2":
                    throw new Exception("Versión incorrecta de comprobante");
                case "4.0":
                    var serializer40 = new XmlSerializer(typeof(ComprobanteCFDI));
                    StreamReader reader40 = new StreamReader(path);
                    var comprobante40 = (ComprobanteCFDI)serializer40.Deserialize(reader40);
                    foreach (var oComplementoInterior in comprobante40.Complemento.Any)
                    {
                        if (oComplementoInterior.Name.Contains("TimbreFiscalDigital"))
                        {

                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(TimbreFiscalDigital));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                comprobante40.TimbreFiscalDigital =
                                    (TimbreFiscalDigital)oSerializerComplemento.Deserialize(readerComplemento);
                            }

                        }
                        
                        if (oComplementoInterior.Name.Contains("Pagos"))
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(Pagos));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                comprobante40.Pagos =
                                    (Pagos)oSerializerComplemento.Deserialize(readerComplemento);
                            }

                        }

                    }

                    reader40.Close();
                    reader40.Dispose();
                    return comprobante40;
                default:
                    throw new Exception("Versión de comprobante Incorrecto");
            }
        }
        public ComprobanteCFDI33 DecodificarComprobante33(String path, String version )
        {
            
            switch (version)
            {
                case "3.2":
                    throw new Exception("Versión incorrecta de comprobante");
                case "3.3":
                    var serializer33 = new XmlSerializer(typeof(ComprobanteCFDI33));
                    StreamReader reader33 = new StreamReader(path);
                    var comprobante33 = (ComprobanteCFDI33)serializer33.Deserialize(reader33);
                    reader33.Close();
                    reader33.Dispose();
                    return comprobante33;
                default:
                    throw new Exception("Versión de comprobante Incorrecto");
            }
        }

        public TimbreFiscalDigital DecodificarTimbre(ComprobanteCFDI comprobante40, ComprobanteCFDI33 comprobante33)
        {
            TimbreFiscalDigital timbreFiscalDigital = null;
            if (comprobante40 != null)
            {
               
                    XmlElement timbreFiscalDigitalFisico = comprobante40.Complemento.Any.FirstOrDefault(p => p.OuterXml.Contains("tfd"));

                    if (timbreFiscalDigitalFisico == null)
                    {
                        throw new Exception("Documento sin Timbre Fiscal Digital");
                    }

                    timbreFiscalDigital = ObtenerComplemento<TimbreFiscalDigital>(timbreFiscalDigitalFisico);
                
            }
            if(comprobante33 != null)
            {
                foreach (var complemento in comprobante33.Complemento)
                {
                    XmlElement timbreFiscalDigitalFisico = complemento.Any.FirstOrDefault(p => p.OuterXml.Contains("tfd"));


                    if (timbreFiscalDigitalFisico == null)
                    {
                        throw new Exception("Documento sin Timbre Fiscal Digital");
                    }

                    timbreFiscalDigital = ObtenerComplemento<TimbreFiscalDigital>(timbreFiscalDigitalFisico);
                }
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

       /* private void GenerarComplementoComprobante(ref Comprobante comprobante, TimbreFiscalDigital complemento)
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
        }*/

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
