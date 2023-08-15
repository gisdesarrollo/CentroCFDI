using API.Catalogos;
using API.Enums;
using API.Enums.CartaPorteEnums;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Correos;
using Aplicacion.LogicaPrincipal.Descargas;
using Aplicacion.LogicaPrincipal.Email;
using Aplicacion.LogicaPrincipal.Facturas;
using Aplicacion.LogicaPrincipal.GeneraPDfCartaPorte;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;


namespace Aplicacion.LogicaPrincipal.GeneracionComplementosPagos
{
    public class PagosManager
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly DescargasManager _descarga = new DescargasManager();
        private readonly CreationFile _deserealizaXml = new CreationFile();
        private readonly EnviosEmails _enviosEmails = new EnviosEmails();
        private readonly GetTipoCambioDocRel _conversionTipoCambio = new GetTipoCambioDocRel();
        private static string pathXml = @"D:\XML-GENERADOS-CARTAPORTE\PagoSenatorErrorImporteDR.xml";
        //private static string pathCer = @"D:\Descargas(C)\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.cer";
        //private static string pathCer = @"C:\inetpub\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.cer";
        //private static string pathKey = @"D:\Descargas(C)\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.key";
        //private static string pathKey = @"C:\inetpub\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.key";
        //private static string passwordKey = "12345678a";
        #endregion

        public void GenerarComplementoPago(int sucursalId, int complementoPagoId, string mailAlterno)
        {

            var sucursal = _db.Sucursales.Find(sucursalId);
            var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
            try
            {
                //llenado CFDI Complemento Pago
                GeneraFactura(complementoPago, sucursalId);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error al momento de generar el complemento: {0}", ex.Message));

            }

        }

