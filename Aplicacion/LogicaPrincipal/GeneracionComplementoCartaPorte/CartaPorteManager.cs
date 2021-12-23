using API.Enums.CartaPorteEnums;
using API.Operaciones.ComplementoCartaPorte;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Aplicacion.LogicaPrincipal.GeneracionComplementoCartaPorte
{
    public class CartaPorteManager
    {
        private readonly AplicacionContext _db = new AplicacionContext();

        public void GenerarComplementoCartaPorte(int sucursalId, int complementoCartaPorteId, string mailAlterno)
        {
            byte[] xml = null;
            var sucursal = _db.Sucursales.Find(sucursalId);
            var complementoCartaPorte = _db.ComplementoCartaPortes.Find(complementoCartaPorteId);
            ComprobanteCFDI ocomprobanteCfdi = new ComprobanteCFDI();
            //llenado factura y complemento
            ocomprobanteCfdi = GeneraFactura(complementoCartaPorte, sucursalId);

            
        }

        public ComprobanteCFDI GeneraFactura(ComplementoCartaPorte complementoCartaPorte, int sucursalId)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);
           
            //------------------comprobante---------------
            ComprobanteCFDI oComprobante = new ComprobanteCFDI();
            oComprobante.SubTotal = complementoCartaPorte.Subtotal;
            oComprobante.Moneda = complementoCartaPorte.Moneda.Value;
            oComprobante.Total = complementoCartaPorte.Total;
            oComprobante.TipoDeComprobante = complementoCartaPorte.TipoDeComprobante;

            oComprobante.Folio = "666";
            oComprobante.Serie = "PRU";
            oComprobante.Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            oComprobante.MetodoPago = complementoCartaPorte.MetodoPago;
            //-------------------emisor-------------------
            ComprobanteEmisor oEmisor = new ComprobanteEmisor();
            oEmisor.Rfc = sucursal.Rfc;
            oEmisor.Nombre = sucursal.RazonSocial;
            oEmisor.RegimenFiscal = c_RegimenFiscal.Item601;
            //-------------------receptor----------------
            ComprobanteReceptor oReceptor = new ComprobanteReceptor();
            oReceptor.Nombre = "Pepe";
            oReceptor.Rfc = "PEP0987654321";
            oReceptor.UsoCFDI = complementoCartaPorte.UsoCfdi;
            oComprobante.Emisor = oEmisor;
            oComprobante.Receptor = oReceptor;
            //-------------------conceptos---------------
            var complementoConceptos = _db.ComplementoCartaPorteConceptos.Where(x => x.ComplementoCartaPorte_Id == complementoCartaPorte.Id).Include(x => x.Conceptos).ToList();
            List<ComprobanteConcepto> listConcepto = new List<ComprobanteConcepto>();
            List<ComprobanteConceptoImpuestosTraslado> listTraslado = new List<ComprobanteConceptoImpuestosTraslado>();
            List<ComprobanteConceptoImpuestosRetencion> listRetencion = new List<ComprobanteConceptoImpuestosRetencion>();
            List<ComprobanteImpuestosRetencion> listImpuestoRetencion = new List<ComprobanteImpuestosRetencion>();
            List<ComprobanteImpuestosTraslado> listImpuestoTraslado = new List<ComprobanteImpuestosTraslado>();
            ComprobanteImpuestos impuestoTR = new ComprobanteImpuestos();

            foreach (var concepto in complementoConceptos)
            {
                listTraslado = new List<ComprobanteConceptoImpuestosTraslado>();
                listRetencion = new List<ComprobanteConceptoImpuestosRetencion>();
                listImpuestoRetencion = new List<ComprobanteImpuestosRetencion>();
                listImpuestoTraslado = new List<ComprobanteImpuestosTraslado>();
               
                //----------------------retencion y traslado-------------------
                var complementoRT= _db.ConceptoSubImpuestoConcepto.Where(x => x.Concepto_Id == concepto.Conceptos_Id).Include(x => x.SubImpuestoConcepto).ToList();
                foreach(var impuesto in complementoRT)
                {
                    if (impuesto.SubImpuestoConcepto.TipoImpuesto == "Traslado")
                    {
                        var conceptoImpuestoTrasladado = new ComprobanteConceptoImpuestosTraslado() { 
                            Base = impuesto.SubImpuestoConcepto.Base,
                            Impuesto = impuesto.SubImpuestoConcepto.Impuesto,
                            TipoFactor = impuesto.SubImpuestoConcepto.TipoFactor,
                            TasaOCuota = impuesto.SubImpuestoConcepto.TasaOCuota,
                            Importe = impuesto.SubImpuestoConcepto.Importe    
                        };
                        listTraslado.Add(conceptoImpuestoTrasladado);
                        impuestoTR.TotalImpuestosTrasladados = impuesto.SubImpuestoConcepto.TotalImpuestosTR;
                        var impuestoTrasladado = new ComprobanteImpuestosTraslado()
                        {
                            Impuesto = impuesto.SubImpuestoConcepto.Impuesto,
                            Importe = impuesto.SubImpuestoConcepto.Importe,
                            TipoFactor = impuesto.SubImpuestoConcepto.TipoFactor,
                            TasaOCuota = impuesto.SubImpuestoConcepto.TasaOCuota
                        };
                        listImpuestoTraslado.Add(impuestoTrasladado);
                        impuestoTR.Traslados = listImpuestoTraslado.ToArray();
                    }
                    else { 
                        var conceptoImpuestoRetenido = new ComprobanteConceptoImpuestosRetencion(){
                            Base = impuesto.SubImpuestoConcepto.Base,
                            Impuesto = impuesto.SubImpuestoConcepto.Impuesto,
                            TipoFactor = impuesto.SubImpuestoConcepto.TipoFactor,
                            TasaOCuota = impuesto.SubImpuestoConcepto.TasaOCuota,
                            Importe = impuesto.SubImpuestoConcepto.Importe
                        };
                        listRetencion.Add(conceptoImpuestoRetenido);
                        impuestoTR.TotalImpuestosRetenidos = impuesto.SubImpuestoConcepto.TotalImpuestosTR;
                        var impuestoRetenido = new ComprobanteImpuestosRetencion(){
                            Impuesto = impuesto.SubImpuestoConcepto.Impuesto,
                            Importe = impuesto.SubImpuestoConcepto.Importe
                        };
                        listImpuestoRetencion.Add(impuestoRetenido);
                        impuestoTR.Retenciones = listImpuestoRetencion.ToArray();
                    }
                   
                }
                var oConcepto = new ComprobanteConcepto()
                {
                    ClaveUnidad = concepto.Conceptos.ClavesUnidad,
                    ClaveProdServ = concepto.Conceptos.ClavesProdServ,
                    Descripcion = concepto.Conceptos.Descripcion,
                    Impuestos = new ComprobanteConceptoImpuestos()
                    {
                        Retenciones = listRetencion.ToArray(),
                        Traslados = listTraslado.ToArray()
                    }
                    

                };
                listConcepto.Add(oConcepto);
                
            }
            oComprobante.Impuestos = impuestoTR;
            oComprobante.Conceptos = listConcepto.ToArray();

            //------------carta porte---------------
            
            CartaPorte ocartaPorte = new CartaPorte();
            ocartaPorte.PaisOrigenDestino = complementoCartaPorte.PaisOrigendestino;
            ocartaPorte.TotalDistRec = complementoCartaPorte.TotalDistRec;
            if (complementoCartaPorte.TranspInternac)
            {
                ocartaPorte.TranspInternac = "Sí";
            }
            else
            {
                ocartaPorte.TranspInternac = "No";
            }
             
            ocartaPorte.Version = complementoCartaPorte.Version;
            ocartaPorte.ViaEntradaSalida = complementoCartaPorte.viaEntradaSalida;
            ocartaPorte.EntradaSalidaMerc = complementoCartaPorte.EntradaSalidaMerc;
            ocartaPorte.PaisOrigenDestino = complementoCartaPorte.PaisOrigendestino;
            //-----------------ubicaciones------------------
            
            var complementoCPUbicaciones = _db.ComplementoCartaPorteUbicaciones.Where(x => x.ComplementoCartaPorte_Id == complementoCartaPorte.Id).Include(x => x.Ubicacion).ToList();
            
            List<CartaPorteUbicacion> oListUbicaciones = new List<CartaPorteUbicacion>();
            
            foreach (var ubicacion in complementoCPUbicaciones)
            {
                var oUbicacion = new CartaPorteUbicacion()
                {
                    IDUbicacion = ubicacion.Ubicacion.IDUbicacion,
                    RFCRemitenteDestinatario = ubicacion.Ubicacion.RfcRemitenteDestinatario,
                    NombreRemitenteDestinatario = ubicacion.Ubicacion.NombreRemitenteDestinatario,
                    TipoUbicacion = ubicacion.Ubicacion.TipoUbicacion
                };
                oListUbicaciones.Add(oUbicacion);
            }
            ocartaPorte.Ubicaciones = oListUbicaciones.ToArray();

            //-----------------mercancias-----------------
            CartaPorteMercancias oMercancia = new CartaPorteMercancias();
            oMercancia.CargoPorTasacion = complementoCartaPorte.Mercancias.CargoPorTasacion;
            oMercancia.NumTotalMercancias = complementoCartaPorte.Mercancias.NumTotalMercancias;
            oMercancia.PesoBrutoTotal = complementoCartaPorte.Mercancias.PesoBrutoTotal;
            oMercancia.PesoNetoTotal = complementoCartaPorte.Mercancias.PesoNetoTotal;
            oMercancia.UnidadPeso = (c_ClaveUnidadPeso)Enum.Parse(typeof(c_ClaveUnidadPeso), complementoCartaPorte.Mercancias.ClaveUnidadPeso_Id, true);

            var complementoCPMercancias = _db.MercanciasMercancias.Where(x => x.Mercancias_Id == complementoCartaPorte.Mercancias_Id).Include(x => x.Mercancia).ToList();
            foreach(var mercancia in complementoCPMercancias)
            {
                var mercanciaPedimento = _db.MercanciaPedimentos.Where(x => x.Mercancia_Id == mercancia.Mercancia_Id).Include(x => x.Pedimentos).ToList();

                var oMMercancia = new CartaPorteMercanciasMercancia()
                {

                };

                var oMPedimento = new CartaPorteMercanciasMercanciaPedimentos() 
                { 
                
                };

                var oMCantidaTransporta = new CartaPorteMercanciasMercanciaCantidadTransporta()
                {

                };

                var oMGuiasIdentificacion = new CartaPorteMercanciasMercanciaGuiasIdentificacion()
                {

                };

                var oMDetalleMercancia = new CartaPorteMercanciasMercanciaDetalleMercancia() 
                {

                };

            }

            
            oComprobante.Complemento = new ComprobanteComplementoCP[1];
            oComprobante.Complemento[0] = new ComprobanteComplementoCP();
            //-----------------------serealizacion----------------------
            XmlDocument docCartaPorte = new XmlDocument();
            XmlSerializerNamespaces xmlNamesPacesCP = new XmlSerializerNamespaces();
            xmlNamesPacesCP.Add("cartaporte20", "http://www.sat.gob.mx/CartaPorte20");

            using (XmlWriter writter = docCartaPorte.CreateNavigator().AppendChild())
            {
                new XmlSerializer(ocartaPorte.GetType()).Serialize(writter, ocartaPorte, xmlNamesPacesCP);
            }
            oComprobante.Complemento[0].Any = new XmlElement[1];
            oComprobante.Complemento[0].Any[0] = docCartaPorte.DocumentElement;

            //serializamos
            string path = @"D:\XML-GENERADOS-CARTAPORTE\carta-porte.xml";

            XmlSerializer oXmlSerealizer = new XmlSerializer(typeof(ComprobanteCFDI));
            string xml = "";

            using(var sww = new StringWriter())
            {
                using (XmlWriter writter = XmlWriter.Create(sww) )
                {
                    oXmlSerealizer.Serialize(writter,oComprobante);
                    xml = sww.ToString();
                }
            }

            //guardar string en un archivo
            System.IO.File.WriteAllText(path,xml);

            return oComprobante;
        }
    }
}
