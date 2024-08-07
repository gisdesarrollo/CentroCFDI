﻿using API.Catalogos;
using API.Models.Dto;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Aplicacion.LogicaPrincipal.Validacion
{
    
    public class DecodificaFacturas
    {
        private static string pathXml = @"D:\XML-GENERADOS-CARTAPORTE\FacturaRelacionada.xml";
        private readonly AplicacionContext _db = new AplicacionContext();
        public List<ImpuestoDeserealizadoDto> DecodificarFactura(FacturaEmitida facturaEmitida)
        {
            ImpuestosRTDto impuestoDto = new ImpuestosRTDto();
            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(facturaEmitida.ArchivoFisicoXml);
            // System.IO.File.WriteAllText(pathXml, CadenaXML);
            //var doc = XDocument.Parse(CadenaXML);
           
            Comprobante oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(Comprobante));
            
            List<ImpuestoDeserealizadoDto> impuestoDtoList = new List<ImpuestoDeserealizadoDto>();
            ImpuestoDeserealizadoDto impuestosDTO;
            //string versionCfdi = LeerValorXML(CadenaXML, "Version", "Comprobante");

            //string path = @"C:\Users\Alexander\Downloads\Factura_ComplementoPago_v1\FacturaA1.xml";
            //string CadenaXML = System.IO.File.ReadAllText(path);
            string versionCfdi = LeerValorXML(CadenaXML, "Version", "Comprobante");
            
            if (versionCfdi == "4.0")
            {
                using (StreamReader reader = new StreamReader(new MemoryStream(facturaEmitida.ArchivoFisicoXml), Encoding.UTF8))
                {
                    //aqui deserializamos
                    oComprobante = (Comprobante)oSerializer.Deserialize(reader);
                }
                /*using (StreamReader reader = new StreamReader(path))
                {
                    //aqui deserializamos
                    oComprobante = (Comprobante)oSerializer.Deserialize(reader);
                }*/
                decimal Base = 0;
                if (oComprobante.Impuestos != null)
                {
                    if (oComprobante.Impuestos.Traslados != null && oComprobante.Impuestos.Traslados.Length > 0)
                    {
                        
                        foreach (var ooComprobante in oComprobante.Impuestos.Traslados)
                        {
                            impuestosDTO = new ImpuestoDeserealizadoDto()
                            {
                                TipoImpuesto = "Traslado",
                                Base = ooComprobante.Base,
                                TasaOCuota = ooComprobante.TasaOCuota,
                                TipoFactor = ooComprobante.TipoFactor,
                                Impuesto = ooComprobante.Impuesto,
                                Importe = ooComprobante.Importe
                            };
                           
                            impuestoDtoList.Add(impuestosDTO);
                        }

                    }
                    if (oComprobante.Impuestos.Retenciones != null && oComprobante.Impuestos.Retenciones.Length > 0)
                    {
                        decimal baseR = 0;
                        string tipoFactorR = "";
                        decimal tasaOCuotaR = 0;
                     if(oComprobante.Conceptos !=null && oComprobante.Conceptos.Length > 0) { 
                        foreach(var oConcepto in oComprobante.Conceptos)
                            {
                                if(oConcepto.Impuestos!= null)
                                {
                                    if(oConcepto.Impuestos.Retenciones!=null && oConcepto.Impuestos.Retenciones.Length >0)
                                    {
                                        foreach(var retencion in oConcepto.Impuestos.Retenciones)
                                        {
                                            baseR = retencion.Base;
                                            tipoFactorR = retencion.TipoFactor;
                                            tasaOCuotaR = retencion.TasaOCuota;
                                        }
                                    }
                                }
                            }
                        }   
                        foreach (var ooComprobante in oComprobante.Impuestos.Retenciones)
                        {
                            impuestosDTO = new ImpuestoDeserealizadoDto()
                            {
                                TipoImpuesto = "Retencion",
                                Base = baseR,
                                TasaOCuota = tasaOCuotaR,
                                TipoFactor = tipoFactorR,
                                Impuesto = ooComprobante.Impuesto,
                                Importe = ooComprobante.Importe
                            };
                            impuestoDtoList.Add(impuestosDTO);
                        }

                    }
                }
            }
            return impuestoDtoList;
        }
        public ComprobanteCFDI33 DeserealizarXML33(byte[] byteXml)
        {
            
            ComprobanteCFDI33 oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI33));

            //using (StreamReader reader = new StreamReader(new MemoryStream(xmlLocal), Encoding.UTF8))
            using (StreamReader reader = new StreamReader(new MemoryStream(byteXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI33)oSerializer.Deserialize(reader);


                //complementos
                foreach (var oComplemento in oComprobante.Complemento)
                {
                    foreach (var oComplementoInterior in oComplemento.Any)
                    {
                        if (oComplementoInterior.Name.Contains("TimbreFiscalDigital"))
                        {

                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(TimbreFiscalDigital));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.TimbreFiscalDigital =
                                    (TimbreFiscalDigital)oSerializerComplemento.Deserialize(readerComplemento);
                            }

                        }
                        if (oComplementoInterior.Name.Contains("CartaPorte"))
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(CartaPorte));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.CartaPorte =
                                    (CartaPorte)oSerializerComplemento.Deserialize(readerComplemento);
                            }
                        }
                        if (oComplementoInterior.Name.Contains("Pagos"))
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(Pagos10));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.Pagos =
                                    (Pagos10)oSerializerComplemento.Deserialize(readerComplemento);
                            }
                        }

                    }
                }
            }

            return oComprobante;
        }

        public ComprobanteCFDI DeserealizarXML40(byte[] byteXml)
        {
            
            ComprobanteCFDI oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI));

            using (StreamReader reader = new StreamReader(new MemoryStream(byteXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI)oSerializer.Deserialize(reader);


                //complementos

                foreach (var oComplementoInterior in oComprobante.Complemento.Any)
                {
                    if (oComplementoInterior.Name.Contains("TimbreFiscalDigital"))
                    {

                        XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(TimbreFiscalDigital));
                        using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                        {
                            oComprobante.TimbreFiscalDigital =
                                (TimbreFiscalDigital)oSerializerComplemento.Deserialize(readerComplemento);
                        }

                    }
                    if (oComplementoInterior.Name.Contains("CartaPorte"))
                    {
                        string version = GetCartaPorteVersion(oComplementoInterior);
                        if (version == "2.0")
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(CartaPorte));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.CartaPorte20 =
                                    (Aplicacion.CARTAPORTE20Xsd.CartaPorte)oSerializerComplemento.Deserialize(readerComplemento);
                            }
                        }
                        if(version == "3.0" || version == "3.1")
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(CartaPorte));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.CartaPorte30 =
                                    (CartaPorte)oSerializerComplemento.Deserialize(readerComplemento);
                            }
                        }
                    }
                    if (oComplementoInterior.Name.Contains("Pagos"))
                    {
                        XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(Pagos));
                        using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                        {
                            oComprobante.Pagos =
                                (Pagos)oSerializerComplemento.Deserialize(readerComplemento);
                        }

                    }

                }

            }
            return oComprobante;
        }
        public string LeerValorXML(string xml, string atributo, string nodo)
        {
            //Variables
            string valor;
            int inicio = 0;
            int fin = 0;
            int indexNodo = 0;
            atributo = " " + atributo + "=\"";
            if (!string.IsNullOrEmpty(nodo))
            {
                indexNodo = xml.IndexOf(nodo);
            }
            if (indexNodo < 0)
                return "";
            //Buscamos y leemos el nombre del atributo
            inicio = xml.IndexOf(atributo, indexNodo) + atributo.Length;
            if (inicio < atributo.Length)
            {
                return "";
            }
            fin = xml.IndexOf("\"", inicio);
            valor = xml.Substring(inicio, fin - inicio);
            //Regreso de valores si encontro
            return valor;
        }

        public string TipoDocumentoCfdi33(byte[] byteXml)
        {
            string tipodocumento = null;
            ComprobanteCFDI33 oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI33));

            using (StreamReader reader = new StreamReader(new MemoryStream(byteXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI33)oSerializer.Deserialize(reader);

                //buscamos si tiene complementos o no
                
                foreach (var oComplemento in oComprobante.Complemento)
                {
                    foreach (var oComplementoInterior in oComplemento.Any)
                    {
                        if (oComplementoInterior.Name.Contains("CartaPorte"))
                        {
                            tipodocumento = "CartaPorte33";
                            break;
                        }
                        else if (oComplementoInterior.Name.Contains("Pagos"))
                        {
                            tipodocumento = "Pagos33";
                            break;
                        }
                        else { tipodocumento = "Cfdi33"; }

                    }
                }
            }
            return tipodocumento;
        }
        public string TipoDocumentoCfdi40(byte[] byteXml)
        {
            string tipoDocumento = null;
            ComprobanteCFDI oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI));

            using (StreamReader reader = new StreamReader(new MemoryStream(byteXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI)oSerializer.Deserialize(reader);


                //complementos

                foreach (var oComplementoInterior in oComprobante.Complemento.Any)
                {
                    
                    if (oComplementoInterior.Name.Contains("CartaPorte"))
                    {
                        tipoDocumento = "CartaPorte40";
                        break;
                    }
                    else if (oComplementoInterior.Name.Contains("Pagos"))
                    {
                        tipoDocumento = "Pagos40";
                        break;
                    }
                    else { tipoDocumento = "Cfdi40"; }

                }

            }
            return tipoDocumento;
        }

        private string GetCartaPorteVersion(XmlElement oComplementoInterior)
        {
            if (oComplementoInterior.Name.Contains("CartaPorte"))
            {
                // Extraer la versión directamente del atributo 'Version' si está disponible
                if (oComplementoInterior.HasAttribute("Version"))
                {
                    return oComplementoInterior.GetAttribute("Version");
                }

                // Si el atributo 'Version' no está en el nivel superior, buscar más profundamente
                // Esto puede depender de cómo está formateado tu XML específico
                foreach (XmlNode childNode in oComplementoInterior.ChildNodes)
                {
                    if (childNode is XmlElement childElement && childElement.HasAttribute("Version"))
                    {
                        return childElement.GetAttribute("Version");
                    }
                }
            }
            return null;
        }
        #region Funciones

        private T ObtenerComplemento<T>(XmlElement element)
        {
            var serializador = new XmlSerializer(typeof(T));
            var nodo = (T)serializador.Deserialize(new XmlNodeReader(element));
            return nodo;
        }

        #endregion

    }
}