        public void GeneraFactura(ComplementoPago complementoPago, int sucursalId)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);
            var cliente = _db.Clientes.Find(complementoPago.ReceptorId);

            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();
            objCfdi = LlenadoCfdi(complementoPago, sucursalId);

            var facturaEmitidaID = 0;
            string xml = objCfdi.Xml;
            if (objCfdi.MensajeError == "")
            {
                facturaEmitidaID = GuardarComprobante(objCfdi, complementoPago, sucursalId);
                try
                {
                    //Incrementar Folio de Sucursal
                    sucursal.Folio += 1;
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
                    MarcarFacturado(complementoPago.Id, facturaEmitidaID);
                    var utf8 = new UTF8Encoding();

                    //envio email SMTP
                    try
                    {
                        if (cliente.Email != null && cliente.Sucursal.Smtp != null && cliente.Sucursal.Puerto != null && cliente.Sucursal.PasswordCorreo != null)
                        {
                            //deserealiza XML
                            ComprobanteCFDI xmlObject = _deserealizaXml.DeserealizarXmlPagos20(complementoPago.Id);
                            var pathXml = _descarga.GeneraFilePathXml(utf8.GetBytes(objCfdi.Xml), objCfdi.Folio, objCfdi.Serie);
                            byte[] bytePdf = _descarga.GeneraPDF40(xmlObject, "Pagos40", complementoPago.Id, false);
                            var pathPdf = _deserealizaXml.GetPathPDf(bytePdf, xmlObject.Serie, xmlObject.Folio);


                            EmailDto objetcCorreo = _enviosEmails.ObjectCorreo(cliente, new List<string> { pathXml, pathPdf });
                            _enviosEmails.SendEmail(objetcCorreo);
                        }

                    }
                    catch (Exception)
                    {
                        //throw new Exception(String.Format("El comprobante se timbró de forma exitosa pero no fue posible mandarlo por correo electrónico, el motivo: {0}", ex.Message));
                    }
                }
            }

        }

        private RVCFDI33.GeneraCFDI LlenadoCfdi(ComplementoPago complementoPago, int sucursalId)
        {
            string error = "";
            var sucursal = _db.Sucursales.Find(sucursalId);
            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();

            // Agrega el certificado prueba
            //objCfdi.agregarCertificado(pathCer);
            // certificado de produccion
            objCfdi.agregarCertificadoBase64(System.Convert.ToBase64String(sucursal.Cer));

            //Agrega Comprobante 4.0
            objCfdi.agregarComprobante40(
                complementoPago.Sucursal.Serie,
                complementoPago.Sucursal.Folio.ToString(),
                complementoPago.FechaDocumento.ToString("yyyy-MM-ddTHH:mm:ss"),
                "",
                "",
                0,
                0,
                c_Moneda.XXX.ToString(), //Moneda Fijo
                "",
                0,
                c_TipoDeComprobante.P.ToString(), //Pagos Fijo
                "",
                sucursal.CodigoPostal,
                "",
                "01" //Exportacion Fijo
                );

            //Agrega DocRelacionado Cancelado
            if (complementoPago.TipoRelacion != null && complementoPago.UUIDCfdiRelacionado != null)
            {
                objCfdi.agregarCfdiRelacionados(
                        complementoPago.TipoRelacion
                    );
                //Obtenemos el contenido del XML seleccionado.
                //string CadenaXML = System.Text.Encoding.UTF8.GetString(complementoPago.CfdiRelacionado.ArchivoFisicoXml);
                //string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
                objCfdi.agregarCfdiRelacionado(
                        complementoPago.UUIDCfdiRelacionado
                    );

            }

            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Emisor Produccion
            var regimenFiscalEmisor = (int)complementoPago.Sucursal.RegimenFiscal;
            objCfdi.agregarEmisor(
                complementoPago.Sucursal.Rfc,
                complementoPago.Sucursal.RazonSocial,
                regimenFiscalEmisor.ToString()
                );
            //Emisor Prueba
            //objCfdi.agregarEmisor("XIA190128J61", "XENON INDUSTRIAL ARTICLES", "601");
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Receptor Produccion
            String numRegIdTrib = "";
            if(complementoPago.Receptor.Rfc == "XEXX010101000")
            {
                if(String.IsNullOrEmpty(complementoPago.Receptor.NumRegIdTrib))
                {
                    if (complementoPago.Receptor.Pais != API.Enums.c_Pais.MEX) {numRegIdTrib = "000000000";}
                }else { numRegIdTrib = complementoPago.Receptor.NumRegIdTrib; }
                
            }
            var regimeFiscalReceptor = (int)complementoPago.Receptor.RegimenFiscal;
            objCfdi.agregarReceptor(
                complementoPago.Receptor.Rfc,
                complementoPago.Receptor.RazonSocial,
                complementoPago.Receptor.Rfc == "XEXX010101000" ? complementoPago.Receptor.Pais.ToString():"",
                numRegIdTrib, 
                c_UsoCfdiCP.CP01.ToString(), //UsoCFDI Fijo
                complementoPago.Receptor.CodigoPostal,
                regimeFiscalReceptor.ToString()
                );
            //Receptor Prueba
            //objCfdi.agregarReceptor("URE180429TM6", "UNIVERSIDAD ROBOTICA ESPAÑOLA", "", "", c_UsoCfdiCP.CP01.ToString(), "65000", "601");
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Concepto
            objCfdi.agregarConcepto("84111506", "", 1, "ACT", "", "Pago", 0, 0, 0, "01");
            //validar si el impuestoBase0 
            double totalTrasladoImpuestoIva0 = -1;
            if(complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVA0 > 0)
            { totalTrasladoImpuestoIva0 = 0; }
            //Agrega PagosTotales
            objCfdi.agregarPago20Totales(
                complementoPago.TotalesPagosImpuestos.TotalRetencionesIVA == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalRetencionesIVA,
                complementoPago.TotalesPagosImpuestos.TotalRetencionesISR == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalRetencionesISR,
                complementoPago.TotalesPagosImpuestos.TotalRetencionesIEPS == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalRetencionesIEPS,
                complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVA16 == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVA16,
                complementoPago.TotalesPagosImpuestos.TotalTrasladosImpuestoIVA16 == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalTrasladosImpuestoIVA16,
                complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVA8 == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVA8,
                complementoPago.TotalesPagosImpuestos.TotalTrasladosImpuestoIVA8 == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalTrasladosImpuestoIVA8,
                complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVA0 == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVA0,
                totalTrasladoImpuestoIva0,
                complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVAExento == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalTrasladosBaseIVAExento,
                complementoPago.TotalesPagosImpuestos.MontoTotalPagos == 0 ? -1 : complementoPago.TotalesPagosImpuestos.MontoTotalPagos);
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            if (complementoPago.Pagos != null)
            {
                for (int x = 0; x < complementoPago.Pagos.Count; x++)
                {
                    var rfcEmisorCtaOrd = "";
                    var ctaOrdenante = "";
                    if (complementoPago.Pagos[x].BancoOrdenante != null)
                    {
                        rfcEmisorCtaOrd = complementoPago.Pagos[x].BancoOrdenante.Banco.Rfc;
                        ctaOrdenante = complementoPago.Pagos[x].BancoOrdenante.NumeroCuenta;
                    }
                    var rfcEmisorCtaB = "";
                    var ctaBeneficiario = "";
                    if (complementoPago.Pagos[x].BancoBeneficiario != null)
                    {
                        rfcEmisorCtaB = complementoPago.Pagos[x].BancoBeneficiario.Banco.Rfc;
                        ctaBeneficiario = complementoPago.Pagos[x].BancoBeneficiario.NumeroCuenta;
                    }
                    //Agrega Pagos 2.0
                    objCfdi.agregarPago20Pago(
                        complementoPago.Pagos[x].FechaPago.ToString("yyyy-MM-ddTHH:mm:ss"),
                        complementoPago.Pagos[x].FormaPago,
                        complementoPago.Pagos[x].Moneda.ToString(),
                        complementoPago.Pagos[x].TipoCambio,
                        complementoPago.Pagos[x].Monto,
                        complementoPago.Pagos[x].NumeroOperacion,
                        rfcEmisorCtaOrd,
                        complementoPago.Pagos[x].NombreBancoOrdenanteExtranjero,
                        ctaOrdenante,
                        rfcEmisorCtaB,
                        ctaBeneficiario,
                        "",
                        "",
                        "",
                        ""
                        );

                    //Agrega Documento Relacionado

                    if (complementoPago.Pagos[x].DocumentosRelacionados != null)
                    {
                        double tipoCambioDR = 1;
                        for (int i = 0; i < complementoPago.Pagos[x].DocumentosRelacionados.Count; i++)
                        {
                            //var pago = _db.Pagos.Find(complementoPago.Pagos[x].DocumentosRelacionados[i].PagoId);
                            objCfdi.agregarPago20DoctoRelacionado(
                                complementoPago.Pagos[x].DocumentosRelacionados[i].IdDocumento,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].Serie,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].Folio,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString(),
                                (double)complementoPago.Pagos[x].DocumentosRelacionados[i].EquivalenciaDR, //se calcula segun su monedaP y monedaDR 
                                (int)complementoPago.Pagos[x].DocumentosRelacionados[i].NumeroParcialidad,
                                (double)complementoPago.Pagos[x].DocumentosRelacionados[i].ImporteSaldoAnterior,
                                (double)complementoPago.Pagos[x].DocumentosRelacionados[i].ImportePagado,
                                (double)complementoPago.Pagos[x].DocumentosRelacionados[i].ImporteSaldoInsoluto,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].ObjetoImpuestoId);

                            //Agregar Impuesto Documento Relacionado
                            //Retencion

                            if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones != null)
                            {
                                for (var r = 0; r < complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones.Count; r++)
                                {
                                    objCfdi.agregarPago20RetencionDoctoRelacionado(
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Base),
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto,
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TipoFactor.ToString(),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TasaOCuota),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Importe)
                                        );
                                }
                            }


                            //Traslado

                            if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados != null)
                            {
                                for (var t = 0; t < complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados.Count; t++)
                                {
                                    objCfdi.agregarPago20TrasladoDoctoRelacionado(
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base),
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto,
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor.ToString(),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe)
                                        );
                                }
                            }
                        }
                    }
                /*}*/

                //variables Retenidos
                var tasaISRImpuestoR = "";
                decimal tasaISRImporteR = 0;
                var tasaIVAImpuestoR = "";
                decimal tasaIVAImporteR = 0;
                var cuotaIEPSImpuestoR = "";
                decimal cuotaIEPSImporteR = 0;
                var defaultImpuestoR = "";
                decimal defaultImporteR = 0;

                /*for (int x = 0; x < complementoPago.Pagos.Count; x++)
                {*/

                    if (complementoPago.Pagos[x].DocumentosRelacionados != null)
                    {
                        Decimal tipoCambioDR = 1;
                        for (int i = 0; i < complementoPago.Pagos[x].DocumentosRelacionados.Count; i++)
                        {
                            var pago = _db.Pagos.Find(complementoPago.Pagos[x].DocumentosRelacionados[i].PagoId);


                            //Impuestos Pagos Retencion

                            if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones != null /*&& complementoPago.Pagos[x].DocumentosRelacionados[i].RetencionDRId != null*/)
                            {
                                for (var r = 0; r < complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones.Count; r++)
                                {
                                    decimal RImporteDR = (decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Importe;
                                    if (pago.Moneda != complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda)
                                    {
                                        if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() == "USD" && pago.Moneda.ToString() == "MXN")
                                        {
                                            tipoCambioDR = GetTipoCambioDocRelacionadoUSD(complementoPago.Pagos[x].DocumentosRelacionados[i], pago.TipoCambio, pago.Monto);
                                            RImporteDR = (decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Importe * (decimal)tipoCambioDR;

                                        }
                                        else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() == "MXN" && pago.Moneda.ToString() == "USD")
                                        {
                                            tipoCambioDR = (decimal)pago.TipoCambio;
                                            RImporteDR = (decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Importe / tipoCambioDR;

                                        }
                                        else if ((complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() != "MXN" && pago.Moneda.ToString() == "USD") || (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() == "USD" && pago.Moneda.ToString() != "MXN"))
                                        {
                                            tipoCambioDR = _conversionTipoCambio.GetTipoCambioDocRelacionadoUSD(complementoPago.Pagos[x].DocumentosRelacionados[i], pago.TipoCambio, pago.Monto);
                                            RImporteDR = (decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Importe * (decimal)tipoCambioDR;

                                        }

                                    }
                                    else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() != "MXN" && pago.Moneda.ToString() != "MXN" && complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda == pago.Moneda)
                                    {
                                        tipoCambioDR = (Decimal)pago.TipoCambio;
                                        RImporteDR = (decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Importe; /*(decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Importe * tipoCambioDR;*/
                                    }

                                    if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto == "001" && complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TipoFactor == c_TipoFactor.Tasa &&
                                         (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TasaOCuota >= (decimal)0.0 && complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TasaOCuota <= (decimal)0.35))
                                    {
                                        tasaISRImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto;
                                        tasaISRImporteR += decimal.Round(RImporteDR, 6);

                                    }
                                    else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto == "002" && complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TipoFactor == c_TipoFactor.Tasa &&
                                         (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TasaOCuota >= (decimal)0.0 && complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TasaOCuota <= (decimal)0.16))
                                    {
                                        tasaIVAImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto;
                                        tasaIVAImporteR += decimal.Round(RImporteDR, 6);
                                    }
                                    else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto == "003" && complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TipoFactor == c_TipoFactor.Cuota &&
                                         (complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TasaOCuota >= (decimal)0.0 && complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].TasaOCuota <= (decimal)59.1449))
                                    {
                                        cuotaIEPSImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto;
                                        cuotaIEPSImporteR += decimal.Round(RImporteDR, 6);
                                    }
                                    else
                                    {
                                        defaultImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retenciones[r].Impuesto;
                                        defaultImporteR += decimal.Round(RImporteDR, 6);
                                    }
                                }

                            }
                        }

                    }
                /*}*/
                if (tasaISRImporteR > 0) { objCfdi.agregarPago20RetencionP(tasaISRImpuestoR, Convert.ToDouble(tasaISRImporteR)); }
                if (tasaIVAImporteR > 0) { objCfdi.agregarPago20RetencionP(tasaIVAImpuestoR, Convert.ToDouble(tasaIVAImporteR)); }
                if (cuotaIEPSImporteR > 0) { objCfdi.agregarPago20RetencionP(cuotaIEPSImpuestoR, Convert.ToDouble(cuotaIEPSImporteR)); }
                if (defaultImporteR > 0) { objCfdi.agregarPago20RetencionP(defaultImpuestoR, Convert.ToDouble(defaultImporteR)); }

                //variables traslados
                //Impuestos Pagos Traslado
                var tasaIVAImpuestoT = "";
                double tasaIVAImporteT = 0;
                double tasaIVABaseT = 0;
                var tasaIVATipoFactorT = "";
                double tasaIVATasaOCuota = 0;
                var tasaIVAImpuestoT0 = "";
                double tasaIVAImporteT0 = 0;
                double tasaIVABaseT0 = 0;
                var tasaIVATipoFactorT0 = "";
                double tasaIVATasaOCuota0 = 0;
                var cuotaIEPSImpuestoT = "";
                double cuotaIEPSImporteT = 0;
                double cuotaIEPSBaseT = 0;
                var cuotaIEPSTipoFactorT = "";
                double cuotaIEPSTasaOCuota = 0;
                var defaultImpuestoT = "";
                double defaultImporteT = 0;
                double defaultBaseT = 0;
                var defaultTipoFactorT = "";
                double defaultTasaOCuotat = 0;
                /*for (int x = 0; x < complementoPago.Pagos.Count; x++)
                {*/
                    if (complementoPago.Pagos[x].DocumentosRelacionados != null)
                    {

                        Decimal tipoCambioDR = 1;
                        for (int i = 0; i < complementoPago.Pagos[x].DocumentosRelacionados.Count; i++)
                        {
                            var pago = _db.Pagos.Find(complementoPago.Pagos[x].DocumentosRelacionados[i].PagoId);


                            if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados != null)
                            {
                                for (var t = 0; t < complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados.Count; t++)
                                {
                                    decimal TImporteDR = (decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe;
                                    decimal TBaseDR = (decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base;
                                    if (pago.Moneda != complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda)
                                    {
                                        if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() == "USD" && pago.Moneda.ToString() == "MXN")
                                        {
                                            tipoCambioDR = _conversionTipoCambio.GetTipoCambioDocRelacionadoUSD(complementoPago.Pagos[x].DocumentosRelacionados[i], pago.TipoCambio, pago.Monto);
                                            TImporteDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe * tipoCambioDR,8);
                                            TBaseDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base * tipoCambioDR,8);
                                        }
                                        else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() == "MXN" && pago.Moneda.ToString() == "USD")
                                        {
                                            tipoCambioDR = (decimal)pago.TipoCambio;
                                            TImporteDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe / tipoCambioDR,6);
                                            TBaseDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base / tipoCambioDR,6);
                                        } else if ((complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() != "MXN" && pago.Moneda.ToString() == "USD") || (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() == "USD" && pago.Moneda.ToString() != "MXN"))
                                        {
                                            tipoCambioDR = _conversionTipoCambio.GetTipoCambioDocRelacionadoUSD(complementoPago.Pagos[x].DocumentosRelacionados[i], pago.TipoCambio, pago.Monto);
                                            TImporteDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe * (decimal)tipoCambioDR,6);
                                            TBaseDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base * (decimal)tipoCambioDR,6);

                                        }
                                        else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() == "EUR" && pago.Moneda.ToString() == "MXN")
                                        {
                                            tipoCambioDR = _conversionTipoCambio.GetTipoCambioDocRelacionadoUSD(complementoPago.Pagos[x].DocumentosRelacionados[i], pago.TipoCambio, pago.Monto);
                                            TImporteDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe * tipoCambioDR, 8);
                                            TBaseDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base * tipoCambioDR, 8);
                                        }

                                    } else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString() != "MXN" && pago.Moneda.ToString() != "MXN" && complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda == pago.Moneda) {
                                        tipoCambioDR = (Decimal)pago.TipoCambio;
                                        TImporteDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe,6); /*(decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Importe * tipoCambioDR;*/
                                        TBaseDR = decimal.Round((decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base,6);/*(decimal)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Base * tipoCambioDR;*/
                                    }
                                    if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto == "002" && complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor == c_TipoFactor.Tasa &&
                                         (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.16
                                         || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.08))
                                    {
                                        tasaIVABaseT += Convert.ToDouble(TBaseDR);
                                        tasaIVATipoFactorT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor.ToString();
                                        tasaIVATasaOCuota = (double)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota;
                                        tasaIVAImpuestoT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto;
                                        tasaIVAImporteT += Convert.ToDouble(TImporteDR);
                                    }else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto == "002" && complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor == c_TipoFactor.Tasa &&
                                         (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.0 ))
                                    {
                                        tasaIVABaseT0 += Convert.ToDouble(TBaseDR);
                                        tasaIVATipoFactorT0 = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor.ToString();
                                        tasaIVATasaOCuota0 = (double)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota;
                                        tasaIVAImpuestoT0 = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto;
                                        tasaIVAImporteT0 += Convert.ToDouble(TImporteDR);
                                    }

                                    else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto == "003" && complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor == c_TipoFactor.Tasa &&
                                         complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.265 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.3 ||
                                         complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.53 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.5 ||
                                         complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)1.6 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.304 ||
                                         complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.25 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.09 ||
                                         complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.08 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.07 ||
                                         complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.06 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.03 ||
                                         complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota == (decimal)0.0)
                                    {
                                        cuotaIEPSBaseT += Convert.ToDouble(TBaseDR);
                                        cuotaIEPSTipoFactorT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor.ToString();
                                        cuotaIEPSTasaOCuota = (double)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota;
                                        cuotaIEPSImpuestoT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto;
                                        cuotaIEPSImporteT += Convert.ToDouble(TImporteDR);
                                    }
                                    else
                                    {
                                        defaultBaseT += Convert.ToDouble(TBaseDR);
                                        defaultTipoFactorT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TipoFactor.ToString();
                                        defaultTasaOCuotat = (double)complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].TasaOCuota;
                                        defaultImpuestoT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslados[t].Impuesto;
                                        defaultImporteT += Convert.ToDouble(TImporteDR);
                                    }
                                }
                            }

                        }

                    }

                /*}*/
                if (tasaIVAImporteT > 0) { objCfdi.agregarPago20TrasladoP(tasaIVABaseT, tasaIVAImpuestoT, tasaIVATipoFactorT, Convert.ToDouble(tasaIVATasaOCuota), tasaIVAImporteT); }
                if (tasaIVAImporteT0 == 0 && tasaIVABaseT0 > 0) { objCfdi.agregarPago20TrasladoP(tasaIVABaseT0, tasaIVAImpuestoT0, tasaIVATipoFactorT0, Convert.ToDouble(tasaIVATasaOCuota0), tasaIVAImporteT0); }
                if (cuotaIEPSImporteT > 0) { objCfdi.agregarPago20TrasladoP(cuotaIEPSBaseT, cuotaIEPSImpuestoT, cuotaIEPSTipoFactorT, Convert.ToDouble(cuotaIEPSTasaOCuota), cuotaIEPSImporteT); }
                if (defaultImporteT > 0) { objCfdi.agregarPago20TrasladoP(defaultBaseT, defaultImpuestoT, defaultTipoFactorT, Convert.ToDouble(defaultTasaOCuotat), defaultImporteT); }

                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }
            }
            }
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Genera XML Prueba
             //objCfdi.GeneraXML(pathKey, passwordKey);
            //Genera XML produccion
            objCfdi.GenerarXMLBase64(System.Convert.ToBase64String(sucursal.Key), sucursal.PasswordKey);

            string xml = objCfdi.Xml;
            //guardar string en un archivo
            //System.IO.File.WriteAllText(pathXml, xml);
            //Timbrado
            objCfdi = Timbra(objCfdi, sucursal);
            return objCfdi;
        }

        private RVCFDI33.GeneraCFDI Timbra(RVCFDI33.GeneraCFDI objCfdi, Sucursal sucursal)
        {
            //Credencial de Pruebas
            //objCfdi.TimbrarCfdi("fgomez", "12121212", "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", false);
            //Credencial de Produccion
            objCfdi.TimbrarCfdi(sucursal.Rfc, sucursal.Rfc, "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", true);

            // Verifica Response
            if (objCfdi.MensajeError == "")
            {
                var xmlTimbrado = objCfdi.XmlTimbrado;
                //guardar string en un archivo
                // System.IO.File.WriteAllText(pathXml, xmlTimbrado);
            }
            else
            {
                var error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            return objCfdi;
        }

        public void Cancelar(ComplementoPago complementoPago)
        {
            var complementoP = _db.ComplementosPago.Find(complementoPago.Id);
            var sucursal = _db.Sucursales.Find(complementoP.SucursalId);


            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(complementoP.FacturaEmitida.ArchivoFisicoXml);
            string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }
            //Creamos el XML de Solicitud de Cancelación.
            string folioSustitucion = (complementoPago.FolioSustitucion == null ? "" : complementoPago.FolioSustitucion);

            //Creamos el objeto de cancelación de la DLL.
            RVCFDI33.RVCancelacion.Cancelacion objCan = new RVCFDI33.RVCancelacion.Cancelacion();
            //Definimos la ruta en donde se guardará el XML de Solicitud de Cancelación en el disco duro.
            string ArchivoCancelacion = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCancelados//{0}-{1}-{2}.xml", complementoP.FacturaEmitida.Serie, complementoP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            /*begin::produccion*/
            //ruta temp cer y key produccion
            string cerRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.cer", complementoP.FacturaEmitida.Serie, complementoP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string keyRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.key", complementoP.FacturaEmitida.Serie, complementoP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            System.IO.File.WriteAllBytes(cerRuta, sucursal.Cer);
            System.IO.File.WriteAllBytes(keyRuta, sucursal.Key);
            objCan.crearXMLCancelacionArchivo(cerRuta, keyRuta, sucursal.PasswordKey, UUID, ArchivoCancelacion, complementoPago.MotivoCancelacion, folioSustitucion);
            System.IO.File.Delete(cerRuta);
            System.IO.File.Delete(keyRuta);
            /*end::produccion*/
            /*begin::pruebas*/
            //objCan.crearXMLCancelacionArchivo(pathCer, pathKey, passwordKey, UUID, ArchivoCancelacion, complementoPago.MotivoCancelacion, folioSustitucion);
            /*end::pruebas*/
            if (objCan.CodigoDeError != 0)
            {
                throw new Exception(String.Format("Ocurrió un error al crear el XML de Solicitud de Cancelación: " + objCan.MensajeDeError));
            }

            //Ejecutamos el proceso de cancelación en el Ambiente de Pruebas.
            //objCan.enviarCancelacionArchivo(ArchivoCancelacion, "fgomez", "12121212", "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", false);
            //System.IO.File.Delete(ArchivoCancelacion);
            //Ejecutamos el proceso de cancelación en el Ambiente de Producción.
            objCan.enviarCancelacionArchivo(ArchivoCancelacion, sucursal.Rfc, sucursal.Rfc, "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", true);
            //System.IO.File.Delete(ArchivoCancelacion);
            // Verifica el resultado
            if (objCan.MensajeDeError == "")
            {
                MarcarNoFacturado(complementoP.Id);
            }
            else
            {
                throw new Exception(String.Format("Ocurrió un error al cancelar el XML: " + objCan.MensajeDeError));
            }

        }

        public string DowloadAcuseCancelacion(ComplementoPago complementoPago)
        {
            var sucursal = _db.Sucursales.Find(complementoPago.SucursalId);
            string xmlCancelacion = null;

            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(complementoPago.FacturaEmitida.ArchivoFisicoXml);
            string RFCEmisor = LeerValorXML(CadenaXML, "Rfc", "Emisor");
            string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }

            RVCFDI33.WSConsultasCFDReal.Service1 objConsulta = new RVCFDI33.WSConsultasCFDReal.Service1();
            RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct objCancel = new RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct();
            //credenciales prueba
             //objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, "fgomez", "12121212", RFCEmisor);
            //credenciales a produccion
            objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, sucursal.Rfc, sucursal.Rfc, RFCEmisor);
            if (objCancel._ERROR == "")
            {
                if (objCancel.xml != null && objCancel.xml != "")
                {
                    xmlCancelacion = objCancel.xml;
                }
            }

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



        #region Funciones Internas

        private int GuardarComprobante(RVCFDI33.GeneraCFDI facturaDto, ComplementoPago complementoPago, int sucursalId)
        {
            var utf8 = new UTF8Encoding();

            var facturaInternaEmitida = new FacturaEmitida
            {
                ComplementosPago = new List<ComplementoPago>(),
                EmisorId = sucursalId,
                ReceptorId = complementoPago.ReceptorId,
                Fecha = complementoPago.FechaDocumento,
                Folio = facturaDto.Folio,
                Moneda = c_Moneda.XXX,
                Serie = facturaDto.Serie,
                Subtotal = 0, //(double)complementoPago.TotalesPagosImpuestos.MontoTotalPagos,
                TipoCambio = complementoPago.Pagos[0].TipoCambio,
                TipoComprobante = c_TipoDeComprobante.P,
                Total = 0, //complementoPago.TotalesPagosImpuestos.MontoTotalPagos,
                Uuid = facturaDto.UUID,
                ArchivoFisicoXml = utf8.GetBytes(facturaDto.XmlTimbrado),
                CodigoQR = facturaDto.GenerarQrCode(),
                Status = API.Enums.Status.Activo
            };
            if (complementoPago.Pagos[0].FormaPago != null)
            {
                facturaInternaEmitida.FormaPago = complementoPago.Pagos[0].FormaPago;
            }
            else { facturaInternaEmitida.FormaPago = null; }

            _db.FacturasEmitidas.Add(facturaInternaEmitida);
            _db.SaveChanges();

            return facturaInternaEmitida.Id;
        }

        private void MarcarFacturado(int complementoPagoId, int facturaEmitidaId)
        {
            var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
            complementoPago.FacturaEmitidaId = facturaEmitidaId;
            complementoPago.Generado = true;
            _db.Entry(complementoPago).State = EntityState.Modified;
            _db.SaveChanges();
        }

        private void MarcarNoFacturado(int complementoPagoId)
        {
            var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
            complementoPago.Status = API.Enums.Status.Cancelado;
            _db.Entry(complementoPago).State = EntityState.Modified;
            var facturaEmitida = _db.FacturasEmitidas.Find(complementoPago.FacturaEmitidaId);
            facturaEmitida.Status = API.Enums.Status.Cancelado;
            _db.Entry(facturaEmitida).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public String GenerarXml(int complementoPagoId)
        {
            try
            {
                var complementoPago = _db.ComplementosPago.Find(complementoPagoId);

                var path = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCfdiGenerados//{0} - {1} - {2}.xml", complementoPago.FacturaEmitida.Serie, complementoPago.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //guardar string en un archivo
                System.IO.File.WriteAllText(path, Encoding.UTF8.GetString(complementoPago.FacturaEmitida.ArchivoFisicoXml));

                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private double ConvertToDouble(string s)
        {
            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        throw new Exception("Wrong string-to-double format");
                    }
                }
            }
            return result;
        }

        public String GenerarZipFacturaRecibida(int facturaId)
        {
            try
            {
                var facturaRecibida = _db.FacturasRecibidas.Find(facturaId);
                var pathXml = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCfdiGenerados//{0} - {1} - {2}.xml", facturaRecibida.Serie, facturaRecibida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(pathXml))
                {
                    File.Delete(pathXml);
                }

                //guardar string en un archivo
                System.IO.File.WriteAllText(pathXml, Encoding.UTF8.GetString(facturaRecibida.ArchivoFisicoXml));

                return pathXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Decimal GetTipoCambioDocRelacionadoUSD(DocumentoRelacionado documentoRelacionado, double TipoCambioPago, double MontoPago)
        {
            Decimal valor = ((Decimal)documentoRelacionado.ImportePagado / (Decimal)documentoRelacionado.EquivalenciaDR) / (Decimal)documentoRelacionado.ImportePagado;
            Decimal valorFormat = decimal.Round(valor, 12);//7
            return valor;
        }
    }
}
