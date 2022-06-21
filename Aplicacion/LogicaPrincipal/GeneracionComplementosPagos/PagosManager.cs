using API.Catalogos;
using API.Enums;
using API.Enums.CartaPorteEnums;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using DTOs.Correos;
using DTOs.Facturacion.Facturacion;
using Infodextra.LogicaPrincipal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using Utilerias.LogicaPrincipal;

namespace Aplicacion.LogicaPrincipal.GeneracionComplementosPagos
{
    public class PagosManager
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();
        private static string pathXml = @"D:\XML-GENERADOS-CARTAPORTE\complementoPagos.xml";
        private static string pathCer = @"C:\Users\Alexander\Downloads\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.cer";
        private static string pathKey = @"C:\Users\Alexander\Downloads\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.key";
        private static string passwordKey = "12345678a";
        #endregion

        public string GenerarComplementoPago(int sucursalId, int complementoPagoId, string mailAlterno)
        {
            
                var sucursal = _db.Sucursales.Find(sucursalId);
                var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
                string cfdi = null;
                try
                {
                    //llenado CFDI Complemento Pago
                    cfdi = GeneraFactura(complementoPago, sucursalId);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Error al momento de generar el complemento: {0}", ex.Message));

                }

                return cfdi;
            
        }

        public string GeneraFactura(ComplementoPago complementoPago, int sucursalId)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);

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
                }
            }
            return xml;
        }

        private RVCFDI33.GeneraCFDI LlenadoCfdi(ComplementoPago complementoPago, int sucursalId)
        {
            string error = "";
            var sucursal = _db.Sucursales.Find(sucursalId);
            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();

            // Agrega el certificado prueba
            objCfdi.agregarCertificado(pathCer);

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
            if (complementoPago.TipoRelacion != null && complementoPago.CfdiRelacionadoId != null)
            {
                objCfdi.agregarCfdiRelacionados(
                        complementoPago.TipoRelacion
                    );
                //Obtenemos el contenido del XML seleccionado.
                string CadenaXML = System.Text.Encoding.UTF8.GetString(complementoPago.CfdiRelacionado.ArchivoFisicoXml);
                string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
                objCfdi.agregarCfdiRelacionado(
                        UUID
                    );

            }

            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Emisor
            var regimenFiscalEmisor = (int)complementoPago.Sucursal.RegimenFiscal;
            /*objCfdi.agregarEmisor(
                complementoPago.Sucursal.Rfc,
                complementoPago.Sucursal.RazonSocial,
                regimenFiscalEmisor.ToString()
                );*/
            objCfdi.agregarEmisor("XIA190128J61", "XENON INDUSTRIAL ARTICLES", "601");
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Receptor
            var regimeFiscalReceptor = (int)complementoPago.Receptor.RegimenFiscal;
            /*objCfdi.agregarReceptor(
                complementoPago.Receptor.Rfc,
                complementoPago.Receptor.RazonSocial,
                "", 
                "", 
                c_UsoCfdiCP.CP01.ToString(), //UsoCFDI Fijo
                complementoPago.Receptor.CodigoPostal,
                regimeFiscalReceptor.ToString()
                );*/
            objCfdi.agregarReceptor("URE180429TM6", "UNIVERSIDAD ROBOTICA ESPAÑOLA", "", "", c_UsoCfdiCP.CP01.ToString(), "65000", "601");
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Agrega Concepto
            objCfdi.agregarConcepto("84111506", "", 1, "ACT", "", "Pago", 0, 0, 0, "01");
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
                complementoPago.TotalesPagosImpuestos.TotalTrasladosImpuestoIVA0 == 0 ? -1 : complementoPago.TotalesPagosImpuestos.TotalTrasladosImpuestoIVA0,
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
                    double tipoCambio = 0;
                    if (complementoPago.Pagos[x].DocumentosRelacionados != null)
                    {
                        for (int i = 0; i < complementoPago.Pagos[x].DocumentosRelacionados.Count; i++)
                        {
                            //validacion equivalencia DR
                            if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda == complementoPago.Pagos[x].Moneda)
                            {
                                tipoCambio = 1.0;
                            }
                            else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda == c_Moneda.MXN &&
                                complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda != complementoPago.Pagos[x].Moneda)
                            {
                                tipoCambio = 1;
                            }
                            
                            objCfdi.agregarPago20DoctoRelacionado(
                                complementoPago.Pagos[x].DocumentosRelacionados[i].IdDocumento,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].Serie,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].Folio,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].Moneda.ToString(),
                                tipoCambio,
                                (int)complementoPago.Pagos[x].DocumentosRelacionados[i].NumeroParcialidad,
                                (double)complementoPago.Pagos[x].DocumentosRelacionados[i].ImporteSaldoAnterior,
                                (double)complementoPago.Pagos[x].DocumentosRelacionados[i].ImportePagado,
                                (double)complementoPago.Pagos[x].DocumentosRelacionados[i].ImporteSaldoInsoluto,
                                complementoPago.Pagos[x].DocumentosRelacionados[i].ObjetoImpuestoId);

                            //Agregar Impuesto Documento Relacionado
                            //Retencion
                            
                                if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion != null && complementoPago.Pagos[x].DocumentosRelacionados[i].RetencionDRId != null)
                                {
                                    objCfdi.agregarPago20RetencionDoctoRelacionado(
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Base),
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto,
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TipoFactor.ToString(),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TasaOCuota),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Importe)
                                        );
                                }
                            
                            //Traslado
                            
                                if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado != null && complementoPago.Pagos[x].DocumentosRelacionados[i].TrasladoDRId != null)
                                {
                                    objCfdi.agregarPago20TrasladoDoctoRelacionado(
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Base),
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Impuesto,
                                        complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TipoFactor.ToString(),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota),
                                        Convert.ToDouble(complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Importe)
                                        );
                                }


                            //Impuestos Pagos Retencion
                            var tasaISRImpuestoR = "";
                            decimal tasaISRImporteR = 0;
                            var tasaIVAImpuestoR = "";
                            decimal tasaIVAImporteR = 0;
                            var cuotaIEPSImpuestoR = "";
                            decimal cuotaIEPSImporteR = 0;
                            var defaultImpuestoR = "";
                            decimal defaultImporteR = 0;
                            if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion != null && complementoPago.Pagos[x].DocumentosRelacionados[i].RetencionDRId != null)
                            {
                                if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto == "001" && complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TipoFactor == c_TipoFactor.Tasa &&
                                     (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TasaOCuota >= (decimal)0.0 && complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TasaOCuota <= (decimal)0.35))
                                {
                                    tasaISRImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto;
                                    tasaISRImporteR += complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Importe;

                                }
                                else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto == "002" && complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TipoFactor == c_TipoFactor.Tasa &&
                                     (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TasaOCuota >= (decimal)0.0 && complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TasaOCuota <= (decimal)0.16))
                                {
                                    tasaIVAImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto;
                                    tasaIVAImporteR += complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Importe;
                                }
                                else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto == "003" && complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TipoFactor == c_TipoFactor.Cuota &&
                                     (complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TasaOCuota >= (decimal)0.0 && complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.TasaOCuota <= (decimal)59.1449))
                                {
                                    cuotaIEPSImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto;
                                    cuotaIEPSImporteR += complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Importe;
                                }
                                else
                                {
                                    defaultImpuestoR = complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Impuesto;
                                    defaultImporteR += complementoPago.Pagos[x].DocumentosRelacionados[i].Retencion.Importe;
                                }

                                if (tasaISRImporteR > 0) { objCfdi.agregarPago20RetencionP(tasaISRImpuestoR, Convert.ToDouble(tasaISRImporteR)); }
                                if (tasaIVAImporteR > 0) { objCfdi.agregarPago20RetencionP(tasaIVAImpuestoR, Convert.ToDouble(tasaIVAImporteR)); }
                                if (cuotaIEPSImporteR > 0) { objCfdi.agregarPago20RetencionP(cuotaIEPSImpuestoR, Convert.ToDouble(cuotaIEPSImporteR)); }
                                if (defaultImporteR > 0) { objCfdi.agregarPago20RetencionP(defaultImpuestoR, Convert.ToDouble(defaultImporteR)); }

                            }
                            //Impuestos Pagos Traslado
                            var tasaIVAImpuestoT = "";
                            decimal tasaIVAImporteT = 0;
                            decimal tasaIVABaseT = 0;
                            var tasaIVATipoFactorT = "";
                            decimal tasaIVATasaOCuota = 0;
                            var cuotaIEPSImpuestoT = "";
                            decimal cuotaIEPSImporteT = 0;
                            decimal cuotaIEPSBaseT = 0;
                            var cuotaIEPSTipoFactorT = "";
                            decimal cuotaIEPSTasaOCuota = 0;
                            var defaultImpuestoT = "";
                            decimal defaultImporteT = 0;
                            decimal defaultBaseT = 0;
                            var defaultTipoFactorT = "";
                            decimal defaultTasaOCuotat = 0;

                            if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado != null && complementoPago.Pagos[x].DocumentosRelacionados[i].TrasladoDRId != null)
                            {

                                if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Impuesto == "002" && complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TipoFactor == c_TipoFactor.Tasa &&
                                     (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.0 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.16
                                     || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.08))
                                {
                                    tasaIVABaseT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Base;
                                    tasaIVATipoFactorT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TipoFactor.ToString();
                                    tasaIVATasaOCuota = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota;
                                    tasaIVAImpuestoT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Impuesto;
                                    tasaIVAImporteT += complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Importe;
                                }
                                else if (complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Impuesto == "003" && complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TipoFactor == c_TipoFactor.Tasa &&
                                     complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.265 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.3 ||
                                     complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.53 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.5 ||
                                     complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)1.6 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.304 ||
                                     complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.25 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.09 ||
                                     complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.08 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.07 ||
                                     complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.06 || complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.03 ||
                                     complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota == (decimal)0.0)
                                {
                                    cuotaIEPSBaseT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Base;
                                    cuotaIEPSTipoFactorT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TipoFactor.ToString();
                                    cuotaIEPSTasaOCuota = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota;
                                    cuotaIEPSImpuestoT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Impuesto;
                                    cuotaIEPSImporteT += complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Importe;
                                }
                                else
                                {
                                    defaultBaseT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Base;
                                    defaultTipoFactorT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TipoFactor.ToString();
                                    defaultTasaOCuotat = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.TasaOCuota;
                                    defaultImpuestoT = complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Impuesto;
                                    defaultImporteT += complementoPago.Pagos[x].DocumentosRelacionados[i].Traslado.Importe;
                                }

                                if (tasaIVAImporteT > 0) { objCfdi.agregarPago20TrasladoP(Convert.ToDouble(tasaIVABaseT), tasaIVAImpuestoT, tasaIVATipoFactorT, Convert.ToDouble(tasaIVATasaOCuota), Convert.ToDouble(tasaIVAImporteT)); }
                                if (cuotaIEPSImporteT > 0) { objCfdi.agregarPago20TrasladoP(Convert.ToDouble(cuotaIEPSBaseT), cuotaIEPSImpuestoT, cuotaIEPSTipoFactorT, Convert.ToDouble(cuotaIEPSTasaOCuota), Convert.ToDouble(cuotaIEPSImporteT)); }
                                if (defaultImporteT > 0) { objCfdi.agregarPago20TrasladoP(Convert.ToDouble(defaultBaseT), defaultImpuestoT, defaultTipoFactorT, Convert.ToDouble(defaultTasaOCuotat), Convert.ToDouble(defaultImporteT)); }

                            }
                        }
                    }

                }
                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }
                
            }
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //Genera XML
            objCfdi.GeneraXML(pathKey, passwordKey);
            string xml = objCfdi.Xml;
            //guardar string en un archivo
             System.IO.File.WriteAllText(pathXml, xml);
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
                 System.IO.File.WriteAllText(pathXml, xmlTimbrado);
            }
            else
            {
                var error = objCfdi.MensajeError;
                error = objCfdi.MensajeError;
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

            //Creamos el objeto de cancelación de la DLL.
            RVCFDI33.RVCancelacion.Cancelacion objCan = new RVCFDI33.RVCancelacion.Cancelacion();
            //Definimos la ruta en donde se guardará el XML de Solicitud de Cancelación en el disco duro.
            string ArchivoCancelacion = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCancelados//{0}-{1}-{2}.xml", complementoP.FacturaEmitida.Serie, complementoP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            //ruta temp cer y key produccion
            string cerRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.cer", complementoP.FacturaEmitida.Serie, complementoP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string keyRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.key", complementoP.FacturaEmitida.Serie, complementoP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            System.IO.File.WriteAllBytes(cerRuta, sucursal.Cer);
            System.IO.File.WriteAllBytes(keyRuta, sucursal.Key);
            //Creamos el XML de Solicitud de Cancelación.
            string folioSustitucion = (complementoPago.FolioSustitucion == null ? "" : complementoPago.FolioSustitucion);

            objCan.crearXMLCancelacionArchivo(cerRuta, keyRuta, sucursal.PasswordKey, UUID, ArchivoCancelacion, complementoPago.MotivoCancelacion, folioSustitucion);
            System.IO.File.Delete(cerRuta);
            System.IO.File.Delete(keyRuta);
            if (objCan.CodigoDeError != 0)
            {
                throw new Exception(String.Format("Ocurrió un error al crear el XML de Solicitud de Cancelación: " + objCan.MensajeDeError));
            }

            //Ejecutamos el proceso de cancelación en el Ambiente de Pruebas.
            //objCan.enviarCancelacionArchivo(ArchivoCancelacion, "fgomez", "12121212", "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", false);
            //System.IO.File.Delete(ArchivoCancelacion);
            //Ejecutamos el proceso de cancelación en el Ambiente de Producción.
            objCan.enviarCancelacionArchivo(ArchivoCancelacion, sucursal.Rfc, sucursal.Rfc, "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", true);
            System.IO.File.Delete(ArchivoCancelacion);
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
            // objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, "fgomez", "12121212", RFCEmisor);
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
                Subtotal =  0, //(double)complementoPago.TotalesPagosImpuestos.MontoTotalPagos,
                TipoCambio = complementoPago.Pagos[0].TipoCambio,
                TipoComprobante = c_TipoDeComprobante.P,
                Total =  0,//complementoPago.TotalesPagosImpuestos.MontoTotalPagos,
                Uuid = facturaDto.UUID,
                ArchivoFisicoXml = utf8.GetBytes(facturaDto.XmlTimbrado),
                CodigoQR = facturaDto.GenerarQrCode()
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

        
    }
}
