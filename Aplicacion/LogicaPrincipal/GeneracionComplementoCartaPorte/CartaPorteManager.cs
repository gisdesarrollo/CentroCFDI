using API.Catalogos;
using API.Enums;
using API.Enums.CartaPorteEnums;
using API.Models.Dto;
using API.Operaciones.ComplementoCartaPorte;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.Facturacion;
using API.RelacionesCartaPorte;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.GeneracionXSA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Aplicacion.LogicaPrincipal.GeneracionComplementoCartaPorte
{
    public class CartaPorteManager
    {
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly XsaManager _xsaManager = new XsaManager();
        private static string pathXml = @"C:\TempFileCartaPorte\carta-porte31.xml";
        //private static string pathCer = @"C:\Users\kmaur\Desktop\PruebaCSD\CSD_Sucursal_1_XIA190128J61_20230518_063218.cer";
        //private static string pathCer = @"C:\inetpub\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.cer";
        //private static string pathKey = @"C:\Users\kmaur\Desktop\PruebaCSD\CSD_Sucursal_1_XIA190128J61_20230518_063218.key";
        //private static string pathKey = @"C:\inetpub\CertificadoPruebas\CSD_Pruebas_CFDI_XIA190128J61.key";
        //private static string passwordKey = "12345678a";

        public string GenerarComplementoCartaPorte(int sucursalId, int complementoCartaPorteId, string mailAlterno)
        {
            string cfdi = null;
            var sucursal = _db.Sucursales.Find(sucursalId);
            var complementoCartaPorte = _db.ComplementoCartaPortes.Find(complementoCartaPorteId);
            try
            {
                //llenado CFDI y complemento Carta Porte
                cfdi = GeneraFacturaWithXsa(complementoCartaPorte, sucursalId);
            }
            catch (Exception ex){
                throw new Exception(String.Format("Error al momento de generar el complemento: {0}", ex.Message));

            }

            return cfdi;
        }
        public string GeneraFacturaWithXsa(ComplementoCartaPorte complementoCartaPorte, int sucursalId)
        {
            
            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();

            objCfdi = LlenadoCfdi(complementoCartaPorte, sucursalId);
            return objCfdi.MensajeError;
        }

        public string GeneraFactura(ComplementoCartaPorte complementoCartaPorte, int sucursalId)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);

            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();

            objCfdi = LlenadoCfdi(complementoCartaPorte, sucursalId);
            var facturaEmitidaID = 0;
            string xml = objCfdi.Xml;
            if (objCfdi.MensajeError == "")
            {
                facturaEmitidaID = GuardarComplemento(objCfdi, complementoCartaPorte, sucursalId);

                try
                {
                    //Incrementar Folio de Sucursal
                    sucursal.FolioCartaPorte += 1;
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
                    //MarcarFacturado(complementoCartaPorte.Id, facturaEmitidaID);
                }
            }
            return xml;
        }

        
       
        private DateTime getFormatoFecha(DateTime fecha, DateTime hora)
        {
            //get fecha formateada
            DateTime fechaDocumentoCompleto = new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, hora.Second);

            return fechaDocumentoCompleto;
        }

        
        private RVCFDI33.GeneraCFDI Timbra(RVCFDI33.GeneraCFDI objCfdi,Sucursal sucursal)
        {
            //prueba
            //objCfdi.TimbrarCfdi("fgomez", "12121212", "http://generacfdi.com.mx/rvltimbrado/service1.asmx?WSDL", false);
            //produccion
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
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            return objCfdi;
        }

        private RVCFDI33.GeneraCFDI LlenadoCfdi(ComplementoCartaPorte complementoCartaPorte, int sucursalId)
        {
            string error = "";
            var sucursal = _db.Sucursales.Find(sucursalId);
            // Crea instancia
            RVCFDI33.GeneraCFDI objCfdi = new RVCFDI33.GeneraCFDI();

            // Agrega el certificado prueba
            //objCfdi.agregarCertificado(pathCer);

            // certificado de produccion
            objCfdi.agregarCertificadoBase64(System.Convert.ToBase64String(sucursal.Cer));
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }

            string tipoCambio = "";
            if (complementoCartaPorte.Moneda != null)
            {
                if (complementoCartaPorte.Moneda == API.Enums.c_Moneda.MXN)
                {
                    tipoCambio = "1";
                }
                else if (complementoCartaPorte.Moneda == API.Enums.c_Moneda.XXX)
                {
                    tipoCambio = "";
                }
                else
                {
                    tipoCambio = complementoCartaPorte.TipoCambio;
                }
            }


            // parseo Enum

            var regimenFiscal = (int)complementoCartaPorte.Sucursal.RegimenFiscal;
            objCfdi.agregarComprobante40
                   (
                       complementoCartaPorte.Sucursal.SerieCartaPorte, //Serie
                       complementoCartaPorte.Sucursal.FolioCartaPorte.ToString(), //Folio
                       getFormatoFecha(complementoCartaPorte.FechaDocumento, complementoCartaPorte.Hora).ToString("yyyy-MM-ddTHH:mm:ss"), //Fecha de emision
                       complementoCartaPorte.FormaPago ?? "",//complementoCartaPorte.FormaPago, //Forma de pago
                       complementoCartaPorte.CondicionesPago == null ? "" : complementoCartaPorte.CondicionesPago, //Condicion de pago
                       Decimal.ToDouble(complementoCartaPorte.Subtotal), //Subtotal
                       0, //Descuento
                       complementoCartaPorte.Moneda.ToString() ?? "", //Moneda
                       tipoCambio, //Tipo de cambio
                       Decimal.ToDouble(complementoCartaPorte.Total), //Total
                       complementoCartaPorte.TipoDeComprobante.ToString(), //Tipo de comprobante
                       complementoCartaPorte.MetodoPago.ToString() ?? "", //Metodo de pago
                       sucursal.CodigoPostal, //Lugar de expedicion
                       "", //Clave de confirmacion
                       complementoCartaPorte.ExportacionId //Exportacion CFDI40
                   );

            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }

            if (complementoCartaPorte.CfdiRelacionados != null)
            {
                foreach (var cfdiRelacionado in complementoCartaPorte.CfdiRelacionados)
                {
                    objCfdi.agregarCfdiRelacionados(
                        cfdiRelacionado.TipoRelacion
                    );
                    objCfdi.agregarCfdiRelacionado(
                            cfdiRelacionado.UUIDCfdiRelacionado
                        );
                }
            }
            /*if (complementoCartaPorte.UUIDCfdiRelacionado != null)
            {
                objCfdi.agregarCfdiRelacionados(complementoCartaPorte.TipoRelacion);
                objCfdi.agregarCfdiRelacionado(complementoCartaPorte.UUIDCfdiRelacionado);
            }*/
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //emisor pruebas
            //objCfdi.agregarEmisor("XIA190128J61", "XENON INDUSTRIAL ARTICLES", "601");

            objCfdi.agregarEmisor(
                complementoCartaPorte.Sucursal.Rfc, //Rfc
                complementoCartaPorte.Sucursal.RazonSocial,  //Nombre
                regimenFiscal.ToString() //Regimen fiscal
            );
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            /*pruebas receptor*/
            //objCfdi.agregarReceptor("XIA190128J61", "XENON INDUSTRIAL ARTICLES", "", "", complementoCartaPorte.UsoCfdiCP.ToString(), "26670", "601");
            var RegimeFiscalReceptor = (int)complementoCartaPorte.Receptor.RegimenFiscal;
            objCfdi.agregarReceptor(
                complementoCartaPorte.Receptor.Rfc, //Rfc
                complementoCartaPorte.Receptor.RazonSocial, //Nombre
                "",//complementoCartaPorte.Receptor.Pais.ToString(), //Residencia fiscal 
                "",//complementoCartaPorte.Receptor.NumRegIdTrib ?? "", //NumRegIdTrib
                complementoCartaPorte.UsoCfdiCP.ToString(), //UsoCFDI //para Comprobante traslado CFDI40 agregar c_UsoCfdiCP.S01.ToString()
                complementoCartaPorte.Receptor.CodigoPostal, //Domicilio Fiscal CFDI40
                RegimeFiscalReceptor.ToString() //Regimen Fiscal CFDI40
            );

            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //CONCEPTOS
            Boolean impuestoRetenido = false;
            Boolean impuestoTraladado = false;
            var conceptos = _db.Conceptos.Where(c => c.Complemento_Id == complementoCartaPorte.Id).ToList();

            if (conceptos != null)
            {
                if (conceptos.Count > 0)
                {
                    foreach (var concepto in conceptos)
                    {
                        objCfdi.agregarConcepto(
                            concepto.ClavesProdServ, //ClaveProdServ
                            concepto.NoIdentificacion, //NoIdentificacion
                             Convert.ToDouble(concepto.Cantidad), //Cantidad
                             concepto.ClavesUnidad, //ClaveUnidad
                             concepto.Unidad, //Unidad
                             concepto.Descripcion, //Descripcion
                             Convert.ToDouble(concepto.ValorUnitario), //ValorUnitario
                             Convert.ToDouble(concepto.Importe), //Importe
                             0, //Descuento
                             concepto.ObjetoImpuestoId
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

            decimal sumaImporteT = 0;
            decimal sumaImporteR = 0;

            string impuestoT = "";
            string impuestoR = "";
            string tipoFactorT = "";
            decimal tasaCuotaT = 0;
            decimal baseT = 0;
            decimal baseR = 0;
            string tipoFactorR = "";    

            if (impuestoTraladado || impuestoRetenido)
            {

                objCfdi.agregarImpuestos(Convert.ToDouble(complementoCartaPorte.TotalImpuestoRetenidos), Convert.ToDouble(complementoCartaPorte.TotalImpuestoTrasladado));


                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }
                //retenido
                foreach (var impuesto in conceptos)
                {

                    if (impuesto.Retencion != null)
                    {
                        if (impuesto.Retencion.TipoFactor == API.Enums.CartaPorteEnums.c_TipoFactor.Tasa)
                        {
                            sumaImporteR += impuesto.Retencion.Importe;
                            impuestoR = impuesto.Retencion.Impuesto;
                            tipoFactorR = impuesto.Retencion.TipoFactor.ToString();
                            baseR += impuesto.Retencion.Base;
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

                if (sumaImporteR >= 0 && baseR > 0 && tipoFactorR == "Tasa")
                {
                    objCfdi.agregarRetencion(
                                   impuestoR,
                                   Convert.ToDouble(sumaImporteR)
                                   );
                }

                //traslado
                foreach (var impuesto in conceptos)
                {
                    if (impuesto.Traslado != null)
                    {

                        if (impuesto.Traslado.TipoFactor == API.Enums.CartaPorteEnums.c_TipoFactor.Tasa)
                        {
                            sumaImporteT += impuesto.Traslado.Importe;
                            impuestoT = impuesto.Traslado.Impuesto;
                            tipoFactorT = impuesto.Traslado.TipoFactor.ToString();
                            tasaCuotaT = impuesto.Traslado.TasaOCuota;
                            baseT += impuesto.Traslado.Base;
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
                if (sumaImporteT >= 0 && baseT > 0 && tipoFactorT == "Tasa")
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
            //CARTA PORTE

            objCfdi.agregarCartaPorte31(
                complementoCartaPorte.TranspInternac ? "Sí" : "No", //TranspInternac
                complementoCartaPorte.viaEntradaSalida == null ? "" : complementoCartaPorte.EntradaSalidaMerc, //EntradaSalidaMerc
                complementoCartaPorte.PaisOrigendestino == null ? "" : complementoCartaPorte.PaisOrigendestino, //Pais Origen Destino
                complementoCartaPorte.viaEntradaSalida == null ? "" : complementoCartaPorte.viaEntradaSalida, //ViaEntradaSalida
                Convert.ToDouble(complementoCartaPorte.TotalDistRec), //TotalDistRec
                complementoCartaPorte.RegistroIstmo ? "Sí" : "", //Registro ISTMO
                complementoCartaPorte.UbicacionPoloOrigen == null ? "" : GetClaveEnum(complementoCartaPorte.UbicacionPoloOrigen), //Ubicación Polo Origen
                complementoCartaPorte.UbicacionPoloDestino == null ? "" : GetClaveEnum(complementoCartaPorte.UbicacionPoloDestino), //Ubicación Polo Destino
                complementoCartaPorte.RegimenAduanero == null ? null : new[] { complementoCartaPorte.RegimenAduanero.ToString() } //RegimenAduanero


            );

            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //UBICACIONES 

            var ubicaciones = _db.UbicacionOrigen.Where(c => c.Complemento_Id == complementoCartaPorte.Id).ToList();

            if (ubicaciones != null)
            {
                if (ubicaciones.Count > 0)
                {
                    foreach (var ubicacion in ubicaciones)
                    {
                        string residenciaFiscal = "";
                        if (ubicacion.ResidenciaFiscal != null)
                        {
                            residenciaFiscal = ubicacion.ResidenciaFiscal.ToString();
                        }
                        objCfdi.agregarCartaPorte31_Ubicacion
                         (
                             ubicacion.TipoUbicacion, //TipoUbicacion
                             ubicacion.IDUbicacion, //IDUbicacion
                             ubicacion.RfcRemitenteDestinatario, //RFCRemitenteDestinatario
                             ubicacion.NombreRemitenteDestinatario == null ? "" : ubicacion.NombreRemitenteDestinatario, //NombreRemitenteDestinatario
                             ubicacion.NumRegIdTrib == null ? "" : ubicacion.NumRegIdTrib, //NumRegIdTrib
                             residenciaFiscal, //ResidenciaFiscal
                             ubicacion.NumEstacion == null ? "" : ubicacion.NumEstacion, //NumEstacion
                             ubicacion.NombreEstacion == null ? "" : ubicacion.NombreEstacion, //NombreEstacion
                             ubicacion.NavegacionTrafico == null ? "" : ubicacion.NavegacionTrafico, //NavegacionTrafico
                             ubicacion.FechaHoraSalidaLlegada.ToString("s"), //FechaHoraSalidaLlegada
                             ubicacion.TipoEstacion_Id ?? "", //TipoEstacion
                             Convert.ToDouble(ubicacion.DistanciaRecorrida == 0 ? 0 : ubicacion.DistanciaRecorrida) //DistanciaRecorrida
                           );

                        if (objCfdi.MensajeError != "")
                        {
                            error = objCfdi.MensajeError;
                            throw new Exception(string.Join(",", error));
                        }
                        if (ubicacion.Domicilio != null)
                        {

                            objCfdi.agregarCartaPorte31_Ubicacion_Domicilio
                            (
                                ubicacion.Domicilio.Calle ?? "", //Calle
                                ubicacion.Domicilio.NumeroExterior ?? "", //NumeroExterior
                                ubicacion.Domicilio.NumeroInterior ?? "", //NumeroInterior
                                ubicacion.Domicilio.Colonia ?? "", //Colonia
                                ubicacion.Domicilio.Localidad ?? "", //Localidad
                                ubicacion.Domicilio.Referencia ?? "", //Referencia
                                ubicacion.Domicilio.Municipio ?? "", //Municipio
                                ubicacion.Domicilio.Estado ?? "", //Estado
                                ubicacion.Domicilio.Pais ?? "", //Pais
                                ubicacion.Domicilio.CodigoPostal ?? "" //CodigoPostal
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
            //MERCANCIA
            objCfdi.agregarCartaPorte31_Mercancias
            (
                Convert.ToDouble(complementoCartaPorte.Mercancias.PesoBrutoTotal == 0 ? 0: complementoCartaPorte.Mercancias.PesoBrutoTotal), //PesoBrutoTotal
                complementoCartaPorte.Mercancias.ClaveUnidadPeso_Id ?? "", //UnidadPeso
                Convert.ToDouble(complementoCartaPorte.Mercancias.PesoNetoTotal == 0 ? 0: complementoCartaPorte.Mercancias.PesoNetoTotal), //PesoNetoTotal
                complementoCartaPorte.Mercancias.NumTotalMercancias == 0 ? 0: complementoCartaPorte.Mercancias.NumTotalMercancias, //NumTotalMercancias
                Convert.ToDouble(complementoCartaPorte.Mercancias.CargoPorTasacion == 0 ? 0 : complementoCartaPorte.Mercancias.CargoPorTasacion), //CargoPorTasacion
                complementoCartaPorte.Mercancias.LogisticaInversaRecoleccionDevolucion  ? "Sí" : "" // Logistica Inversa Recolección Devolución
            );
            if (objCfdi.MensajeError != "")
            {
                error = objCfdi.MensajeError;
                throw new Exception(string.Join(",", error));
            }
            //MERCANCIA - MERCANCIAS
            var mercancias = _db.Mercancia.Where(c => c.Mercancias_Id == complementoCartaPorte.Mercancias.Id).ToList();

            if (mercancias != null)
            {
                if (mercancias.Count > 0)
                {
                   
                    foreach (var mercancia in mercancias)
                    {
                        var moneda = "";
                        //valida si existe un valorMercancia para agregar moneda
                        if (mercancia.ValorMercancia == null) { moneda = ""; } else { moneda = mercancia.Moneda.ToString(); }
                        
                        //valida clave al obtener si o no o vacio
                        var validaMPeligroso = _db.ClavesProdServCP.Where(c => c.c_ClaveUnidad == mercancia.ClaveProdServCP).FirstOrDefault();
                        string valorMaterialPeligroso="";
                        if(validaMPeligroso.MaterialPeligroso == "0,1") { valorMaterialPeligroso = mercancia.MaterialPeligrosos ? "Sí" : "No"; }
                        if(validaMPeligroso.MaterialPeligroso == "1") { valorMaterialPeligroso = mercancia.MaterialPeligrosos ? "Sí" : "No"; }
                        if(validaMPeligroso.MaterialPeligroso == "0") { valorMaterialPeligroso = mercancia.MaterialPeligrosos ? "Sí" : ""; }
                        string fechaCaducidad = mercancia.FechaCaducidad == null ? "" : mercancia.FechaCaducidad.Value.ToString("yyyy-MM-dd");
                        
                        objCfdi.agregarCartaPorte31_Mercancias_Mercancia
                        (
                            mercancia.ClaveProdServCP, //BienesTransp
                            mercancia.ClaveProdSTCC == null ? "" : mercancia.ClaveProdSTCC, //ClaveSTCC
                            mercancia.Descripcion == null ? "" : mercancia.Descripcion, //Descripcion
                            mercancia.Cantidad == 0 ? 0 : mercancia.Cantidad, //Cantidad
                            mercancia.ClavesUnidad, //ClaveUnidad
                            mercancia.Unidad == null ? "" : mercancia.Unidad, //Unidad
                            mercancia.Dimensiones == null ? "" : mercancia.Dimensiones, //Dimensiones
                            valorMaterialPeligroso, //MaterialPeligroso
                            mercancia.ClaveMaterialPeligroso == null ? "" : mercancia.ClaveMaterialPeligroso, //CveMaterialPeligroso
                            mercancia.TipoEmbalaje_Id == null ? "" : mercancia.TipoEmbalaje_Id, //Embalaje
                            mercancia.DescripEmbalaje ?? "", //DescripEmbalaje
                            Convert.ToDouble(mercancia.PesoEnKg == 0 ? 0 : mercancia.PesoEnKg), //PesoEnKg
                            Convert.ToDouble(mercancia.ValorMercancia == null ? "-1" : mercancia.ValorMercancia), //ValorMercancia. -1 Para no agregar este atributo al XML
                            moneda, //Moneda
                            mercancia.FraccionArancelarias ?? "", //FraccionArancelaria
                            mercancia.UUIDComecioExt ?? "" ,//UUIDComercioExt
                            mercancia.SectorCofepris == null ? "" : GetClaveEnum(mercancia.SectorCofepris), //sector COFEPRIS
                            mercancia.NombreIngredienteActivo ?? "", //Ubicación Polo Destino
                            mercancia.NomQuimico ?? "", //Nom Quimico
                            mercancia.DenominacionGenericaProd ?? "", //Denominacion Generica Prod
                            mercancia.DenominacionDistintivaProd ?? "", //Denominacion Distintiva Prod
                            mercancia.Fabricante ?? "", //Fabricante
                            fechaCaducidad, //Fecha Caducidad
                            mercancia.LoteMedicamento ?? "", //Lote Medicamento
                            mercancia.FormaFarmaceutica == null ? "" : GetClaveEnum(mercancia.FormaFarmaceutica), //Forma Farmaceutica
                            mercancia.CondicionesEspecialesTransp == null ? "": GetClaveEnum(mercancia.CondicionesEspecialesTransp),//Condiciones Esp Transp
                            mercancia.RegistroSanitarioFolioAutorizacion ?? "", //RegistroSanitarioFolioAutorizacion
                            mercancia.PermisoImportacion ?? "",//Permiso Importacion
                            mercancia.FolioImpoVucem ?? "", //Folio Impo VUCEM
                            mercancia.NumCas ?? "", //Num CAS
                            mercancia.RazonSocialEmpImp ?? "", //Razon Social Emp Imp
                            mercancia.NumRegSanPlagCofepris ?? "", //Num Reg San Plag COFEPRIS
                            mercancia.DatosFabricante ?? "", //Datos Fabricante
                            mercancia.DatosFormulador ?? "", //Datos Formulador
                            mercancia.DatosMaquilador ?? "", //Datos Maquilador
                            mercancia.UsoAutorizado ?? "", //Uso Autorizado
                            mercancia.TipoMateria == null ? "" : GetClaveEnum(mercancia.TipoMateria), //Tipo Materia
                            mercancia.DescripcionMateria ?? ""
                         );
                        if (objCfdi.MensajeError != "")
                        {
                            error = objCfdi.MensajeError;
                            throw new Exception(string.Join(",", error));
                        }

                        
                        var DAduaneras = _db.DocumentacionAduanera.Where(c => c.Mercancia_Id == mercancia.Id).ToList();
                        if (DAduaneras != null)
                        {
                            if (DAduaneras.Count > 0)
                            {
                                foreach (var dAduanera in DAduaneras)
                                {
                                    objCfdi.agregarCartaPorte31_Mercancias_Mercancia_DocumentacionAduanera
                                        (
                                            dAduanera.NumPedimento,
                                            GetClaveEnum(dAduanera.TipoDocumento),
                                            dAduanera.IdentDocAduanero ?? "",
                                            dAduanera.RfcImpo
                                        );
                                    if (objCfdi.MensajeError != "")
                                    {
                                        error = objCfdi.MensajeError;
                                        throw new Exception(string.Join(",", error));
                                    }
                                }

                            }
                        }

                        var gIdentificacion = _db.GuiasIdentificacion.Where(c => c.Mercancia_Id == mercancia.Id).ToList();
                        if (gIdentificacion != null)
                        {
                            if (gIdentificacion.Count > 0)
                            {
                                foreach (var mGIdentificacion in gIdentificacion)
                                {
                                    objCfdi.agregarCartaPorte31_Mercancias_Mercancia_GuiasIdentificacion
                                        (
                                        mGIdentificacion.NumeroGuiaIdentificacion ?? "",
                                        mGIdentificacion.DescripGuiaIdentificacion ?? "",
                                        Convert.ToDouble(mGIdentificacion.PesoGuiaIdentificacion == 0 ? 0 : mGIdentificacion.PesoGuiaIdentificacion)
                                        );
                                    if (objCfdi.MensajeError != "")
                                    {
                                        error = objCfdi.MensajeError;
                                        throw new Exception(string.Join(",", error));
                                    }
                                }

                            }
                        }
                        var cantidadTransportadas = _db.CantidadTransportadas.Where(c => c.Mercancia_Id == mercancia.Id).ToList();
                        if (cantidadTransportadas != null)
                        {
                            if (cantidadTransportadas.Count > 0)
                            {
                                foreach (var mCTransportada in cantidadTransportadas)
                                {
                                    objCfdi.agregarCartaPorte31_Mercancias_Mercancia_CantidadTransporta(
                                         Convert.ToDouble(mCTransportada.Cantidad), //Cantidad
                                         mCTransportada.IDOrigen, //IDOrigen
                                         mCTransportada.IDDestino, //IDDestino
                                         mCTransportada.CveTransporte_Id ?? "" //CvesTransporte
                                     );
                                    if (objCfdi.MensajeError != "")
                                    {
                                        error = objCfdi.MensajeError;
                                        throw new Exception(string.Join(",", error));
                                    }
                                }

                            }
                        }

                        

                        if (mercancia.DetalleMercancia != null)
                        {
                            objCfdi.agregarCartaPorte31_Mercancias_Mercancia_DetalleMercancia
                                (
                                mercancia.DetalleMercancia.ClaveUnidadPeso_Id,
                                Convert.ToDouble(mercancia.DetalleMercancia.PesoBruto),
                                Convert.ToDouble(mercancia.DetalleMercancia.PesoNeto),
                                Convert.ToDouble(mercancia.DetalleMercancia.PesoTara),
                                mercancia.DetalleMercancia.NumPiezas
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
            //TRANSPORTES
            //AUTOTRANSPORTE
            if (complementoCartaPorte.Mercancias.AutoTransporte != null)
            {
                objCfdi.agregarCartaPorte31_Mercancias_Autotransporte
                    (
                        complementoCartaPorte.Mercancias.AutoTransporte.TipoPermiso_Id, //PermSCT
                        complementoCartaPorte.Mercancias.AutoTransporte.NumPermisoSCT //NumPermisoSCT
                    );
                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }

                if (complementoCartaPorte.Mercancias.AutoTransporte.IdentificacionVehicular != null)
                {
                    objCfdi.agregarCartaPorte31_Mercancias_Autotransporte_IdentificacionVehicular
                        (
                            complementoCartaPorte.Mercancias.AutoTransporte.IdentificacionVehicular.ConfigAutotransporte_Id,
                            complementoCartaPorte.Mercancias.AutoTransporte.IdentificacionVehicular.PlacaVM,
                            complementoCartaPorte.Mercancias.AutoTransporte.IdentificacionVehicular.AnioModeloVM,
                            Convert.ToDouble(complementoCartaPorte.Mercancias.AutoTransporte.IdentificacionVehicular.PesoBrutoVehicular)
                        );
                    if (objCfdi.MensajeError != "")
                    {
                        error = objCfdi.MensajeError;
                        throw new Exception(string.Join(",", error));
                    }
                }
                if (complementoCartaPorte.Mercancias.AutoTransporte.Seguros != null)
                {

                    objCfdi.agregarCartaPorte31_Mercancias_Autotransporte_Seguros
                        (
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.AseguraRespCivil,
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.PolizaRespCivil ?? "",
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.AseguraMedAmbiente ?? "",
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.PolizaMedAmbiente ?? "",
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.AseguraCarga ?? "",
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.PolizaCarga ?? "",
                        Convert.ToDouble(complementoCartaPorte.Mercancias.AutoTransporte.Seguros.PrimaSeguro ?? "0" )
                        );
                    if (objCfdi.MensajeError != "")
                    {
                        error = objCfdi.MensajeError;
                        throw new Exception(string.Join(",", error));
                    }
                }
                var remolques = _db.Remolques.Where(c => c.AutoTransporte_Id == complementoCartaPorte.Mercancias.AutoTransporte.Id).ToList();

                if (remolques != null)
                {
                    if (remolques.Count > 0)
                    {

                        if (remolques.Count <= 1)
                        {
                            objCfdi.agregarCartaPorte31_Mercancias_Autotransporte_Remolques
                                    (
                                        remolques[0].SubTipoRem_Id,
                                        remolques[0].Placa,
                                        "",
                                        ""
                                    );
                            if (objCfdi.MensajeError != "")
                            {
                                error = objCfdi.MensajeError;
                                throw new Exception(string.Join(",", error));
                            }
                        }
                        if (remolques.Count > 1 && remolques.Count <= 2)
                        {
                            objCfdi.agregarCartaPorte31_Mercancias_Autotransporte_Remolques
                                   (
                                       remolques[0].SubTipoRem_Id,
                                       remolques[0].Placa,
                                       remolques[1].SubTipoRem_Id,
                                       remolques[1].Placa
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
            if (complementoCartaPorte.Mercancias.TransporteMaritimo != null)
            {
                objCfdi.agregarCartaPorte31_Mercancias_TransporteMaritimo
                    (
                        complementoCartaPorte.Mercancias.TransporteMaritimo.TipoPermiso_Id, //PermSCT
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NumPermisoSCT ?? "", //NumPermisoSCT
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NombreAseg ?? "", //NombreAseg
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NumPolizaSeguro ?? "", //NumPolizaSeguro
                        complementoCartaPorte.Mercancias.TransporteMaritimo.ConfigMaritima_Id ?? "", //TipoEmbarcacion
                        complementoCartaPorte.Mercancias.TransporteMaritimo.Matricula ?? "", //Matricula
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NumeroOMI ?? "", //NumeroOMI
                        complementoCartaPorte.Mercancias.TransporteMaritimo.AnioEmbarcacion, //AnioEmbarcacion
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NombreEmbarc ?? "", //NombreEmbarc
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NacionalidadEmbarc ?? "", //NacionalidadEmbarc
                        Convert.ToDouble(complementoCartaPorte.Mercancias.TransporteMaritimo.UnidadesDeArqBruto == 0 ? 0 : complementoCartaPorte.Mercancias.TransporteMaritimo.UnidadesDeArqBruto), //UnidadesDeArqBruto
                        complementoCartaPorte.Mercancias.TransporteMaritimo.ClaveTipoCarga_Id ?? "", //TipoCarga
                        //complementoCartaPorte.Mercancias.TransporteMaritimo.NumCerITC ?? "", //NumCertITC
                        Convert.ToDouble(complementoCartaPorte.Mercancias.TransporteMaritimo.Eslora == 0 ? 0: complementoCartaPorte.Mercancias.TransporteMaritimo.Eslora), //Eslora
                        Convert.ToDouble(complementoCartaPorte.Mercancias.TransporteMaritimo.Manga == 0 ? 0: complementoCartaPorte.Mercancias.TransporteMaritimo.Manga), //Manga
                        Convert.ToDouble(complementoCartaPorte.Mercancias.TransporteMaritimo.Calado == 0 ? 0: complementoCartaPorte.Mercancias.TransporteMaritimo.Calado), //Calado
                        Convert.ToDouble(complementoCartaPorte.Mercancias.TransporteMaritimo.Puntual == 0 ? 0: complementoCartaPorte.Mercancias.TransporteMaritimo.Puntual), //Puntual
                        complementoCartaPorte.Mercancias.TransporteMaritimo.LineaNaviera ?? "", //LineaNaviera
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NombreAgenteNaviero ?? "", //NombreAgenteNaviero
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NumAutorizacionNaviero_Id ?? "", //NumAutorizacionNaviero
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NumViaje ?? "", //NumViaje
                        complementoCartaPorte.Mercancias.TransporteMaritimo.NumConocEmbarc ?? "", //NumConocEmbarc
                        complementoCartaPorte.Mercancias.TransporteMaritimo.PermisoTempNavegacion ?? ""
                    );
                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }

                var contenedoresM = _db.ContenedoresM.Where(c => c.TransporteMaritimo_Id == complementoCartaPorte.Mercancias.TransporteMaritimo.Id).ToList();

                if (contenedoresM != null)
                {
                    if (contenedoresM.Count > 0)
                    {
                        foreach (var MContenedor in contenedoresM)
                        {
                            objCfdi.agregarCartaPorte31_Mercancias_TransporteMaritimo_Contenedor
                                (
                                MContenedor.MatriculaContenedor,
                                MContenedor.ContenedorMaritimo_Id,
                                MContenedor.NumPrecinto ?? "",
                                MContenedor.IdCCPRelacionado ?? "",
                                MContenedor.PlacaVMCCP ?? "",
                                MContenedor.FechaCertificacionCCP.ToString() ?? ""
                                );
                            if (objCfdi.MensajeError != "")
                            {
                                error = objCfdi.MensajeError;
                                throw new Exception(string.Join(",", error));
                            }

                        }

                    }
                }
                /*******************/
                var remolqueCCPs = _db.RemolqueCCP.Where(c => c.TransporteMaritimo_Id == complementoCartaPorte.Mercancias.TransporteMaritimo.Id).ToList();

                if (remolqueCCPs != null)
                {
                    if (remolqueCCPs.Count > 0)
                    {
                        foreach (var remolqueCCP in remolqueCCPs)
                        {
                            objCfdi.agregarCartaPorte31_Mercancias_TransporteMaritimo_RemolquesCCP
                                (
                                    remolqueCCP.SubTipoRemCCP,
                                    remolqueCCP.PlacaCCP,
                                    "",
                                    ""
                                );
                            if (objCfdi.MensajeError != "")
                            {
                                error = objCfdi.MensajeError;
                                throw new Exception(string.Join(",", error));
                            }

                        }

                    }
                }
                /*******************/
            }
            if (complementoCartaPorte.Mercancias.TransporteAereo != null)
            {
                objCfdi.agregarCartaPorte31_Mercancias_TransporteAereo
                    (
                        complementoCartaPorte.Mercancias.TransporteAereo.TipoPermiso_Id, //PermSCT
                        complementoCartaPorte.Mercancias.TransporteAereo.NumPermisoSCT, //NumPermisoSCT
                        complementoCartaPorte.Mercancias.TransporteAereo.MatriculaAereonave, //MatriculaAeronave
                        complementoCartaPorte.Mercancias.TransporteAereo.NombreAseg ?? "", //NombreAseg
                        complementoCartaPorte.Mercancias.TransporteAereo.NumPolizaSeguro ?? "", //NumPolizaSeguro
                        complementoCartaPorte.Mercancias.TransporteAereo.NumeroGuia ?? "", //NumeroGuia
                        complementoCartaPorte.Mercancias.TransporteAereo.LugarContrato ?? "", //LugarContrato
                        complementoCartaPorte.Mercancias.TransporteAereo.CodigoTransporteAereo_Id ?? "", //CodigoTransportista
                        complementoCartaPorte.Mercancias.TransporteAereo.RFCEmbarcador ?? "", //RFCEmbarcador
                        complementoCartaPorte.Mercancias.TransporteAereo.NumRegIdTribEmbarc ?? "", //NumRegIdTribEmbarc
                        complementoCartaPorte.Mercancias.TransporteAereo.ResidenciaFiscalEmbarc.ToString(), //ResidenciaFiscalEmbarc
                        complementoCartaPorte.Mercancias.TransporteAereo.NombreEmbarcador ?? "" //NombreEmbarcador
                    );
                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }
            }
            if (complementoCartaPorte.Mercancias.TransporteFerroviario != null)
            {
                objCfdi.agregarCartaPorte31_Mercancias_TransporteFerroviario
                   (
                       complementoCartaPorte.Mercancias.TransporteFerroviario.TipoDeServicio_Id, //TipoDeServicio
                       complementoCartaPorte.Mercancias.TransporteFerroviario.TipoDeTrafico.ToString(),
                       complementoCartaPorte.Mercancias.TransporteFerroviario.NombreAseg ?? "",
                       complementoCartaPorte.Mercancias.TransporteFerroviario.NumPolizaSeguro ?? "" //NumPolizaSeguro
                   );
                if (objCfdi.MensajeError != "")
                {
                    error = objCfdi.MensajeError;
                    throw new Exception(string.Join(",", error));
                }

                var derechosDePasos = _db.DerechoDePasos.Where(c => c.TransporteFerroviario_Id == complementoCartaPorte.Mercancias.TransporteFerroviario.Id).ToList();
                if (derechosDePasos != null)
                {
                    if (derechosDePasos.Count > 0)
                    {
                        foreach (var DPaso in derechosDePasos)
                        {
                            objCfdi.agregarCartaPorte31_Mercancias_TransporteFerroviario_DerechosDePaso
                                (
                                DPaso.TipoDerechoDePaso,
                                Convert.ToDouble(DPaso.KilometrajePagado)
                                );
                            if (objCfdi.MensajeError != "")
                            {
                                error = objCfdi.MensajeError;
                                throw new Exception(string.Join(",", error));
                            }

                        }
                    }
                }
                var carros = _db.Carros.Where(c => c.TransporteFerroviario_Id == complementoCartaPorte.Mercancias.TransporteFerroviario.Id).ToList();
                if (carros != null)
                {
                    if (carros.Count > 0)
                    {
                        foreach (var Carro in carros)
                        {
                            objCfdi.agregarCartaPorte31_Mercancias_TransporteFerroviario_Carro
                                (
                                Carro.TipoCarro_Id,
                                Carro.MatriculaCarro,
                                Carro.GuiaCarro,
                                Convert.ToDouble(Carro.ToneladasNetasCarro)
                                );
                            if (objCfdi.MensajeError != "")
                            {
                                error = objCfdi.MensajeError;
                                throw new Exception(string.Join(",", error));
                            }
                            var contenedoresC = _db.ContenedoresC.Where(c => c.Carro_Id == Carro.Id).ToList();
                            if (contenedoresC != null)
                            {
                                if (contenedoresC.Count > 0)
                                {
                                    foreach (var contenedorC in contenedoresC)
                                    {
                                        objCfdi.agregarCartaPorte31_Mercancias_TransporteFerroviario_Carro_Contenedor
                                            (
                                            contenedorC.Contenedor_Id,
                                            Convert.ToDouble(contenedorC.PesoContenedorVacio),
                                            Convert.ToDouble(contenedorC.PesoNetoMercancia)
                                            );


                                    }
                                    if (objCfdi.MensajeError != "")
                                    {
                                        error = objCfdi.MensajeError;
                                        throw new Exception(string.Join(",", error));
                                    }

                                }
                            }
                        }

                    }
                }
                
            }

            // FIGURA TRANSPORTE
            var figuraTransporte = _db.Tiposfigura.Where(c => c.Complemento_Id == complementoCartaPorte.Id).ToList();
            if (figuraTransporte != null)
            {
                if (figuraTransporte.Count > 0)
                {
                    foreach (var TFigura in figuraTransporte)
                    {
                        objCfdi.agregarCartaPorte31_TiposFigura(
                            TFigura.FiguraTransporte, //TipoFigura
                            TFigura.RFCFigura ?? "", //RFCFigura
                            TFigura.NumLicencia ?? "", //NumLicencia
                            TFigura.NombreFigura ?? "", //NombreFigura
                            TFigura.NumRegIdTribFigura ?? "", //NumRegIdTribFigura
                            TFigura.ResidenciaFiscalFigura.ToString() ?? ""//ResidenciaFiscalFigura
                    );

                        
                        var partesTransportes = _db.PartesTransporte.Where(c => c.TiposFigura_Id == TFigura.Id).ToList();
                        
                            if (partesTransportes != null)
                            {
                                if (partesTransportes.Count > 0)
                                {
                                    foreach (var pTransporte in partesTransportes)
                                    {
                                        objCfdi.agregarCartaPorte31_TiposFigura_ParteTransporte
                                            (
                                            pTransporte.ToString()
                                            );
                                    }
                                }

                            }
                        if (TFigura.Domicilio != null)
                        {
                            objCfdi.agregarCartaPorte31_TiposFigura_Domicilio
                                (
                                TFigura.Domicilio.Calle ?? "",
                                TFigura.Domicilio.NumeroExterior ?? "",
                                TFigura.Domicilio.NumeroInterior ?? "",
                                TFigura.Domicilio.Colonia ?? "",
                                TFigura.Domicilio.Localidad ?? "",
                                TFigura.Domicilio.Referencia ?? "",
                                TFigura.Domicilio.Municipio ?? "",
                                TFigura.Domicilio.Estado ?? "",
                                TFigura.Domicilio.Pais ?? "",
                                TFigura.Domicilio.CodigoPostal
                                );
                        }


                    }
                    if (objCfdi.MensajeError != "")
                    {
                        error = objCfdi.MensajeError;
                        throw new Exception(string.Join(",", error));
                    }
                }
            }

            // key pruebas
            //objCfdi.GeneraXML(pathKey, passwordKey);
            // key produccion
            objCfdi.GenerarXMLBase64(System.Convert.ToBase64String(sucursal.Key), sucursal.PasswordKey);

            string xml = objCfdi.Xml;
            objCfdi.Xml = xml;

            //guardar string en un archivo
            //System.IO.File.WriteAllText(pathXml, xml);
            //obtener IdCCP para guardar despues de timbrar
            string idCCP = GetIdCCPXml(xml);
            //objCfdi = Timbra(objCfdi, sucursal);
            //timbrado por XSA Tralix

            ComprobanteDto comprobanteDto = new ComprobanteDto() {
                SucursalId = sucursal.Id,
                RfcSucursal = sucursal.Rfc,
                ReceptorId = complementoCartaPorte.ReceptorId,
                RfcReceptor = complementoCartaPorte.Receptor.Rfc,
                FechaDocumento = complementoCartaPorte.FechaDocumento,
                Moneda = (c_Moneda)complementoCartaPorte.Moneda,
                Subtotal = (double)complementoCartaPorte.Subtotal,
                TipoCambio = Convert.ToDouble(complementoCartaPorte.TipoCambio),
                TipoComprobante = complementoCartaPorte.TipoDeComprobante,
                Total = Decimal.ToDouble(complementoCartaPorte.Total),
                FormaPago = complementoCartaPorte.FormaPago,
                Referencia = complementoCartaPorte.ReferenciaAddenda,
                TotalImpuestoTrasladado = (double)complementoCartaPorte.TotalImpuestoTrasladado,
                TotalImpuestoRetenidos = (double)complementoCartaPorte.TotalImpuestoRetenidos
           
            };
            if (complementoCartaPorte.MetodoPago != null) { comprobanteDto.MetodoPago = (c_MetodoPago)Enum.ToObject(typeof(c_MetodoPago), complementoCartaPorte.MetodoPago); }
            else { comprobanteDto.MetodoPago = null; }

            int facturaEmitidaId = _xsaManager.GenerarCFDI(xml, sucursal, sucursal.FolioCartaPorte, sucursal.SerieCartaPorte, comprobanteDto,pathXml);
            if(facturaEmitidaId > 0)
            {
                try
                {
                    //Incrementar Folio de Sucursal
                    sucursal.FolioCartaPorte += 1;
                    _db.Entry(sucursal).State = EntityState.Modified;
                    _db.SaveChanges();
                    MarcarFacturado(complementoCartaPorte.Id, facturaEmitidaId,idCCP);
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
            }
            return objCfdi;
        }

        private int GuardarComplemento(RVCFDI33.GeneraCFDI objCfdi,ComplementoCartaPorte complementoCartaPorte, int sucursalId)
        {
            var utf8 = new UTF8Encoding();
            

            var facturaInternaEmitida = new FacturaEmitida
            {
                ComplementosPago = new List<ComplementoPago>(),
                EmisorId = sucursalId,
                ReceptorId = complementoCartaPorte.Receptor.Id,
                Fecha = getFormatoFecha(complementoCartaPorte.FechaDocumento, complementoCartaPorte.Hora),
                Folio = objCfdi.Folio,
                Moneda = (API.Enums.c_Moneda)complementoCartaPorte.Moneda,
                Serie = objCfdi.Serie,
                Subtotal = (double)complementoCartaPorte.Subtotal,
                TipoCambio = Convert.ToDouble(complementoCartaPorte.TipoCambio),
                TipoComprobante = complementoCartaPorte.TipoDeComprobante,
                Total = (double)complementoCartaPorte.Total,
                TotalImpuestosTrasladados = (double)complementoCartaPorte.TotalImpuestoTrasladado,
                TotalImpuestosRetenidos = (double)complementoCartaPorte.TotalImpuestoRetenidos,
                Uuid = objCfdi.UUID,
                ArchivoFisicoXml = utf8.GetBytes(objCfdi.XmlTimbrado),
                CodigoQR = objCfdi.GenerarQrCode(),
                Status = API.Enums.Status.Activo,
                ReferenciaAddenda = complementoCartaPorte.ReferenciaAddenda
            };
            
            if (complementoCartaPorte.FormaPago != null)
            {
                facturaInternaEmitida.FormaPago = complementoCartaPorte.FormaPago;
            }
            else { facturaInternaEmitida.FormaPago = null; }
            if(complementoCartaPorte.MetodoPago != null)
            {
                facturaInternaEmitida.MetodoPago = (c_MetodoPago)Enum.ToObject(typeof(c_MetodoPago), complementoCartaPorte.MetodoPago);
            }
            else { facturaInternaEmitida.MetodoPago = null; }

            _db.FacturasEmitidas.Add(facturaInternaEmitida);
            _db.SaveChanges();

            return facturaInternaEmitida.Id;
        }

        private void MarcarFacturado(int complementoCartaPorteId, int facturaEmitidaId,string idCCP)
        {
            var complementoCartaPorte = _db.ComplementoCartaPortes.Find(complementoCartaPorteId);
            complementoCartaPorte.FacturaEmitidaId = facturaEmitidaId;
            complementoCartaPorte.Generado = true;
            complementoCartaPorte.IdCCP = idCCP;
            _db.Entry(complementoCartaPorte).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public String GenerarZipComplementoCartaPorte(int complementoPagoId)
        {
            try
            {
                var complementoCartaPorte = _db.ComplementoCartaPortes.Find(complementoPagoId);
                
                var path = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCfdiGenerados//{0} - {1} - {2}.xml", complementoCartaPorte.FacturaEmitida.Serie, complementoCartaPorte.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //guardar string en un archivo
                System.IO.File.WriteAllText(path, Encoding.UTF8.GetString(complementoCartaPorte.FacturaEmitida.ArchivoFisicoXml));
                
                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Cancelar(ComplementoCartaPorte complementoCartaP)
        {
            var complementoCP = _db.ComplementoCartaPortes.Find(complementoCartaP.Id);
            var sucursal = _db.Sucursales.Find(complementoCP.SucursalId);


            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(complementoCP.FacturaEmitida.ArchivoFisicoXml);
            string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }

            //Creamos el objeto de cancelación de la DLL.
            RVCFDI33.RVCancelacion.Cancelacion objCan = new RVCFDI33.RVCancelacion.Cancelacion();
            //Definimos la ruta en donde se guardará el XML de Solicitud de Cancelación en el disco duro.
            string ArchivoCancelacion = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCancelados//{0}-{1}-{2}.xml", complementoCP.FacturaEmitida.Serie, complementoCP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            /*****begind::produccion*****/
            //ruta temp cer y key produccion
            string cerRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.cer", complementoCP.FacturaEmitida.Serie, complementoCP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string keyRuta  = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.key", complementoCP.FacturaEmitida.Serie, complementoCP.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            System.IO.File.WriteAllBytes(cerRuta, sucursal.Cer);
            System.IO.File.WriteAllBytes(keyRuta, sucursal.Key);

            //Creamos el XML de Solicitud de Cancelación.
            string folioSustitucion = (complementoCartaP.FolioSustitucion == null ? "" : complementoCartaP.FolioSustitucion);

            objCan.crearXMLCancelacionArchivo(cerRuta, keyRuta, sucursal.PasswordKey, UUID,ArchivoCancelacion, complementoCartaP.MotivoCancelacion, folioSustitucion);
            System.IO.File.Delete(cerRuta);
            System.IO.File.Delete(keyRuta);
            /*****end::produccion*****/
            /*begin::pruebas*/
            //string folioSustitucion = (complementoCartaP.FolioSustitucion == null ? "" : complementoCartaP.FolioSustitucion);
            //objCan.crearXMLCancelacionArchivo(pathCer, pathKey, passwordKey, UUID,ArchivoCancelacion, complementoCartaP.MotivoCancelacion, folioSustitucion);
            /*end:pruebas*/
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
                MarcarNoFacturado(complementoCP.Id);
                //throw new Exception(String.Format("Proceso de cancelación finalizado con éxito."));
                
            }
            else
            {
                throw new Exception(String.Format("Ocurrió un error al cancelar el XML: " + objCan.MensajeDeError));
            }
            
        }

        public string DowloadAcuseCancelacion(ComplementoCartaPorte complementoCP)
        {
            var sucursal = _db.Sucursales.Find(complementoCP.SucursalId);
            string xmlCancelacion = null;

            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(complementoCP.FacturaEmitida.ArchivoFisicoXml);
            string RFCEmisor = LeerValorXML(CadenaXML, "Rfc", "Emisor");
            string UUID = LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }

            RVCFDI33.WSConsultasCFDReal.Service1 objConsulta = new RVCFDI33.WSConsultasCFDReal.Service1();
            RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct objCancel = new RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct();
            //objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, "fgomez", "12121212", RFCEmisor);
            //consulta a produccion
            objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, sucursal.Rfc, sucursal.Rfc, RFCEmisor);
            if(objCancel._ERROR == "")
            {
                if(objCancel.xml !=null && objCancel.xml != "")
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

        private void MarcarNoFacturado(int complementoCPId)
        {
            var complementoCP = _db.ComplementoCartaPortes.Find(complementoCPId);
            complementoCP.Status = API.Enums.Status.Cancelado;
            _db.Entry(complementoCP).State = EntityState.Modified;
            var facturaEmitida = _db.FacturasEmitidas.Find(complementoCP.FacturaEmitidaId);
            facturaEmitida.Status = API.Enums.Status.Cancelado;
            _db.Entry(facturaEmitida).State = EntityState.Modified;

            _db.SaveChanges();
        }
        private string GetClaveEnum(Enum valorEnum)
        {
            var descriptionAttribute = (DescriptionAttribute[])valorEnum
                .GetType()
                .GetField(valorEnum.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttribute.Length > 0)
            {
                string descripcion = descriptionAttribute[0].Description;
                return descripcion.Substring(0, 2);
            }
            else
            {
                return null; // Manejo de error si el atributo Description no está presente
            }
        }
        private string GetIdCCPXml(string xml)
        {
            // Cargar el archivo XML
            //XDocument xmlDoc = XDocument.Load(pathXml); // Asegúrate de especificar la ruta correcta del archivo
            //carga String xml
            XDocument xmlDoc = XDocument.Parse(xml);
            // Definir el namespace si es necesario
            XNamespace cfdi = "http://www.sat.gob.mx/cfd/4";
            XNamespace cartaporte31 = "http://www.sat.gob.mx/CartaPorte31";

            // Busca el elemento CartaPorte dentro del Complemento
            XElement cartaPorte = xmlDoc.Descendants(cartaporte31 + "CartaPorte").FirstOrDefault();

            // Obtiene el valor del atributo IdCCP
            string idCCP = cartaPorte != null ? cartaPorte.Attribute("IdCCP")?.Value : null;

            return idCCP.ToString();
        }


    }
}
