using API.Enums.Addenda;
using API.Models.Dto;
using Aplicacion.LogicaPrincipal.ComplementosPagos;
using Aplicacion.LogicaPrincipal.Facturas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Utilerias.LogicaPrincipal;

namespace Aplicacion.LogicaPrincipal.Addenda
{
    public class ProcesaAddenda
    {
        private readonly Decodificar _decodificar = new Decodificar();
        OperacionesStreams _operacionesStreams = new OperacionesStreams();
        public AddendaDto ParseoXml(ComprobanteCFDI comprobante40)
        {
            /*Encabeazado*/
            AddendaDto addendadto = new AddendaDto() 
            {
                Fecha = comprobante40.Fecha,
                Subtotal = comprobante40.SubTotal,
                Total = comprobante40.Total,
                //Iva = comprobante40.Impuestos.TotalImpuestosTrasladados

            };
            if (comprobante40.Impuestos != null)
            {
                addendadto.Iva = comprobante40.Impuestos.TotalImpuestosTrasladados != 0 ? comprobante40.Impuestos.TotalImpuestosTrasladados : 0 ;

                if (comprobante40.Impuestos.Traslados.Length > 0)
                {
                    foreach(var traslado in comprobante40.Impuestos.Traslados)
                    {
                        addendadto.IvaPCT = (int)(traslado.TasaOCuota * 100);
                    }
                }
            }
            /*Cuerpo*/
            addendadto.ConceptosAddenda = new List<ConceptoAddendaDto>();
            if(comprobante40.Conceptos.Length > 0)
            {
                foreach(var concepto in comprobante40.Conceptos)
                {
                    
                    var conceptoAddenda = new ConceptoAddendaDto() 
                    {
                        Descripcion = concepto.Descripcion,
                        Unidad = concepto.Unidad,
                        Cantidad = concepto.Cantidad,
                        PUnitario = concepto.ValorUnitario,
                        Subtotal = concepto.Importe,
                        
                    };
                    if(concepto.Impuestos.Traslados.Length > 0)
                    {
                        foreach(var traslado in concepto.Impuestos.Traslados)
                        {
                            conceptoAddenda.Importe = concepto.Importe;
                            conceptoAddenda.Iva = traslado.Importe;
                            conceptoAddenda.IvaPCT = traslado.TasaOCuota;
                            
                        }
                    }
                    addendadto.ConceptosAddenda.Add(conceptoAddenda);
                }
            }
            return addendadto;
        }

