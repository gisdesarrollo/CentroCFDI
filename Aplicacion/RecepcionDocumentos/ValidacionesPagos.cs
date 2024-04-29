using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.RecepcionDocumentos
{
    internal class ValidacionesPagos
    {
<<<<<<< Updated upstream
=======
        #region Variables

        private readonly DataContext _db = new DataContext();
        private AuthResponse responseAutenticacion = new AuthResponse();
        private readonly ProcesaDocumentoRecibido _procesaDocumentoRecibido = new ProcesaDocumentoRecibido();

        #endregion Variables

        public String ValidaComplementoPago(PagosDR pago, ComprobanteCFDI cfdi)
        {
            StringBuilder sb = new StringBuilder();
            if (cfdi.Pagos.Pago.Length < 2)
            {
                foreach (var pagoXml in cfdi.Pagos.Pago)
                {
                    if (pago.SocioComercial.Rfc != cfdi.Emisor.Rfc)
                    {
                        sb.Append("error" + ":" + "Emisor no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Emisor coincide correctamente" + ","); }
                    if (pago.Sucursal.Rfc != cfdi.Receptor.Rfc)
                    {
                        sb.Append("error" + ":" + "Receptor no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Receptor coincide correctamente" + ","); }
                    //if (pago.DocumentoRecibido.RecibidosComprobante.FormaPago != pagoXml.FormaDePagoP)
                    //{
                    //    sb.Append("error" + ":" + "Forma de Pago no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Forma de Pago coincide correctamente" + ","); }

                    //if (pago.DocumentoRecibido.RecibidosComprobante.TipoComprobante != cfdi.TipoDeComprobante)
                    //{
                    //    sb.Append("error" + ":" + "Tipo comprobante no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Tipo comprobante coincide correctamente" + ","); }

                    //if (pago.TipoCambio != (double)pagoXml.TipoCambioP)
                    //{
                    //    sb.Append("error" + ":" + "Tipo cambio no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Tipo cambio coincide correctamente" + ","); }

                    //if (pago.Moneda != pagoXml.MonedaP)
                    //{
                    //    sb.Append("error" + ":" + "Moneda no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Moneda coincide correctamente" + ","); }

                    //if (pago.Total != (double)pagoXml.Monto)
                    //{
                    //    sb.Append("error" + ":" + "Monto no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Monto coincide correctamente" + ","); }

                    //if (pago.DocumentoRecibido.RecibidosComprobante.Serie != cfdi.Serie)
                    //{
                    //    sb.Append("error" + ":" + "Serie no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Serie coincide correctamente" + ","); }

                    //if (pago.DocumentoRecibido.RecibidosComprobante.Folio != cfdi.Folio)
                    //{
                    //    sb.Append("error" + ":" + "Folio no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Folio coincide correctamente" + ","); }

                    //if (pago.DocumentoRecibido.CfdiRecibidos_UUID != cfdi.TimbreFiscalDigital.UUID)
                    //{
                    //    sb.Append("error" + ":" + "UUID no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "UUID coincide correctamente" + ","); }
                    //DateTime fechaPagoXml = DateTime.ParseExact(pagoXml.FechaPago.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //if (pago.FechaPago.Date != fechaPagoXml.Date)
                    //{
                    //    sb.Append("error" + ":" + "Fecha pago no coincide con el complemento cargado" + ",");
                    //}
                    //else { sb.Append("ok" + ":" + "Fecha pago coincide correctamente" + ","); }
                }

            }
            return sb.ToString();
            ;
        }

        //public void ValidacionesNegocio(DataValidar dv)
        //{
        //    //crear socio comercial en caso de que no existe solo si el usuario no es socio comercial
        //    if (!dv.Usuario.esProveedor)
        //    {
        //        if (dv.SocioComercial == null)
        //        {
        //            //crear socio comercial apartir del emisor
        //            var socioComercial = new SocioComercial()
        //            {
        //                Rfc = dv.Cfdi.Emisor.Rfc,
        //                RazonSocial = dv.Cfdi.Emisor.Nombre,
        //                RegimenFiscal = (API.Enums.c_RegimenFiscal)dv.Cfdi.Emisor.RegimenFiscal,
        //                CodigoPostal = dv.Cfdi.LugarExpedicion,
        //                Pais = API.Enums.c_Pais.MEX,
        //                SucursalId = dv.Sucursal.Id,
        //                Status = Status.Activo,
        //                FechaAlta = DateTime.Now,
        //                GrupoId = dv.Sucursal.GrupoId,
        //                Observaciones = "Socio Comercial creado automaticamente"
        //            };
        //            //guardar datos en base de datos
        //            _db.SociosComerciales.Add(socioComercial);
        //            _db.SaveChanges();
        //        }
        //    }
        //    //revisar que el Receptor.Rfc sea igual al Rfc de la Sucursal
        //    if (dv.Cfdi.Receptor.Rfc != dv.Sucursal.Rfc)
        //    {
        //        throw new Exception(String.Format("El receptor de la factura {0} no coincide con el RFC de la empresa a la que estas entregando: {1}", dv.Cfdi.Receptor.Rfc, dv.Sucursal.Rfc));
        //    }

        //    //revisar que no sea una factura duplicada, y de ser así, que el UUID cargado anteriormente esté rechazado
        //    var existUUID = _db.DocumentoRecibidoDr.Where(dr => dr.CfdiRecibidos_UUID == dv.TimbreFiscalDigital.UUID).FirstOrDefault();
        //    if (existUUID != null)
        //    {
        //        if (existUUID.EstadoComercial != c_EstadoComercial.Rechazado && existUUID.EstadoPago != c_EstadoPago.Rechazado)
        //        {
        //            throw new Exception("Error Validación : El archivo ya se encuentra cargado en el sistema y no puede duplicarse. De ser necesario, debe rechazarse la solicitud anterior de este CFDi para poder subirlo nuevamente.");
        //        }
        //        else
        //        {
        //            throw new Exception(String.Format("La factura {0} ya fue cargada al sistema y está activa.", dv.TimbreFiscalDigital.UUID));
        //        }
        //    }

        //    //si el usuario es proveedor, revisar que el RFC del emisor de la factura sea igual al RFC asignado del socio comercial del proveedor
        //    if (dv.Usuario.esProveedor)
        //    {
        //        if (dv.Usuario.SocioComercialId == dv.SocioComercial.Id)
        //        {
        //            if (dv.SocioComercial.Rfc != dv.Cfdi.Emisor.Rfc)
        //            {
        //                throw new Exception(String.Format("El RFC del Emisor de la factura {0} no coincide con el RFC asignado al Socio Comercial del usuario que esta subiendo el CFDi{1}", dv.Cfdi.Emisor.Rfc, dv.Usuario.SocioComercialId));
        //            }
        //        }
        //    }

        //    //revisar que el documento cargado sea tipo I o E
        //    if (dv.Cfdi.TipoDeComprobante != c_TipoDeComprobante.I && dv.Cfdi.TipoDeComprobante != c_TipoDeComprobante.E)
        //    {
        //        throw new Exception(String.Format("El Tipo de Comprobante del CFDi cargado debe ser Ingreso o Egreso"));
        //    }
        //    if (dv.Cfdi.TipoDeComprobante == c_TipoDeComprobante.P)
        //    {
        //        throw new Exception(String.Format("El Tipo de Comprobante del CFDi que estas cargando es de Pago. Los complementos de Pago deben cargarse en la sección de Administración de Pagos"));
        //    }

        //    //validar que la factura esté emitida dentro del mes en curso
        //    DateTime fechaActual = DateTime.Now;
        //    DateTime fechaFactura = DateTime.Parse(dv.Cfdi.Fecha);
        //    if (fechaFactura.Year != fechaActual.Year || fechaFactura.Month != fechaActual.Month)
        //    {
        //        throw new Exception(String.Format("La Fecha de emisión de la factura {0} está fuera del mes actual", fechaFactura));
        //    }
        //}

        //public DataValidar ValidacionesConfiguraciones(DataValidar dv)
        //{
        //    //Validar si se requiere validar contra el PAC de manera obligatoria
        //    if (dv.ConfiguracionEmpresa.ValidacionDocumentosObligatoria)
        //    {
        //        //autenticacion
        //        responseAutenticacion = _procesaDocumentoRecibido.GetToken();

        //        if (responseAutenticacion.data.token != null)
        //        {
        //            //Valida CFDI
        //            var responseValidacion = _procesaDocumentoRecibido.ValidaCfdi(responseAutenticacion.data.token, dv.Archivo.PathDestinoXml);
        //            if (responseValidacion == null) { throw new Exception("Error response validación CFDI : null"); }
        //            if (responseValidacion.status == "success")
        //            {
        //                dv.DocumentoRecibidoDr.EstadoComercial = c_EstadoComercial.EnRevision;
        //                dv.DocumentoRecibidoDr.Procesado = true;
        //                //Para iterar la lista sobre la validacion estructura
        //                List<Detail> detail1 = responseValidacion.detail;
        //                StringBuilder sb = new StringBuilder();
        //                var count = detail1.Count();
        //                var limite = 0;

        //                foreach (var detalle in detail1)
        //                {
        //                    limite++;
        //                    var limiteDetail = 0;
        //                    foreach (var nodedetalle in detalle.detail)
        //                    {
        //                        limiteDetail++;
        //                        sb.AppendLine(limite + "." + limiteDetail + " " + detalle.section + ":" + nodedetalle.message + ":" + nodedetalle.messageDetail);
        //                        //add validaciones
        //                        if (limite < count)
        //                        {
        //                            dv.DocumentoRecibidoDr.DetalleArrays.Add(limite + "." + limiteDetail + " " + detalle.section + ":" + nodedetalle.message + ":" + nodedetalle.messageDetail + "\r\n");
        //                        }
        //                        else { dv.DocumentoRecibidoDr.DetalleArrays.Add(limite + "." + limiteDetail + " " + detalle.section + ":" + nodedetalle.message + ":" + nodedetalle.messageDetail); }
        //                    }
        //                }
        //                dv.DocumentoRecibidoDr.Validaciones_Detalle = sb.ToString();
        //                dv.DocumentoRecibidoDr.Validaciones = new ValidacionesDR()
        //                {
        //                    Detalle = sb.ToString()
        //                };
        //                dv.DocumentoRecibidoDr.Validaciones.Fecha = DateTime.Now;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        dv.DocumentoRecibidoDr.EstadoComercial = c_EstadoComercial.EnRevision;
        //        dv.DocumentoRecibidoDr.Procesado = true;
        //        dv.DocumentoRecibidoDr.Validaciones.Fecha = DateTime.Now;
        //    }

        //    dv.DocumentoRecibidoDr.SocioComercial_Id = dv.SocioComercial.Id;
        //    dv.DocumentoRecibidoDr.Usuario_Id = dv.Usuario.Id;
        //    dv.DocumentoRecibidoDr.CfdiRecibidos_Serie = dv.Cfdi.Serie;
        //    dv.DocumentoRecibidoDr.CfdiRecibidos_Folio = dv.Cfdi.Folio;
        //    dv.DocumentoRecibidoDr.Moneda_Id = dv.Cfdi.Moneda;
        //    dv.DocumentoRecibidoDr.FechaComprobante = Convert.ToDateTime(dv.Cfdi.Fecha);
        //    dv.DocumentoRecibidoDr.CfdiRecibidos_UUID = dv.TimbreFiscalDigital.UUID;
        //    dv.DocumentoRecibidoDr.FechaEntrega = DateTime.Now;
        //    dv.DocumentoRecibidoDr.TipoDocumentoRecibido = c_TipoDocumentoRecibido.CFDI;
        //    dv.DocumentoRecibidoDr.Monto = dv.Cfdi.Total;
        //    dv.DocumentoRecibidoDr.PathArchivoXml = dv.Archivo.PathDestinoXml;
        //    dv.DocumentoRecibidoDr.PathArchivoPdf = dv.Archivo.PathDestinoPdf;

        //    return dv;
        //}

        public class DataValidar
        {
            public ComprobanteCFDI Cfdi { get; set; }
            public TimbreFiscalDigital TimbreFiscalDigital { get; set; }
            public Sucursal Sucursal { get; set; }
            public SocioComercial SocioComercial { get; set; }
            public Usuario Usuario { get; set; }
            public ConfiguracionesDR ConfiguracionEmpresa { get; set; }
            public DocumentosRecibidosDR DocumentoRecibidoDr { get; set; }
            public PathArchivosDto Archivo { get; set; }
        }
>>>>>>> Stashed changes
    }
}
