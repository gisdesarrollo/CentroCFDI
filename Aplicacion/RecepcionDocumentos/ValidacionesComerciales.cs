using System;
using API.Catalogos;
using API.Context;
using System.Linq;
using API.Enums;
using API.Operaciones.OperacionesProveedores;
using API.Models;
using Aplicacion.LogicaPrincipal.DocumentosRecibidos;
using System.Collections.Generic;
using System.Text;
using SW.Services.Authentication;
using SW.Services.Validate;
using API.Models.Dto;
using static Aplicacion.RecepcionDocumentos.ValidacionesComerciales;

namespace Aplicacion.RecepcionDocumentos
{
    public class ValidacionesComerciales
    {
        #region Variables

        private readonly DataContext _db = new DataContext();
        private AuthResponse responseAutenticacion = new AuthResponse();
        private readonly ProcesaDocumentoRecibido _procesaDocumentoRecibido = new ProcesaDocumentoRecibido();

        #endregion Variables

        public void ValidacionesNegocio(DataValidar dv)
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
                }
            }
            //revisar que el Receptor.Rfc sea igual al Rfc de la Sucursal
            if (dv.Cfdi.Receptor.Rfc != dv.Sucursal.Rfc)
            {
                throw new Exception(String.Format("El receptor de la factura {0} no coincide con el RFC de la empresa a la que estas entregando: {1}", dv.Cfdi.Receptor.Rfc, dv.Sucursal.Rfc));
            }

            //revisar que no sea una factura duplicada, y de ser así, que el UUID cargado anteriormente esté rechazado
            var existUUID = _db.DocumentosRecibidos.Where(dr => dr.CfdiRecibidosUUID == dv.TimbreFiscalDigital.UUID).FirstOrDefault();
            if (existUUID != null)
            {
                if (existUUID.EstadoComercial != c_EstadoComercial.Rechazado && existUUID.EstadoPago != c_EstadoPago.Rechazado)
                {
                    throw new Exception("Error Validación : El archivo ya se encuentra cargado en el sistema y no puede duplicarse. De ser necesario, debe rechazarse la solicitud anterior de este CFDi para poder subirlo nuevamente.");
                }
                else
                {
                    throw new Exception(String.Format("La factura {0} ya fue cargada al sistema y está activa.", dv.TimbreFiscalDigital.UUID));
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

            //revisar que el documento cargado sea tipo I o E
            if (dv.Cfdi.TipoDeComprobante != c_TipoDeComprobante.I && dv.Cfdi.TipoDeComprobante != c_TipoDeComprobante.E)
            {
                throw new Exception(String.Format("El Tipo de Comprobante del CFDi cargado debe ser Ingreso o Egreso"));
            }
            if (dv.Cfdi.TipoDeComprobante == c_TipoDeComprobante.P)
            {
                throw new Exception(String.Format("El Tipo de Comprobante del CFDi que estas cargando es de Pago. Los complementos de Pago deben cargarse en la sección de Administración de Pagos"));
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
                        dv.DocumentoRecibidoDr.EstadoComercial = c_EstadoComercial.EnRevision;
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
                dv.DocumentoRecibidoDr.EstadoComercial = c_EstadoComercial.EnRevision;
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
            dv.DocumentoRecibidoDr.Monto = dv.Cfdi.Total;
            dv.DocumentoRecibidoDr.PathArchivoXml = dv.Archivo.PathDestinoXml;
            dv.DocumentoRecibidoDr.PathArchivoPdf = dv.Archivo.PathDestinoPdf;

            return dv;
        }

        public class DataValidar
        {
            public ComprobanteCFDI Cfdi { get; set; }
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