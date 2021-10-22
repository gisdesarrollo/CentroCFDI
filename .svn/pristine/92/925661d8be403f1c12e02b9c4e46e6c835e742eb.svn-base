using API.Catalogos;
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
using Utilerias.LogicaPrincipal;

namespace Aplicacion.LogicaPrincipal.GeneracionComplementosPagos
{
    public class PagosManager
    {
        #region Variables

        private static readonly Object obj = new Object();
        private readonly GenerarDto _generarDto = new GenerarDto();
        private readonly GenerarZips _generarZips = new GenerarZips();
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();
        private readonly UtileriasInfodextra _utileriasInfodextra = new UtileriasInfodextra();
        private readonly CancelacionInfodextra _cancelacionInfodextra = new CancelacionInfodextra();
        private readonly FacturacionInfodextra _facturacionInfodextra = new FacturacionInfodextra();

        #endregion

        public byte[] GenerarComplementoPago(int sucursalId, int complementoPagoId, string mailAlterno)
        {
            lock(obj)
            {
                var sucursal = _db.Sucursales.Find(sucursalId);
                var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
                var cliente = _db.Clientes.Find(complementoPago.ReceptorId);

                //Generación del comprobante
                var facturaDto = _generarDto.GenerarFactura(sucursal, cliente);
                facturaDto.ComplementoPagoDto = _generarDto.GenerarComplemento(complementoPago);
                _generarDto.CfdisRelacionados(ref facturaDto, complementoPago);

                try
                {
                    _facturacionInfodextra.GenerarXml(ref facturaDto);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                var facturaId = GuardarComprobante(facturaDto, sucursalId, complementoPago.ReceptorId);

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

                MarcarFacturado(complementoPago.Id, facturaId);

                try
                {
                    var pathXml = String.Format(@"C:\Infodextra\Temp\{0} - {1} - {2}.xml", facturaDto.Serie, facturaDto.Folio, DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    _operacionesStreams.ByteArrayArchivo(facturaDto.Xml, pathXml);
                    var pathPdf = GenerarPdf(facturaDto.Xml, complementoPago);
                    EnviarCorreo(cliente, new List<string> { pathXml, pathPdf }, mailAlterno);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("El comprobante se timbró de forma exitosa pero no fue posible mandarlo por correo electrónico, el motivo: {0}", ex.Message));
                }

                return facturaDto.Xml;
            }
        }

        #region Generacion Archivos

        public String GenerarZipComplementoPago(int complementoPagoId)
        {
            try
            {
                var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
                var path = String.Format("C:/Infodextra/Temp/{0} - {1} - {2}.zip", complementoPago.FacturaEmitida.Serie, complementoPago.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                var zip = _facturacionInfodextra.GenerarZip(complementoPago.FacturaEmitida.ArchivoFisicoXml, complementoPago.Sucursal.Logo, null);
                _operacionesStreams.ByteArrayArchivo(zip, path);
                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GenerarZipFacturaEmitida(int facturaId)
        {
            try
            {
                var facturaEmitida = _db.FacturasEmitidas.Find(facturaId);
                var pathZip = String.Format("C:/Infodextra/Temp/{0} - {1} - {2}.zip", facturaEmitida.Serie, facturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(pathZip))
                {
                    File.Delete(pathZip);
                }

                if (facturaEmitida.PathPdf == null)
                {
                    var zip = _facturacionInfodextra.GenerarZip(facturaEmitida.ArchivoFisicoXml, facturaEmitida.Emisor.Logo, null);
                    _operacionesStreams.ByteArrayArchivo(zip, pathZip);
                }
                else
                {
                    var pathXml = pathZip.Replace(".zip", ".xml");
                    _operacionesStreams.ByteArrayArchivo(facturaEmitida.ArchivoFisicoXml, pathXml);
                    _generarZips.Generar(new List<string>
                    {
                        pathXml,
                        facturaEmitida.PathPdf
                    }
                    , pathZip);
                }

                return pathZip;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GenerarZipFacturaRecibida(int facturaId)
        {
            try
            {
                var facturaRecibida = _db.FacturasRecibidas.Find(facturaId);
                var pathZip = String.Format("C:/Infodextra/Temp/{0} - {1} - {2}.zip", facturaRecibida.Serie, facturaRecibida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(pathZip))
                {
                    File.Delete(pathZip);
                }

                if (facturaRecibida.PathPdf == null)
                {
                    var zip = _facturacionInfodextra.GenerarZip(facturaRecibida.ArchivoFisicoXml, facturaRecibida.Receptor.Logo, null);
                    _operacionesStreams.ByteArrayArchivo(zip, pathZip);
                }
                else
                {
                    var pathXml = pathZip.Replace(".zip", ".xml");
                    _operacionesStreams.ByteArrayArchivo(facturaRecibida.ArchivoFisicoXml, pathXml);
                    _generarZips.Generar(new List<string>
                    {
                        pathXml,
                        facturaRecibida.PathPdf
                    }
                    , pathZip);
                }

                return pathZip;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GenerarPdf(byte[] xml, ComplementoPago complementoPago)
        {
            try
            {
                var path = String.Format("C:/Infodextra/Temp/{0} - {1} - {2}.pdf", complementoPago.FacturaEmitida.Serie, complementoPago.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                var pdf = _facturacionInfodextra.GenerarPdf(xml, complementoPago.Sucursal.Logo, null);
                _operacionesStreams.ByteArrayArchivo(pdf, path);
                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Cancelaciones

        public void Cancelar(int complementoPagoId)
        {
            var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
            try
            {
                _cancelacionInfodextra.CancelarCfdi(DTOs.Enums.Pacs.RealVirtual, complementoPago.Sucursal.Cer, complementoPago.Sucursal.Key, complementoPago.Sucursal.PasswordKey, complementoPago.Sucursal.Rfc, complementoPago.FacturaEmitida.Uuid);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se pudo cancelar el recibo con folio fiscal {0} el motivo {1}", complementoPago.FacturaEmitida.Uuid, ex.Message));
            }

            var complementosPagoCancelar = _db.ComplementosPago.Where(cp => cp.FacturaEmitidaId == complementoPago.FacturaEmitida.Id).ToList();

            foreach (var complementoPagoCancelar in complementosPagoCancelar)
            {
                MarcarNoFacturado(complementoPagoCancelar.Id);
            }
            _db.SaveChanges();
        }

        public string DescargarAcuse(int complementoPagoId)
        {
            var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
            try
            {
                var path = String.Format("C:/Infodextra/Temp/{0}.xml", DateTime.Now.ToString("yyyyMMddHHmmss"));
                var acuse = _cancelacionInfodextra.ObtenerAcuseCancelacion(DTOs.Enums.Pacs.RealVirtual, complementoPago.FacturaEmitida.Uuid, complementoPago.FacturaEmitida.Emisor.Rfc);
                _operacionesStreams.ByteArrayArchivo(acuse, path);
                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Correo Electronico

        public void EnviarCorreo(Cliente cliente, List<String> archivos, string mailAlterno)
        {
            var envioEmailDto = new EnvioEmailDto
            {
                CuerpoCorreo = cliente.Sucursal.CuerpoCorreo,
                EncabezadoCorreo = cliente.Sucursal.EncabezadoCorreo,

                EmailEmisor = cliente.Sucursal.MailEmisor,
                NombreSucursal = cliente.Sucursal.Nombre,
                EmailsReceptores = new List<string> { cliente.Email, cliente.Sucursal.MailConfirmacion, mailAlterno },
            };

            if (archivos.Count > 0)
            {
                var archivosAdjuntosDto = new List<ArchivoAdjuntoDto>();
                foreach (var archivo in archivos)
                {
                    archivosAdjuntosDto.Add(new ArchivoAdjuntoDto
                    {
                        Archivo = _operacionesStreams.ArchivoByteArray(archivo),
                        NombreArchivo = String.Format("{0}_{1}{2}", cliente.Rfc, DateTime.Now.ToString("ddMMyyyyHHmmssffff"), Path.GetExtension(archivo))
                    });
                }

                envioEmailDto.Archivos = archivosAdjuntosDto;
            }

            if (cliente.Sucursal.PasswordCorreo != null && cliente.Sucursal.Smtp != null && cliente.Sucursal.Puerto != null)
            {
                envioEmailDto.Contrasena = cliente.Sucursal.PasswordCorreo;
                envioEmailDto.Smtp = cliente.Sucursal.Smtp;
                envioEmailDto.Puerto = (int)cliente.Sucursal.Puerto;
                envioEmailDto.Ssl = cliente.Sucursal.Ssl;
                _utileriasInfodextra.EnviarCorreoTradicional(envioEmailDto);
            }
            else
            {
                _utileriasInfodextra.EnviarCorreo(envioEmailDto);
            }
        }

        #endregion

        #region Funciones Internas

        private int GuardarComprobante(FacturaDto facturaDto, int sucursalId, int clienteId)
        {
            var facturaInternaEmitida = new FacturaEmitida
            {
                ComplementosPago = new List<ComplementoPago>(),
                EmisorId = sucursalId,
                ReceptorId = clienteId,
                Fecha = facturaDto.Fecha,
                Folio = facturaDto.Folio.ToString(),
                FormaPago = facturaDto.FormaPago,
                MetodoPago = facturaDto.MetodoPago,
                Moneda = facturaDto.Moneda,
                Serie = facturaDto.Serie,
                Subtotal = facturaDto.SubtotalCalculado,
                TipoCambio = facturaDto.TipoCambio,
                TipoComprobante = facturaDto.TipoComprobante,
                Total = facturaDto.TotalCalculado,
                Uuid = facturaDto.Uuid,
                ArchivoFisicoXml = facturaDto.Xml
            };

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

        #endregion
    }
}
