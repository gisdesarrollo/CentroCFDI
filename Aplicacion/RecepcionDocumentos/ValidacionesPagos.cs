using API.Catalogos;
using API.Context;
using API.Enums;
using API.Models.Dto;
using API.Operaciones.OperacionesProveedores;
using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using SW.Services.Authentication;
using SW.Services.Validate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aplicacion.RecepcionDocumentos
{
    public class ValidacionesPagos
    {
        #region Variables

        private readonly DataContext _db = new DataContext();
        private AuthResponse responseAutenticacion = new AuthResponse();
        private readonly ProcesaDocumentoRecibido _procesaDocumentoRecibido = new ProcesaDocumentoRecibido();

        #endregion Variables

        public void ValidaComplementoPago(DataValidar dv)
        {
            //crear socio comercial en caso de que no existe solo si el usuario no es socio comercial
            if (!dv.Usuario.esProveedor)
            {
                if (dv.SocioComercial == null)
                {
                    //crear socio comercial apartir del emisor
                    var socioComercial = new SocioComercial()
                    {
                        Rfc = dv.Cfdi.Emisor.Rfc,
                        RazonSocial = dv.Cfdi.Emisor.Nombre,
                        RegimenFiscal = (API.Enums.c_RegimenFiscal)dv.Cfdi.Emisor.RegimenFiscal,
                        CodigoPostal = dv.Cfdi.LugarExpedicion,
                        Pais = API.Enums.c_Pais.MEX,
                        SucursalId = dv.Sucursal.Id,
                        Status = Status.Activo,
                        FechaAlta = DateTime.Now,
                        GrupoId = dv.Sucursal.GrupoId,
                        Observaciones = "Socio Comercial creado automaticamente"
                    };
                    //guardar datos en base de datos
                    _db.SociosComerciales.Add(socioComercial);
                    _db.SaveChanges();
                    dv.SocioComercial = socioComercial;
                }
            }
            //revisar que el Receptor.Rfc sea igual al Rfc de la Sucursal
            if (dv.Cfdi.Receptor.Rfc != dv.Sucursal.Rfc)
            {
                throw new Exception(String.Format("El receptor de la factura {0} no coincide con el RFC de la empresa a la que estas entregando: {1}", dv.Cfdi.Receptor.Rfc, dv.Sucursal.Rfc));
            }
            // valido que por menos una ves se cargue el complemento de pago
            if(dv.DocumentoRecibidoDr.CfdiRecibidosUUID == dv.TimbreFiscalDigital.UUID)
            {
                throw new Exception("Error Validación : El complemento pago ya se encuentra cargado en el sistema");
            }
            //busco los documentos recibidos por cada documento pagado y verifico que tengan estatus pagado y aprobado
            foreach (var pago in dv.Cfdi.Pagos.Pago)
            {
                foreach (var item in pago.DoctoRelacionado)
                {
                    var exisDocPagado = _db.DocumentoPagadoDr.Where(dp => dp.Pago_Id == dv.Pago.Id && dp.UUID == item.IdDocumento).FirstOrDefault();
                    if (exisDocPagado == null)
                    {
                        throw new Exception("Error Validación : El cfdi relacionado no se encuentra cargado en el sistema");
                    }
                    var documentoRecibido = _db.DocumentosRecibidos.Where(dr => dr.CfdiRecibidosUUID == item.IdDocumento).FirstOrDefault();
                    if (documentoRecibido.EstadoComercial == c_EstadoComercial.EnRevision || documentoRecibido.EstadoComercial == c_EstadoComercial.Rechazado)
                    {
                        throw new Exception(String.Format("Error Validación : El cfdi tiene un estatus comercial {0} no valido", documentoRecibido.EstadoComercial.ToString()));
                    }
                    if (documentoRecibido.EstadoPago != c_EstadoPago.Pagado)
                    {
                        throw new Exception(String.Format("Error Validación : El cfdi tiene un estatus de pago {0} no valido", documentoRecibido.EstadoPago.ToString()));
                    }
                    
                }
            }


            //si el usuario es proveedor, revisar que el RFC del emisor de la factura sea igual al RFC asignado del socio comercial del proveedor
            if (dv.Usuario.esProveedor)
            {
                if (dv.Usuario.SocioComercialId == dv.SocioComercial.Id)
                {
                    if (dv.SocioComercial.Rfc != dv.Cfdi.Emisor.Rfc)
                    {
                        throw new Exception(String.Format("El RFC del Emisor de la factura {0} no coincide con el RFC asignado al Socio Comercial del usuario que esta subiendo el CFDi{1}", dv.Cfdi.Emisor.Rfc, dv.Usuario.SocioComercialId));
                    }
                }
            }

            if (dv.Cfdi.TipoDeComprobante != c_TipoDeComprobante.P)
            {
                throw new Exception(String.Format("El Tipo de Comprobante del CFDi cargado debe ser Pago"));
            }

            if (dv.Cfdi.Pagos.Totales.MontoTotalPagos != (decimal)dv.Pago.Total)
            {
                throw new Exception(String.Format("El complemento cargado no coincide el monto con el documento pagado"));
            }

            //validar que la factura esté emitida dentro del mes en curso
            DateTime fechaActual = DateTime.Now;
            DateTime fechaFactura = DateTime.Parse(dv.Cfdi.Fecha);
            if (dv.ConfiguracionEmpresa.RecibirFacturasMesCorriente)
            {
                if (fechaFactura.Year != fechaActual.Year || fechaFactura.Month != fechaActual.Month)
                {
                    throw new Exception(String.Format("La Fecha de emisión de la factura {0} está fuera del mes actual", fechaFactura));
                }
            }
        }

        public DataValidar ValidacionesConfiguraciones(DataValidar dv)
        {
            //Validar si se requiere validar contra el PAC de manera obligatoria
            if (dv.ConfiguracionEmpresa.ValidacionDocumentosObligatoria)
            {
                //autenticacion
                responseAutenticacion = _procesaDocumentoRecibido.GetToken();

                if (responseAutenticacion.data.token != null)
                {
                    //Valida CFDI
                    var responseValidacion = _procesaDocumentoRecibido.ValidaCfdi(responseAutenticacion.data.token, dv.Archivo.PathDestinoXml);
                    if (responseValidacion == null) { throw new Exception("Error response validación CFDI : null"); }
                    if (responseValidacion.status == "success")
                    {
                        dv.DocumentoRecibidoDr.EstadoComercial = c_EstadoComercial.Aprobado;
                        dv.DocumentoRecibidoDr.EstadoPago = c_EstadoPago.Completado;
                        dv.DocumentoRecibidoDr.Procesado = true;
                        //Para iterar la lista sobre la validacion estructura
                        List<Detail> detail1 = responseValidacion.detail;
                        StringBuilder sb = new StringBuilder();
                        var count = detail1.Count();
                        var limite = 0;

                        foreach (var detalle in detail1)
                        {
                            limite++;
                            var limiteDetail = 0;
                            foreach (var nodedetalle in detalle.detail)
                            {
                                limiteDetail++;
                                sb.AppendLine(limite + "." + limiteDetail + " " + detalle.section + ":" + nodedetalle.message + ":" + nodedetalle.messageDetail);
                                //add validaciones
                                if (limite < count)
                                {
                                    dv.DocumentoRecibidoDr.DetalleArrays.Add(limite + "." + limiteDetail + " " + detalle.section + ":" + nodedetalle.message + ":" + nodedetalle.messageDetail + "\r\n");
                                }
                                else { dv.DocumentoRecibidoDr.DetalleArrays.Add(limite + "." + limiteDetail + " " + detalle.section + ":" + nodedetalle.message + ":" + nodedetalle.messageDetail); }
                            }
                        }
                        dv.DocumentoRecibidoDr.ValidacionesDetalle = sb.ToString();
                        dv.DocumentoRecibidoDr.Validaciones = new ValidacionesDR()
                        {
                            Detalle = sb.ToString()
                        };
                        dv.DocumentoRecibidoDr.Validaciones.Fecha = DateTime.Now;
                    }
                }
            }
            else
            {
                dv.DocumentoRecibidoDr.EstadoComercial = c_EstadoComercial.Aprobado;
                dv.DocumentoRecibidoDr.EstadoPago = c_EstadoPago.Completado;
                dv.DocumentoRecibidoDr.Procesado = true;
                dv.DocumentoRecibidoDr.Validaciones.Fecha = DateTime.Now;
            }

            dv.DocumentoRecibidoDr.SocioComercialId = dv.SocioComercial.Id;
            dv.DocumentoRecibidoDr.UsuarioId = dv.Usuario.Id;
            dv.DocumentoRecibidoDr.CfdiRecibidosSerie = dv.Cfdi.Serie;
            dv.DocumentoRecibidoDr.CfdiRecibidosFolio = dv.Cfdi.Folio;
            dv.DocumentoRecibidoDr.MonedaId = dv.Cfdi.Moneda;
            dv.DocumentoRecibidoDr.FechaComprobante = Convert.ToDateTime(dv.Cfdi.Fecha);
            dv.DocumentoRecibidoDr.CfdiRecibidosUUID = dv.TimbreFiscalDigital.UUID;
            dv.DocumentoRecibidoDr.FechaEntrega = DateTime.Now;
            dv.DocumentoRecibidoDr.TipoDocumentoRecibido = c_TipoDocumentoRecibido.CFDI;
            dv.DocumentoRecibidoDr.Monto = dv.Cfdi.Pagos.Totales.MontoTotalPagos;
            dv.DocumentoRecibidoDr.PathArchivoXml = dv.Archivo.PathDestinoXml;
            dv.DocumentoRecibidoDr.PathArchivoPdf = dv.Archivo.PathDestinoPdf;

            return dv;
        }

        public class DataValidar
        {
            public ComprobanteCFDI Cfdi { get; set; }
            public PagosDR Pago { get; set; }
            public TimbreFiscalDigital TimbreFiscalDigital { get; set; }
            public Sucursal Sucursal { get; set; }
            public SocioComercial SocioComercial { get; set; }
            public Usuario Usuario { get; set; }
            public ConfiguracionesDR ConfiguracionEmpresa { get; set; }
            public DocumentosRecibidos DocumentoRecibidoDr { get; set; }
            public PathArchivosDto Archivo { get; set; }
        }
    }
}
