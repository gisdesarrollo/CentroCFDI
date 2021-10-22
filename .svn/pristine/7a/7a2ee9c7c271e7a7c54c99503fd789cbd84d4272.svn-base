using API.Catalogos;
using API.DTOs.Pagos;
using API.Operaciones.ComplementosPagos;
using CFDI.API.Enums.CFDI33;
using DTOs.Correos;
using DTOs.Facturacion.Facturacion;
using System;
using System.Collections.Generic;

namespace Aplicacion.LogicaPrincipal.GeneracionComplementosPagos
{
    public class GenerarDto
    {
        #region Factura

        public FacturaDto GenerarFactura(Sucursal sucursal, Cliente cliente)
        {
            var facturaDto = new FacturaDto
            {
                Fecha = DateTime.Now,
                Folio = sucursal.Folio,
                Moneda = c_Moneda.XXX,
                Serie = sucursal.Serie,
                TipoComprobante = c_TipoDeComprobante.P,
                ConceptosDto = new List<ConceptoDto>(),
                EnvioEmailDto = new EnvioEmailDto
                {
                    CuerpoCorreo = "A continuación su complemento de pago, muchas gracias por su preferencia",
                    EmailEmisor = sucursal.MailEmisor,
                    EmailsReceptores = new List<String> { sucursal.MailConfirmacion, cliente.Email },
                    EncabezadoCorreo = "Complemento de Pago",
                    NombreSucursal = sucursal.Nombre
                },
                EmisorDto = new EmisorDto
                {
                    Cer = sucursal.Cer,
                    Key = sucursal.Key,
                    LugarExpedicion = sucursal.CodigoPostal,
                    PasswordKey = sucursal.PasswordKey,
                    RazonSocial = sucursal.RazonSocial,
                    RegimenFiscal = sucursal.RegimenFiscal,
                    Rfc = sucursal.Rfc,
                    DobleValidacion = true,
                    Logo = sucursal.Logo,
                    Nombre = sucursal.Nombre
                },
                ReceptorDto = new ReceptorDto
                {
                    RazonSocial = cliente.RazonSocial,
                    ResidenciaFiscal = cliente.Pais,
                    Rfc = cliente.Rfc,
                    UsoCfdi = c_UsoCFDI.PorDefinir,
                },
                Timbrado = true
            };

            var conceptoDto = new ConceptoDto
            {
                Cantidad = 1,
                ClaveProductoServicio = "84111506",
                Descripcion = "Pago",
                PrecioUnitario = 0,
                Total = 0,
                ClaveUnidadMedida = "ACT"
            };

            facturaDto.ConceptosDto.Add(conceptoDto);

            return facturaDto;
        }

        #endregion

        #region Complementos de Pago

