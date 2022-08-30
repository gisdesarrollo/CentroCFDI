using API.Catalogos;
using API.Enums;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComprobantesCfdi;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.GeneracionComprobante
{
   public class ComprobanteManager
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();
        //private static string pathXml = @"D:\XML-GENERADOS-CARTAPORTE\comprobanteCFDI.xml";
        private static string pathCer = @"D:\Descargas(C)\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.cer";
        //private static string pathCer = @"C:\inetpub\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.cer";
        private static string pathKey = @"D:\Descargas(C)\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.key";
        //private static string pathKey = @"C:\inetpub\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.key";
        private static string passwordKey = "12345678a";
        #endregion

        public string GenerarComprobanteCfdi(int sucursalId, int comprobanteId)
        {

            var sucursal = _db.Sucursales.Find(sucursalId);
            var comprobanteCfdi = _db.ComprobantesCfdi.Find(comprobanteId);
            string cfdi = null;
            try
            {
                //llenado CFDI 
                cfdi = GeneraFactura(comprobanteCfdi, sucursalId);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error al momento de generar el comprobante: {0}", ex.Message));

            }

            return cfdi;

        }

        

        public string GeneraFactura(ComprobanteCfdi comprobanteCfdi, int sucursalId)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);

            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();
            objCfdi = LlenadoCfdi(comprobanteCfdi, sucursalId);

            var facturaEmitidaID = 0;
            string xml = objCfdi.Xml;
            if (objCfdi.MensajeError == "")
            {
                facturaEmitidaID = GuardarComprobante(objCfdi, comprobanteCfdi, sucursalId);
                try
                {
                    //Incrementar Folio de Sucursal segun tipo Comprobante
                    if(comprobanteCfdi.TipoDeComprobante == c_TipoDeComprobante.I) { sucursal.FolioIngreso += 1; }
                    if(comprobanteCfdi.TipoDeComprobante == c_TipoDeComprobante.E) { sucursal.FolioEgreso += 1; }
                    
                    _db.Entry(sucursal).State = EntityState.Modified;

                    _db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    var errores = new List<String>();
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            errores.Add(String.Format("Propiedad: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                        }
                    }
                    throw new Exception(string.Join(",", errores.ToArray()));
                }
                if (facturaEmitidaID > 0)
                {
                    MarcarFacturado(comprobanteCfdi.Id, facturaEmitidaID);
                }
            }
            return xml;
        }

        private RVCFDI33.GeneraCFDI LlenadoCfdi(ComprobanteCfdi comprobanteCfdi, int sucursalId)
        {
            string error = "";
            var sucursal = _db.Sucursales.Find(sucursalId);
            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();

            // Agrega el certificado prueba
            objCfdi.agregarCertificado(pathCer);

            //seleccionar folio por tipo Comprobante
            int Folio = 0;
            var Serie = "";
            if(comprobanteCfdi.TipoDeComprobante == c_TipoDeComprobante.I) { Folio = comprobanteCfdi.Sucursal.FolioIngreso; Serie = comprobanteCfdi.Sucursal.SerieIngreso; }
            if(comprobanteCfdi.TipoDeComprobante == c_TipoDeComprobante.E) { Folio = comprobanteCfdi.Sucursal.FolioEgreso; Serie = comprobanteCfdi.Sucursal.SerieEgreso; }
            //Agrega Comprobante 4.0
            objCfdi.agregarComprobante40(
                Serie,
                Folio.ToString(),
                comprobanteCfdi.FechaDocumento.ToString("yyyy-MM-ddTHH:mm:ss"),
                comprobanteCfdi.FormaPago ?? "",
                comprobanteCfdi.CondicionesPago ?? "",
                Decimal.ToDouble(comprobanteCfdi.Subtotal),
                0,
                comprobanteCfdi.Moneda.ToString()  ?? "", 
                comprobanteCfdi.TipoCambio ?? "",
                 Decimal.ToDouble(comprobanteCfdi.Total),
                comprobanteCfdi.TipoDeComprobante.ToString(), 
                comprobanteCfdi.MetodoPago.ToString() ?? "",
                sucursal.CodigoPostal,
                "",
                "01" //Exportacion Fijo
                );

            //Agrega DocRelacionado Cancelado
            if (comprobanteCfdi.TipoRelacion != null && comprobanteCfdi.UUIDCfdiRelacionado != null)
            {
                objCfdi.agregarCfdiRelacionados(
                        comprobanteCfdi.TipoRelacion
                    );
                //Obtenemos el contenido del XML seleccionado.
                //string CadenaXML = System.Text.Encoding.UTF8.GetString(comprobanteCfdi.CfdiRelacionado.ArchivoFisicoXml);
                //string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
                objCfdi.agregarCfdiRelacionado(
                        comprobanteCfdi.UUIDCfdiRelacionado
                    );

            }

            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Emisor
            var regimenFiscalEmisor = (int)comprobanteCfdi.Sucursal.RegimenFiscal;
            /*objCfdi.agregarEmisor(
                comprobanteCfdi.Sucursal.Rfc,
                comprobanteCfdi.Sucursal.RazonSocial,
                regimenFiscalEmisor.ToString()
                );*/
            objCfdi.agregarEmisor("XIA190128J61", "XENON INDUSTRIAL ARTICLES", "601");
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Receptor
            var regimeFiscalReceptor = (int)comprobanteCfdi.Receptor.RegimenFiscal;
            /*objCfdi.agregarReceptor(
                comprobanteCfdi.Receptor.Rfc,
                comprobanteCfdi.Receptor.RazonSocial,
                "", 
                "", 
                comprobanteCfdi.UsoCfdi.ToString(), //UsoCFDI Fijo
                comprobanteCfdi.Receptor.CodigoPostal,
                regimeFiscalReceptor.ToString()
                );*/
            objCfdi.agregarReceptor("URE180429TM6", "UNIVERSIDAD ROBOTICA ESPAÑOLA", "", "", comprobanteCfdi.Receptor.UsoCfdi.ToString(), "65000", "601");
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Concepto
            Boolean impuestoRetenido = false;
            Boolean impuestoTraladado = false;

            if (comprobanteCfdi.Conceptoss != null)
            {
                if (comprobanteCfdi.Conceptoss.Count > 0)
                {
                    foreach (var concepto in comprobanteCfdi.Conceptoss)
                    {
                        objCfdi.agregarConcepto(
                            concepto.ClavesProdServ, //ClaveProdServ
                            concepto.NoIdentificacion ?? "",//NoIdentificacion
                            Convert.ToDouble(concepto.Cantidad) ,//Cantidad
                            concepto.ClavesUnidad ?? "",//ClaveUnidad
                            concepto.Unidad ?? "",//Unidad
                            concepto.Descripcion ?? "",//Descripcion
                            Convert.ToDouble(concepto.ValorUnitario),//ValorUnitario
                            concepto.Importe,//Importe
                            0,//Descuento
                            concepto.ObjetoImpuestoId//ObjetoImpuesto
                            );

                        if (objCfdi.MensajeError != "")
                        {
                            error = objCfdi.MensajeError;
                            throw new Exception(string.Join(",", error));
                        }

                        //RETENCION Y TRASLADO
                        if (concepto.Traslado != null)
                        {
                            impuestoTraladado = true;
                            objCfdi.agregarImpuestoConceptoTraslado(
                                Convert.ToDouble(concepto.Traslado.Base),
                                concepto.Traslado.Impuesto,
                                concepto.Traslado.TipoFactor.ToString(),
                                Convert.ToDouble(concepto.Traslado.TasaOCuota),
                                Convert.ToDouble(concepto.Traslado.Importe)
                            );
                            if (objCfdi.MensajeError != "")
                            {
                                error = objCfdi.MensajeError;
                                throw new Exception(string.Join(",", error));
                            }
                        }


                        if (concepto.Retencion != null)
                        {
                            impuestoRetenido = true;
                            objCfdi.agregarImpuestoConceptoRetenido(
                                 Convert.ToDouble(concepto.Retencion.Base),
                                 concepto.Retencion.Impuesto,
                                 concepto.Retencion.TipoFactor.ToString(),
                                 Convert.ToDouble(concepto.Retencion.TasaOCuota),
                                 Convert.ToDouble(concepto.Retencion.Importe)
                                );

                            if (objCfdi.MensajeError != "")
                            {
                                error = objCfdi.MensajeError;
                                throw new Exception(string.Join(",", error));
                            }
                        }
                    }
                }
            }

            ///impuestos
            decimal sumaImporteT = 0;
            decimal sumaImporteR = 0;

            string impuestoT = "";
            string impuestoR = "";
            string tipoFactorT = "";
            decimal tasaCuotaT = 0;
            decimal baseT = 0;

            if (impuestoTraladado || impuestoRetenido)
            {

                objCfdi.agregarImpuestos(Convert.ToDouble(comprobanteCfdi.TotalImpuestoRetenidos), Convert.ToDouble(comprobanteCfdi.TotalImpuestoTrasladado));


                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }
                //retenido
                foreach (var impuesto in comprobanteCfdi.Conceptoss)
                {

                    if (impuesto.Retencion != null)
                    {
                        if (impuesto.Retencion.TipoFactor == API.Enums.CartaPorteEnums.c_TipoFactor.Tasa)
                        {
                            sumaImporteR += impuesto.Retencion.Importe;
                            impuestoR = impuesto.Retencion.Impuesto;
                        }
                        else
                        {
                            objCfdi.agregarRetencion
                                (
                                    impuesto.Retencion.Impuesto,
                                    Convert.ToDouble(impuesto.Retencion.Importe)
                                );
                        }
                        if (objCfdi.MensajeError != "")
                        {
                            error = objCfdi.MensajeError;
                            throw new Exception(string.Join(",", error));
                        }
                    }

                }

                if (sumaImporteR > 0)
                {
                    objCfdi.agregarRetencion(
                                   impuestoR,
                                   Convert.ToDouble(sumaImporteR)
                                   );
                }

                //traslado
                foreach (var impuesto in comprobanteCfdi.Conceptoss)
                {
                    if (impuesto.Traslado != null)
                    {

                        if (impuesto.Traslado.TipoFactor == API.Enums.CartaPorteEnums.c_TipoFactor.Tasa)
                        {
                            sumaImporteT += impuesto.Traslado.Importe;
                            impuestoT = impuesto.Traslado.Impuesto;
                            tipoFactorT = impuesto.Traslado.TipoFactor.ToString();
                            tasaCuotaT = impuesto.Traslado.TasaOCuota;
                            baseT = impuesto.Traslado.Base;
                        }
                        else
                        {
                            objCfdi.agregarTraslado(
                               impuesto.Traslado.Impuesto,
                               impuesto.Traslado.TipoFactor.ToString(),
                               Convert.ToDouble(impuesto.Traslado.TasaOCuota),
                               Convert.ToDouble(impuesto.Traslado.Importe),
                               Convert.ToDouble(impuesto.Traslado.Base) //CFDI40
                               );
                        }
                        if (objCfdi.MensajeError != "")
                        {
                            error = objCfdi.MensajeError;
                            throw new Exception(string.Join(",", error));
                        }
                    }
                }
                if (sumaImporteT > 0)
                {
                    objCfdi.agregarTraslado(
                                   impuestoT,
                                   tipoFactorT,
                                   Convert.ToDouble(tasaCuotaT),
                                   Convert.ToDouble(sumaImporteT),
                                   Convert.ToDouble(baseT) //CFDI40
                                   );
                }
            }


            //Genera XML
            objCfdi.GeneraXML(pathKey, passwordKey);
            string xml = objCfdi.Xml;
            //guardar string en un archivo
            // System.IO.File.WriteAllText(pathXml, xml);
            //Timbrado
            objCfdi = Timbra(objCfdi, sucursal);
            return objCfdi;
        }

        private RVCFDI33.GeneraCFDI Timbra(RVCFDI33.GeneraCFDI objCfdi, Sucursal sucursal)
        {
            objCfdi.TimbrarCfdi("fgomez", "12121212", "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", false);
            // Verifica Response
            if (objCfdi.MensajeError == "")
            {
                var xmlTimbrado = objCfdi.XmlTimbrado;
                //guardar string en un archivo
                //System.IO.File.WriteAllText(pathXml, xmlTimbrado);
            }
            else
            {
                var error = objCfdi.MensajeError;
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            return objCfdi;
        }

        private int GuardarComprobante(RVCFDI33.GeneraCFDI facturaDto, ComprobanteCfdi comprobanteCfdi, int sucursalId)
        {
            var utf8 = new UTF8Encoding();
            var facturaInternaEmitida = new FacturaEmitida
            {
                ComplementosPago = new List<ComplementoPago>(),
                EmisorId = sucursalId,
                ReceptorId = comprobanteCfdi.ReceptorId,
                Fecha = comprobanteCfdi.FechaDocumento,
                Folio = facturaDto.Folio,
                Moneda = (c_Moneda)comprobanteCfdi.Moneda,
                Serie = facturaDto.Serie,
                Subtotal = (double)comprobanteCfdi.Subtotal,
                TipoCambio = Convert.ToDouble(comprobanteCfdi.TipoCambio),
                TipoComprobante = comprobanteCfdi.TipoDeComprobante,
                Total = Decimal.ToDouble(comprobanteCfdi.Total),
                Uuid = facturaDto.UUID,
                ArchivoFisicoXml = utf8.GetBytes(facturaDto.XmlTimbrado),
                CodigoQR = facturaDto.GenerarQrCode()
            };
            if (comprobanteCfdi.FormaPago != null)
            {
                facturaInternaEmitida.FormaPago = comprobanteCfdi.FormaPago;
            }
            else { facturaInternaEmitida.FormaPago = null; }
            if (comprobanteCfdi.MetodoPago != null)
            {
                facturaInternaEmitida.MetodoPago = (c_MetodoPago)Enum.ToObject(typeof(c_MetodoPago), comprobanteCfdi.MetodoPago);
            }
            else { facturaInternaEmitida.MetodoPago = null; }

            _db.FacturasEmitidas.Add(facturaInternaEmitida);
            _db.SaveChanges();

            return facturaInternaEmitida.Id;
        }

        private void MarcarFacturado(int comprobanteId, int facturaEmitidaId)
        {
            var comprobanteCfdi = _db.ComprobantesCfdi.Find(comprobanteId);
            comprobanteCfdi.FacturaEmitidaId = facturaEmitidaId;
            comprobanteCfdi.Generado = true;
            _db.Entry(comprobanteCfdi).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public String GenerarXml(int comprobanteId)
        {
            try
            {
                var comprobanteCfdi = _db.ComprobantesCfdi.Find(comprobanteId);

                var path = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCfdiGenerados//{0} - {1} - {2}.xml", comprobanteCfdi.FacturaEmitida.Serie, comprobanteCfdi.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //guardar string en un archivo
                System.IO.File.WriteAllText(path, Encoding.UTF8.GetString(comprobanteCfdi.FacturaEmitida.ArchivoFisicoXml));

                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Cancelar(ComprobanteCfdi comprobanteCfdi)
        {
            var comprobante = _db.ComprobantesCfdi.Find(comprobanteCfdi.Id);
            var sucursal = _db.Sucursales.Find(comprobante.SucursalId);


            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(comprobante.FacturaEmitida.ArchivoFisicoXml);
            string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }

            //Creamos el objeto de cancelación de la DLL.
            RVCFDI33.RVCancelacion.Cancelacion objCan = new RVCFDI33.RVCancelacion.Cancelacion();
            //Definimos la ruta en donde se guardará el XML de Solicitud de Cancelación en el disco duro.
            string ArchivoCancelacion = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCancelados//{0}-{1}-{2}.xml", comprobante.FacturaEmitida.Serie, comprobante.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            //ruta temp cer y key produccion
            string cerRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.cer", comprobante.FacturaEmitida.Serie, comprobante.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string keyRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.key", comprobante.FacturaEmitida.Serie, comprobante.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            System.IO.File.WriteAllBytes(cerRuta, sucursal.Cer);
            System.IO.File.WriteAllBytes(keyRuta, sucursal.Key);
            //Creamos el XML de Solicitud de Cancelación.
            string folioSustitucion = (comprobanteCfdi.FolioSustitucion == null ? "" : comprobanteCfdi.FolioSustitucion);
            //cancelacion produccion
            //objCan.crearXMLCancelacionArchivo(cerRuta, keyRuta, sucursal.PasswordKey, UUID, ArchivoCancelacion, comprobanteCfdi.MotivoCancelacion, folioSustitucion);
            //cancelacion pruebas
            objCan.crearXMLCancelacionArchivo(pathCer, pathKey, passwordKey, UUID, ArchivoCancelacion, comprobanteCfdi.MotivoCancelacion, folioSustitucion);

            System.IO.File.Delete(cerRuta);
            System.IO.File.Delete(keyRuta);
            if (objCan.CodigoDeError != 0)
            {
                throw new Exception(String.Format("Ocurrió un error al crear el XML de Solicitud de Cancelación: " + objCan.MensajeDeError));
            }

            //Ejecutamos el proceso de cancelación en el Ambiente de Pruebas.
            objCan.enviarCancelacionArchivo(ArchivoCancelacion, "fgomez", "12121212", "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", false);
            System.IO.File.Delete(ArchivoCancelacion);
            //Ejecutamos el proceso de cancelación en el Ambiente de Producción.
            //objCan.enviarCancelacionArchivo(ArchivoCancelacion, sucursal.Rfc, sucursal.Rfc, "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", true);
            System.IO.File.Delete(ArchivoCancelacion);
            // Verifica el resultado
            if (objCan.MensajeDeError == "")
            {
                MarcarNoFacturado(comprobante.Id);
            }
            else
            {
                throw new Exception(String.Format("Ocurrió un error al cancelar el XML: " + objCan.MensajeDeError));
            }

        }

        private void MarcarNoFacturado(int comprobanteId)
        {
            var comprobante = _db.ComprobantesCfdi.Find(comprobanteId);
            comprobante.Status = API.Enums.Status.Cancelado;
            _db.Entry(comprobante).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public string DowloadAcuseCancelacion(ComprobanteCfdi comprobanteCfdi)
        {
            var sucursal = _db.Sucursales.Find(comprobanteCfdi.SucursalId);
            string xmlCancelacion = null;

            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(comprobanteCfdi.FacturaEmitida.ArchivoFisicoXml);
            string RFCEmisor = LeerValorXML(CadenaXML, "Rfc", "Emisor");
            string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }

            RVCFDI33.WSConsultasCFDReal.Service1 objConsulta = new RVCFDI33.WSConsultasCFDReal.Service1();
            RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct objCancel = new RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct();
            //credenciales prueba
             objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, "fgomez", "12121212", RFCEmisor);
            //credenciales a produccion
            //objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, sucursal.Rfc, sucursal.Rfc, RFCEmisor);
            if (objCancel._ERROR == "")
            {
                if (objCancel.xml != null && objCancel.xml != "")
                {
                    xmlCancelacion = objCancel.xml;
                }
            }
            else { throw new Exception("Error: " + objCancel._ERROR); }

            return xmlCancelacion;
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

    }
}