        public ComprobanteCFDI DecodificaXML(String pathXml)
        {
            ComprobanteCFDI comprobante40 = new ComprobanteCFDI();
            var version = string.Empty;
            try
            {
                version = _decodificar.ObtenerPropiedad(pathXml, "cfdi:Comprobante", "Version");
                
                if (version == "4.0")
                {
                    comprobante40 = _decodificar.DecodificarComprobante40(pathXml, version);
                    
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return comprobante40;
        }

        public FacturaInterfactura GeneraNodoAddenda(AddendaDto addendaDto)
        {
            //llenado objeto FacturaInterfactura
            FacturaInterfactura facturaInterfactura = new FacturaInterfactura() 
            {
                TipoDocumento = t_TipoDocumento.Factura,
                Emisor = new FacturaInterfacturaEmisor()
                {
                    RI = addendaDto.EmisorRegInter
                },
                Receptor = new FacturaInterfacturaReceptor() 
                {
                    RI = addendaDto.ReceptorREgInter
                },
                //llenado encabezado
                Encabezado = new FacturaInterfacturaEncabezado()
                {
                    NumProveedor = addendaDto.NumProveedor,
                    Fecha = addendaDto.Fecha,
                    FolioOrdenCompra = addendaDto.FolioOrdenCompra,
                    TipoProveedorEKT = 3,
                    SubTotal = addendaDto.Subtotal,
                    Total = addendaDto.Total,
                    Iva = addendaDto.Iva,
                    IVAPCT = addendaDto.IvaPCT,
                    MonedaDoc = addendaDto.Moneda,
                    //Observaciones = addendaDto.Observaciones
                    FolioNotaRecepcion = addendaDto.FolioNotaRecepcion
                }
                
            };
            //llenado cuerpo
            int contadorRenglon = 1;
            facturaInterfactura.Encabezado.Cuerpo = new FacturaInterfacturaEncabezadoCuerpo[100];
            List<FacturaInterfacturaEncabezadoCuerpo> cuerpoList = new List<FacturaInterfacturaEncabezadoCuerpo>();
            foreach (var concepto in addendaDto.ConceptosAddenda)
            {
                
                var facturaInterfacturaEncabezadoCuerpo = new FacturaInterfacturaEncabezadoCuerpo()
                {
                    Renglon = contadorRenglon.ToString(),
                    RenglonNotaRecepcion = concepto.RenglonNota,
                    FolioNotaRecepcion = concepto.FolioNotaRec,
                    FolioOrdenCompra = concepto.FolioOrdenCompra,
                    Cantidad = concepto.Cantidad,
                    Importe = concepto.Importe,
                    Concepto = concepto.Descripcion,
                    PUnitario = concepto.PUnitario
                    
                };
                cuerpoList.Add(facturaInterfacturaEncabezadoCuerpo);
                contadorRenglon++;
            }
            facturaInterfactura.Encabezado.Cuerpo = cuerpoList.ToArray();

            return facturaInterfactura; 
        }

        public String SerealizaAddenda(FacturaInterfactura oFacturaInterfactura,String pathXml,string pathArchivo)
        {
            
            // Creamos una instancia de XmlSerializer para serializar 
            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(FacturaInterfactura));
            // Create an instance of the XmlSerializerNamespaces class
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            // Add the "if" namespace to the namespaces collection
            namespaces.Add("if", "https://www.interfactura.com/Schemas/Documentos");
            

            string xml = "";
            string cadenToRemove = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
            
            using (var sww = new StringWriter())
            {
                using(XmlWriter writter = XmlWriter.Create(sww))
                {
                    oXmlSerializar.Serialize(writter, oFacturaInterfactura,namespaces);
                    xml = sww.ToString();
                }
            }

            //eliminamos una cadena que no debe ir en el nodo
            int index = xml.IndexOf(cadenToRemove); // Busca el índice de la cadena a eliminar
            if (index >= 0)
            {
                xml = xml.Remove(index, cadenToRemove.Length); // Elimina la cadena
            }
            //guardamos el xml en file
            //System.IO.File.WriteAllText(pathXml, xml);

            // Cargar el archivo XML
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathArchivo);
            
            // Crear el nuevo nodo que se va a agregar
            XmlElement nuevoNodo = xmlDoc.CreateElement("cfdi","Addenda", "http://www.example.com/addenda");
            nuevoNodo.InnerXml = xml;
            

            // Create a namespace manager for the CFDI 4.0 namespace
            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4");

            // Encontrar el nodo después del cual se va a agregar el nuevo nodo
            XmlNode nodoDespues = xmlDoc.SelectSingleNode("/cfdi:Comprobante/cfdi:Complemento", nsManager);
            
            // Insertar el nuevo nodo después del nodo encontrado
            nodoDespues.ParentNode.InsertAfter(nuevoNodo, nodoDespues);
            //eliminamos el file addenda
            System.IO.File.Delete(pathXml);
            // Guardar los cambios en el archivo XML
            xmlDoc.Save(pathArchivo);

            string fileContent = File.ReadAllText(pathArchivo);
            string cadena = " xmlns:cfdi=\"http://www.example.com/addenda\"";
            //eliminamos una cadena que no debe ir en el nodo
            index = fileContent.IndexOf(cadena); // Busca el índice de la cadena a eliminar
            if (index >= 0)
            {
                fileContent = fileContent.Remove(index, cadena.Length); // Elimina la cadena
            }
            //guardamos el xml en file
            System.IO.File.WriteAllText(pathArchivo, fileContent);

            return pathArchivo;
        }
    }
}
