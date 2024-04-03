using API.Catalogos;
using API.Enums;
using API.Operaciones.OperacionesProveedores;
using API.Relaciones;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Correos;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.CargasMasivas.CSV
{
    public class CargaPagosDR
    {
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly EnviosEmails _envioEmail = new EnviosEmails();

        public String Exportar()
        {
            var path = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//LayoutPagos_{0}.csv", DateTime.Now.ToString("ddMMyyyyHHmmss"));

            #region Encabezados

            var encabezados = new List<String>
            {
                "*FechaPago",
                "*Moneda",
                "*Tipo de Cambio",
                "*Total del Pago",
                "*Referencia Bancaria",
                "*Referencia ERP",
                "*Cuenta Bancaria",
                "*UUID"
            };

            #endregion Encabezados

            using (var textWriter = File.CreateText(path))
            {
                using (var csv = new CsvWriter(textWriter))
                {
                    csv.WriteField(encabezados);
                }
                textWriter.Close();
            }

            return path;
        }

        public List<PagosDR> Importar(string path, int sucursalId, bool previsualizacion, int usuarioId)
        {
            var errores = new List<String>();
            var pagos = new List<PagosDR>();
            using (StreamReader archivo = File.OpenText(path))
            {
                try
                {
                    var csv = new CsvReader(archivo);

                    var registros = new List<List<String>>();
                    //var sucursal = _db.Sucursales.Find(sucursalId);
                    var time = DateTime.Now;
                    while (csv.Read())
                    {
                        var registro = new List<String>();
                        for (int i = 0; csv.TryGetField(i, out string value); i++)
                        {
                            registro.Add(value);
                        }
                        registros.Add(registro);
                    }

                    for (int i = 1; i < registros.Count(); i++)
                    {
                        try
                        {
                            var fechaPago = Convert.ToDateTime(registros[i][0]);
                            fechaPago = fechaPago.Date.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(00);
                            var moneda = ParseEnum<c_Moneda>(registros[i][1], i);
                            var tipoCambioPago = Convert.ToDouble(registros[i][2]);
                            var TotalPago = String.IsNullOrEmpty(registros[i][3]) ? 0 : Convert.ToDouble(registros[i][3]);
                            var referenciaBancaria = registros[i][4];
                            var referenciaERP = registros[i][5];
                            var cuentaBancariaNombre = registros[i][6];
                            var UUID = registros[i][7];

                            var documentoRecibido = _db.DocumentoRecibidoDr.FirstOrDefault(fe => fe.CfdiRecibidos_UUID == UUID);

                            if (documentoRecibido == null)
                            {
                                errores.Add(String.Format("El CFDi de folio fiscal {0} no fue encontrada para el registro {1}", UUID, i));
                                continue;
                            }

                            BancoSucursal banco = null;
                            if (!String.IsNullOrEmpty(cuentaBancariaNombre))
                            {
                                banco = _db.BancosSucursales.FirstOrDefault(b => b.Nombre == cuentaBancariaNombre && b.SucursalId == sucursalId);
                                if (banco == null)
                                {
                                    errores.Add(String.Format("El banco {0} no fue encontrado para el registro {1}", cuentaBancariaNombre, i));
                                    continue;
                                }
                            }
                            var documentoPagado = new DocumentosPagadosDR
                            {
                                FechaDocumento = documentoRecibido.RecibidosComprobante.Fecha,
                                Folio = documentoRecibido.CfdiRecibidos_Folio,
                                Serie = documentoRecibido.CfdiRecibidos_Serie,
                                Moneda = documentoRecibido.RecibidosComprobante.Moneda,
                                Total = documentoRecibido.RecibidosComprobante.Total,
                                UUID = UUID,
                                TipoCambio = documentoRecibido.RecibidosComprobante.TipoCambio,
                                FormaPago = documentoRecibido.RecibidosComprobante.FormaPago,
                                MetodoPago = documentoRecibido.RecibidosComprobante.MetodoPago
                            };

                            if (pagos.Any(p => p.Total == TotalPago && p.FechaPago == fechaPago && p.SocioComercial_Id == documentoRecibido.SocioComercial.Id))
                            {
                                var pago = pagos.First(p => p.Total == TotalPago && p.FechaPago == fechaPago && p.SocioComercial_Id == documentoRecibido.SocioComercial.Id);
                                if (pago != null)
                                {
                                    pago.DocumentosPagados.Add(documentoPagado);
                                }
                            }
                            else
                            {
                                var pago = new PagosDR
                                {
                                    FechaPago = fechaPago,
                                    SucursalId = sucursalId,
                                    Total = TotalPago,
                                    ComplementoPagoRecibido_Id = null,
                                    DocumentoRecibido = null,
                                    Moneda = moneda,
                                    SocioComercial_Id = documentoRecibido.SocioComercial_Id,
                                    SocioComercial = documentoRecibido.SocioComercial,
                                    TipoCambio = tipoCambioPago,
                                    CuentaBancariaSucursal_Id = banco.Id,
                                    BancoSucursal = banco,
                                    ReferenciaBancaria = referenciaBancaria,
                                    ReferenciaERP = referenciaERP
                                };

                                pago.DocumentosPagados = new List<DocumentosPagadosDR>
                                {
                                    documentoPagado
                                };
                                pagos.Add(pago);
                            }
                        }
                        catch (Exception ex)
                        {
                            errores.Add(String.Format("No se pudo procesar el registro {0} el motivo reportado: {1} </br>", i, ex.Message));
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errores.Add(ex.Message);
                }
                finally
                {
                    archivo.Close();
                    archivo.Dispose();
                }
            }

            if (errores.Count > 0)
            {
                throw new Exception(String.Join("|", errores));
            }

            if (!previsualizacion)
            {
                foreach (var pago in pagos)
                {
                    try
                    {
                        var documentosPagados = pago.DocumentosPagados;
                        pago.DocumentosPagados = null;
                        pago.DocumentoPagado = null;
                        pago.ComplementoPagoRecibido_Id = null;
                        _db.PagoDr.Add(pago);
                        _db.SaveChanges();

                        foreach (var documentoPagado in documentosPagados)
                        {
                            documentoPagado.Pagos = null;
                            documentoPagado.Pago_Id = pago.Id;
                            _db.DocumentoPagadoDr.Add(documentoPagado);

                            var docRecib = _db.DocumentoRecibidoDr.FirstOrDefault(fe => fe.CfdiRecibidos_UUID == documentoPagado.UUID);
                            //actualiza estatus pago
                            docRecib.EstadoPago = c_EstadoPago.Pagado;
                            docRecib.AprobacionesDR.FechaCargaPagos = DateTime.Now;
                            docRecib.AprobacionesDR.UsuarioCargaPagos_id = usuarioId;
                            _db.Entry(docRecib).State = EntityState.Modified;
                        }
                        _db.SaveChanges();

                        var usuarioAprobacion = ObtenerUsuarioDeAprobacionesDR(documentosPagados.FirstOrDefault().Id);
                        var socioComercial = _db.Usuarios.FirstOrDefault(u => u.Id == usuarioAprobacion).SocioComercial;
                        _envioEmail.NotificacionPagoSocioComercial(usuarioAprobacion, sucursalId, pago, socioComercial.Id);
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                errores.Add(String.Format("Propiedad: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                            }
                        }
                    }
                }
            }

            return pagos;
        }

        private T ParseEnum<T>(string valor, int registro)
        {
            var esNumero = int.TryParse(valor, out int numero);
            if (esNumero)
            {
                return (T)Enum.Parse(typeof(T), valor);
            }
            else
            {
                try
                {
                    if (String.IsNullOrEmpty(valor))
                    {
                        return default(T);
                    }
                    return (T)Enum.Parse(typeof(T), valor, true);
                }
                catch (Exception)
                {
                    throw new Exception(String.Format("No se pudo convertir el texto {0} en la propiedad {1} para el registro {2}", valor, typeof(T), registro));
                }
            }
        }

        private String ConvertFormaPagoValid(string formaPago)
        {
            String formaPagoValid = "03";

            if (formaPago.Length < 2)
            {
                switch (formaPago)
                {
                    case "1":
                        formaPagoValid = "01";
                        break;

                    case "2":
                        formaPagoValid = "02";
                        break;

                    case "3":
                        formaPagoValid = "03";
                        break;

                    case "4":
                        formaPagoValid = "04";
                        break;

                    case "5":
                        formaPagoValid = "05";
                        break;

                    case "6":
                        formaPagoValid = "06";
                        break;

                    case "8":
                        formaPagoValid = "08";
                        break;

                    default:
                        formaPagoValid = "03";
                        break;
                }
            }
            else
            {
                formaPagoValid = formaPago;
            }
            return formaPagoValid;
        }

        private int? ObtenerUsuarioDeAprobacionesDR(int documentoRecibidoId)
        {
            var usuarioAprobacion = _db.DocumentoRecibidoDr.FirstOrDefault(fe => fe.Id == documentoRecibidoId);
            return usuarioAprobacion.AprobacionesDR.UsuarioEntrega_Id;
        }
    }
}