        public ComplementoPagoDto GenerarComplemento(ComplementoPago complementoPago)
        {
            var complementoPagoDto = new ComplementoPagoDto
            {
                FechaDocumento = complementoPago.FechaDocumento,
                Sucursal = new EmisorDto
                {
                    LugarExpedicion = complementoPago.Sucursal.CodigoPostal,
                    RazonSocial = complementoPago.Sucursal.RazonSocial,
                    RegimenFiscal = complementoPago.Sucursal.RegimenFiscal,
                    Rfc = complementoPago.Sucursal.Rfc
                },
                Receptor = new ReceptorDto
                {
                    RazonSocial = complementoPago.Receptor.RazonSocial,
                    ResidenciaFiscal = complementoPago.Receptor.Pais,
                    Rfc = complementoPago.Receptor.Rfc,
                    UsoCfdi = c_UsoCFDI.PorDefinir
                }
            };

            if (complementoPago.Pagos != null)
            {
                complementoPagoDto.Pagos = new List<PagoDto>();
            }
                
            for (int i = 0; i < complementoPago.Pagos.Count; i++)
            {
                var pago = new PagoDto
                {
                    FechaPago = complementoPago.Pagos[i].FechaPago,
                    FormaPago = complementoPago.Pagos[i].FormaPago,
                    Moneda = complementoPago.Pagos[i].Moneda,
                    Monto = complementoPago.Pagos[i].Monto,
                    NombreBancoOrdenanteExtranjero = complementoPago.Pagos[i].NombreBancoOrdenanteExtranjero,
                    Notas = complementoPago.Pagos[i].Notas,
                    NumeroOperacion = complementoPago.Pagos[i].NumeroOperacion,
                    TipoCambio = complementoPago.Pagos[i].TipoCambio,

                    //SPEI
                    //TipoCadenaPago = complementoPago.Pagos[i].TipoCadenaPago,
                    //SelloPago = complementoPago.Pagos[i].SelloPago,
                    //CadenaPago = complementoPago.Pagos[i].CadenaPago,
                    //CertificadoPago = complementoPago.Pagos[i].CertificadoPago,
                };

                if (complementoPago.Pagos[i].BancoOrdenante != null)
                {
                    pago.RfcEmisorCuentaOrigen = complementoPago.Pagos[i].BancoOrdenante.Banco.Rfc;
                    pago.BancoEmisor = complementoPago.Pagos[i].BancoOrdenante.Banco.RazonSocial;
                    pago.CuentaOrdenante = complementoPago.Pagos[i].BancoOrdenante.NumeroCuenta;

                    if (!complementoPago.Pagos[i].BancoOrdenante.Banco.Nacional)
                    {
                        pago.NombreBancoOrdenanteExtranjero = complementoPago.Pagos[i].BancoOrdenante.Banco.RazonSocial;
                    }                    
                }

                if (complementoPago.Pagos[i].BancoBeneficiario != null)
                {
                    pago.RfcEmisorCuentaBeneficiario = complementoPago.Pagos[i].BancoBeneficiario.Banco.Rfc;
                    pago.BancoReceptor = complementoPago.Pagos[i].BancoBeneficiario.Banco.RazonSocial;
                    pago.CuentaBeneficiario = complementoPago.Pagos[i].BancoBeneficiario.NumeroCuenta;
                }

                if (complementoPago.Pagos[i].DocumentosRelacionados != null)
                {
                    pago.DocumentosRelacionados = new List<DocumentoRelacionadoDto>();

                    for (int j = 0; j < complementoPago.Pagos[i].DocumentosRelacionados.Count; j++)
                    {
                        var documentoRelacionado = new DocumentoRelacionadoDto
                        {
                            Folio = complementoPago.Pagos[i].DocumentosRelacionados[j].Folio,
                            IdDocumento = complementoPago.Pagos[i].DocumentosRelacionados[j].IdDocumento,
                            ImportePagado = complementoPago.Pagos[i].DocumentosRelacionados[j].ImportePagado,
                            ImporteSaldoAnterior = complementoPago.Pagos[i].DocumentosRelacionados[j].ImporteSaldoAnterior,
                            ImporteSaldoInsoluto = complementoPago.Pagos[i].DocumentosRelacionados[j].ImporteSaldoInsoluto,
                            MetodoPago = complementoPago.Pagos[i].DocumentosRelacionados[j].MetodoPago,
                            Moneda = complementoPago.Pagos[i].DocumentosRelacionados[j].Moneda,
                            NumeroParcialidad = complementoPago.Pagos[i].DocumentosRelacionados[j].NumeroParcialidad,
                            Serie = complementoPago.Pagos[i].DocumentosRelacionados[j].Serie,
                            TipoCambio = complementoPago.Pagos[i].DocumentosRelacionados[j].TipoCambio
                        };

                        pago.DocumentosRelacionados.Add(documentoRelacionado);
                    }
                }

                foreach (var documentoRelacionado in pago.DocumentosRelacionados)
                {
                    if(documentoRelacionado.Moneda == pago.Moneda)
                    {
                        documentoRelacionado.TipoCambio = 1.0;
                    }
                }

                complementoPagoDto.Pagos.Add(pago);
            }

            return complementoPagoDto;
        }

        #endregion

        #region CFDIs Relacionados

        public void CfdisRelacionados(ref FacturaDto facturaDto, ComplementoPago complementoPago)
        {
            if(complementoPago.CfdiRelacionadoId != null && complementoPago.TipoRelacion != null)
            {
                facturaDto.TipoRelacion = complementoPago.TipoRelacion;
                facturaDto.CfdisRelacionados = new List<string> { complementoPago.CfdiRelacionado.Uuid };
            }
        }

        #endregion
    }
}
