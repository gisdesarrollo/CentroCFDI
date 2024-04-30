using API.Enums;
using API.Enums.CartaPorteEnums;
using API.Operaciones.ComplementoCartaPorte;
using API.RelacionesCartaPorte;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones
{
    public class AcondicionarComplementosCartaPorte
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public List<ComplementoCartaPorte> Filtrar(DateTime fechaInicial, DateTime fechaFinal, c_TipoDeComprobante? tipoComprobante,string claveTransporte, bool? estatus, int sucursalId)
        {
            var complementos = new List<ComplementoCartaPorte>();
            if (estatus == null && claveTransporte!= null && tipoComprobante !=null)
            {
                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal &&
            cp.TipoDeComprobante == tipoComprobante && cp.ClaveTransporteId == claveTransporte).ToList();

            }
            else if (claveTransporte == null && estatus !=null && tipoComprobante !=null)
            {
                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal &&
            cp.TipoDeComprobante == tipoComprobante && cp.Generado == estatus).ToList();
            }
            else if (tipoComprobante == null && estatus !=null && claveTransporte !=null)
            {
                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal
             && cp.ClaveTransporteId == claveTransporte && cp.Generado == estatus).ToList();

            }
            else if (tipoComprobante == null && claveTransporte == null && estatus == null)
            {
                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal).ToList();
            }
            else if (tipoComprobante == null && claveTransporte == null)
            {
                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal &&
                cp.Generado == estatus).ToList();

            }
            else if (claveTransporte == null && estatus == null)
            {
                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal &&
            cp.TipoDeComprobante == tipoComprobante).ToList();

            }
            else if (tipoComprobante == null && estatus == null)
            {
                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal &&
             cp.ClaveTransporteId == claveTransporte).ToList();

            }
            else if (tipoComprobante != null && claveTransporte != null && estatus != null)
            {

                complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal &&
                cp.TipoDeComprobante == tipoComprobante && cp.ClaveTransporteId == claveTransporte && cp.Generado == estatus).ToList();
            }
                return complementos;
        }

        public void CargaInicial(ref ComplementoCartaPorte complementoCP)
        {
            //factura emitida null
            complementoCP.FacturaEmitida = null;
            //fecha documento
            DateTime fecha = complementoCP.FechaDocumento;
            DateTime hora = complementoCP.Hora;
            DateTime fechaDocumentoCompleto = new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, hora.Second);
            complementoCP.Hora = fechaDocumentoCompleto;

            //carga conceptos
            if (complementoCP.Conceptoss != null)
            {
                foreach (var concepto in complementoCP.Conceptoss)
                {
                    if(complementoCP.TipoDeComprobante == c_TipoDeComprobante.I)
                    {
                        complementoCP.Subtotal += (decimal)concepto.Importe;
                    }
                    concepto.ComplementoCP = null;
                    concepto.ComprobanteCfdi = null;
                    concepto.Comprobante_Id = null;
                    //concepto.ComplementoCP = complementoCP;
                    if (concepto.Retencion == null)
                    {
                       
                        concepto.Retencion = null;
                    }
                    else
                    {
                        if (concepto.Retencion.Importe > 0)
                        {
                            complementoCP.TotalImpuestoRetenidos += decimal.Round(concepto.Retencion.Importe,2);
                        }
                        else { concepto.Retencion = null; }
                    }
                    if (concepto.Traslado == null)
                    {
                        
                        concepto.Traslado = null;
                    }
                    else
                    {
                        if (concepto.Traslado.Importe >= 0 && concepto.Traslado.Base > 0)
                        {
                            complementoCP.TotalImpuestoTrasladado += decimal.Round(concepto.Traslado.Importe,2);
                        }
                        else { concepto.Traslado = null; }
                    }
                }
                complementoCP.Conceptos = null;
                
            }
            //calcula total impuestos
            if(complementoCP.TotalImpuestoRetenidos > 0 && complementoCP.TotalImpuestoTrasladado > 0)
            {
                complementoCP.Total += (complementoCP.Subtotal + complementoCP.TotalImpuestoTrasladado) - complementoCP.TotalImpuestoRetenidos;
            }else if(complementoCP.TotalImpuestoTrasladado > 0)
            {
                complementoCP.Total += complementoCP.Subtotal + complementoCP.TotalImpuestoTrasladado;
            }else if(complementoCP.TotalImpuestoRetenidos > 0)
            {
                complementoCP.Total += complementoCP.Subtotal - complementoCP.TotalImpuestoRetenidos;
            }
            else
            {
                if (complementoCP.TipoDeComprobante == c_TipoDeComprobante.I)
                {
                     complementoCP.Total = complementoCP.Subtotal;
                }
            }
            //carga ubicaciones
            if(complementoCP.Ubicaciones != null)
            {
                foreach(var ubicacion in complementoCP.Ubicaciones)
                {
                    ubicacion.ComplementoCP = null;
                    //ubicacion.ComplementoCP = complementoCP;
                    if (ubicacion.TipoUbicacion == "Destino")
                    {
                        complementoCP.TotalDistRec += ubicacion.DistanciaRecorrida;
                    }
                    if (ubicacion.RfcRemitenteDestinatario != "XEXX010101000")
                    {
                        ubicacion.NumRegIdTrib = null;
                        ubicacion.ResidenciaFiscal = null;
                    }
                    if (complementoCP.ClaveTransporteId == "01")
                    {
                        ubicacion.TipoEstacion_Id = null;
                        ubicacion.TipoEstaciones = null;
                        ubicacion.NumEstacion = null;
                        ubicacion.NavegacionTrafico = null;
                        ubicacion.NombreEstacion = null;
                    }
                }
                complementoCP.Ubicacion = null;
            }
            //carga mercancias
            if(complementoCP.Mercanciass != null)
            {
                complementoCP.Mercancias.Mercancia.Mercancias = null;
                //complementoCP.Mercancias.Mercancia.Mercancias = complementoCP.Mercancias;
                complementoCP.Mercancias.Mercancia.DocumentacionAduanera = null;
                complementoCP.Mercancias.Mercancia.GuiasIdentificacion = null;
                complementoCP.Mercancias.Mercancia.CantidadTransportada = null;
                complementoCP.Mercancias.Mercanciass = new List<Mercancia>();
                foreach (var mercancia in complementoCP.Mercanciass)
                {
                    complementoCP.Mercancias.PesoBrutoTotal += mercancia.PesoEnKg;


                    if (mercancia.DocumentacionAduaneras != null)
                    {
                        foreach (var DAduanera in mercancia.DocumentacionAduaneras)
                        {
                            DAduanera.Mercancia = null;
                            //pedimento.Mercancia = mercancia;
                            //mercancia.Pedimentoss.Add(pedimento);
                        }

                    }
                    if (mercancia.GuiasIdentificacionss != null)
                    {
                        foreach (var gIdentificacion in mercancia.GuiasIdentificacionss)
                        {
                            gIdentificacion.Mercancia = null;
                            //gIdentificacion.Mercancia = mercancia;
                            //mercancia.GuiasIdentificacionss.Add(gIdentificacion);
                        }

                    }
                    if (mercancia.CantidadTransportadass != null)
                    {
                        foreach (var cTransportada in mercancia.CantidadTransportadass)
                        {
                            cTransportada.Mercancia = null;
                            //cTransportada.Mercancia = mercancia;
                            //mercancia.CantidadTransportadass.Add(cTransportada);
                        }

                    }


                    if (mercancia.DetalleMercancia == null)
                    {
                        mercancia.DetalleMercancia = null;
                    }
                    else
                    {
                        /*if (complementoCP.ClaveTransporteId == "02" || complementoCP.ClaveTransporteId == "04")
                        {
                            complementoCP.Mercancias.PesoBrutoTotal += mercancia.DetalleMercancia.PesoBruto;
                            complementoCP.Mercancias.PesoNetoTotal += mercancia.DetalleMercancia.PesoNeto;

                        }*/
                    }
                    if (!mercancia.MaterialPeligrosos)
                    {
                        mercancia.ClaveMaterialPeligroso = null;
                        mercancia.DescripEmbalaje = null;
                        mercancia.TipoEmbalaje_Id = null;
                        mercancia.SectorCofepris = null;
                        mercancia.NombreIngredienteActivo = null;
                        mercancia.NomQuimico = null;
                        mercancia.DenominacionGenericaProd = null;
                        mercancia.DenominacionDistintivaProd = null;
                        mercancia.Fabricante = null;
                        mercancia.FechaCaducidad = null;
                        mercancia.LoteMedicamento = null;
                        mercancia.FormaFarmaceutica = null;
                        mercancia.CondicionesEspecialesTransp = null;
                        mercancia.RegistroSanitarioFolioAutorizacion = null;
                        mercancia.PermisoImportacion = null;
                        mercancia.FolioImpoVucem = null;
                        mercancia.NumCas = null;
                        mercancia.RazonSocialEmpImp = null;
                        mercancia.NumRegSanPlagCofepris = null;
                        mercancia.DatosFabricante = null;
                        mercancia.DatosFormulador = null;
                        mercancia.DatosMaquilador = null;
                        mercancia.UsoAutorizado = null;

                        complementoCP.ValidaMaterialPeligroso = false;


                    }
                    else { complementoCP.ValidaMaterialPeligroso = true; }
                   //complementoCP.Mercancias.Mercanciass = new List<Mercancia>();
                    complementoCP.Mercancias.Mercanciass.Add(mercancia);

                }
            }

            // carga transporte
            if (!complementoCP.TranspInternac)
            {
                complementoCP.EntradaSalidaMerc = null;
                complementoCP.PaisOrigendestino = null;
                complementoCP.viaEntradaSalida = null;
                complementoCP.RegimenAduanero = null;

            }
            if (complementoCP.ClaveTransporteId == "01")
            {
                complementoCP.Mercancias.AutoTransporte.Remolque = null;
                if (!complementoCP.ValidaMaterialPeligroso)
                {
                    if (complementoCP.Mercancias.AutoTransporte.Seguros != null)
                    {
                        complementoCP.Mercancias.AutoTransporte.Seguros.AseguraMedAmbiente = null;
                        complementoCP.Mercancias.AutoTransporte.Seguros.PolizaMedAmbiente = null;
                    }
                }
                if (complementoCP.Mercancias.AutoTransporte.Remolquess != null)
                {
                    foreach(var remoques in complementoCP.Mercancias.AutoTransporte.Remolquess)
                    {
                        remoques.AutoTransporte = null;
                        //remoques.AutoTransporte = complementoCP.Mercancias.AutoTransporte;
                        
                    }
                    
                   
                    
                }
                complementoCP.Mercancias.TransporteFerroviario = null;
                complementoCP.Mercancias.TransporteMaritimo = null;
                complementoCP.Mercancias.TransporteAereo = null;
            }
            else if (complementoCP.ClaveTransporteId == "02")
            {
                complementoCP.Mercancias.TransporteFerroviario = null;
                complementoCP.Mercancias.AutoTransporte = null;
                complementoCP.Mercancias.TransporteAereo = null;
                complementoCP.Mercancias.TransporteMaritimo.ContenedorM = null;
                if(complementoCP.Mercancias.TransporteMaritimo.ContenedoresM != null)
                {
                    foreach(var contenedorM in complementoCP.Mercancias.TransporteMaritimo.ContenedoresM)
                    {
                        contenedorM.TransporteMaritimo = null;
                        //contenedorM.TransporteMaritimo = complementoCP.Mercancias.TransporteMaritimo;
                    }
                }
            }
            else if (complementoCP.ClaveTransporteId == "03")
            {

                complementoCP.Mercancias.TransporteFerroviario = null;
                complementoCP.Mercancias.AutoTransporte = null;
                complementoCP.Mercancias.TransporteMaritimo = null;
                
            }
            else if (complementoCP.ClaveTransporteId == "04")
            {

                complementoCP.Mercancias.TransporteMaritimo = null;
                complementoCP.Mercancias.AutoTransporte = null;
                complementoCP.Mercancias.TransporteAereo = null;
                complementoCP.Mercancias.TransporteFerroviario.DerechosDePasos = null;
                if (complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss != null)
                {
                    foreach (var derechoDePaso in complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss)
                    {

                        derechoDePaso.TransporteFerroviario = null;
                        //derechoDePaso.TransporteFerroviario = complementoCP.Mercancias.TransporteFerroviario;
                    }

                }
                if (complementoCP.Mercancias.TransporteFerroviario.Carros != null)
                {
                    complementoCP.Mercancias.TransporteFerroviario.Carro = null;
                    foreach (var carro in complementoCP.Mercancias.TransporteFerroviario.Carros)
                    {
                        carro.TransporteFerroviario = null;
                        carro.ContenedorC = null;
                        if (carro.ContenedoresC != null)
                        {
                            foreach (var contenedorC in carro.ContenedoresC)
                            {
                                contenedorC.Carro = null;
                                
                            }
                        }
                        
                    }
                }

            }
            else
            {
                complementoCP.Mercancias.TransporteMaritimo = null;
                complementoCP.Mercancias.AutoTransporte = null;
                complementoCP.Mercancias.TransporteAereo = null;
                complementoCP.Mercancias.TransporteFerroviario = null;
                
            }

            //figura transporte
            if(complementoCP.FiguraTransporte != null)
            {
                complementoCP.TiposFigura = null;
                foreach(var figuraTransporte in complementoCP.FiguraTransporte)
                {
                    figuraTransporte.ComplementoCP = null;
                    if (figuraTransporte.Domicilio != null)
                    {
                        if(figuraTransporte.Domicilio.Pais == null)
                        {
                            figuraTransporte.Domicilio = null;
                        }

                    }
                    else { figuraTransporte.Domicilio = null; }
                    if (figuraTransporte.PartesTransportes != null)
                    {
                        figuraTransporte.PartesTransporte = null;
                        foreach (var parteTransporte in figuraTransporte.PartesTransportes)
                        {
                            parteTransporte.TiposFigura = null;
                        }
                    }
                }

            }
        }


        public void cargaRelaciones(ComplementoCartaPorte complementoCP)
        {
            //carga CfdiRelacionados
            if(complementoCP.CfdiRelacionados != null)
            {
                var idsBorrar = complementoCP.CfdiRelacionados.Select(e => e.Id);
                var CRelacionesAnteriores = _db.CfdiRelacionado.Where(es => es.ComplementoCartaPorteId == complementoCP.Id && !idsBorrar.Contains(es.Id)).ToList();
                _db.CfdiRelacionado.RemoveRange(CRelacionesAnteriores);
                _db.SaveChanges();

                foreach (var CRelacionado in complementoCP.CfdiRelacionados.Except(CRelacionesAnteriores))
                {
                    CRelacionado.ComplementoCartaPorteId = complementoCP.Id;
                    CRelacionado.ComplementoCartaPorte = null;
                    CRelacionado.ComplementoPago = null;
                    CRelacionado.ComplementoPagoId = null;
                    CRelacionado.ComprobanteCfdi = null;
                    CRelacionado.ComprobanteId = null;
                    _db.CfdiRelacionado.AddOrUpdate(CRelacionado);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                var CRelacionadosAnteriores = _db.CfdiRelacionado.Where(es => es.ComplementoCartaPorteId == complementoCP.Id).ToList();

                _db.CfdiRelacionado.RemoveRange(CRelacionadosAnteriores);
                _db.SaveChanges();
            }

            //carga conceptos
            if (complementoCP.Conceptoss != null)
            {
                var idsBorrar = complementoCP.Conceptoss.Select(e => e.Id);
                var pagosAnteriores = _db.Conceptos.Where(es => es.Complemento_Id == complementoCP.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.Conceptos.RemoveRange(pagosAnteriores);
                _db.SaveChanges();
                complementoCP.TotalImpuestoTrasladado = 0;
                complementoCP.TotalImpuestoRetenidos = 0;
                complementoCP.Subtotal = 0;
                
                foreach (var concepto in complementoCP.Conceptoss.Except(pagosAnteriores))
                {
                    if (complementoCP.TipoDeComprobante == c_TipoDeComprobante.I)
                    {
                        complementoCP.Subtotal += (decimal)concepto.Importe;
                    }
                    concepto.Complemento_Id = complementoCP.Id;
                    concepto.ComplementoCP = null;
                    concepto.ComprobanteCfdi = null;
                    concepto.Comprobante_Id = null;
                    if (concepto.Traslado == null)
                    {
                        concepto.Traslado = null;
                    }
                    else
                    {
                        if (concepto.Traslado.Importe > 0)
                        {
                            complementoCP.TotalImpuestoTrasladado += concepto.Traslado.Importe;
                        }
                        else { concepto.Traslado = null; }
                    }

                    if (concepto.Retencion == null)
                    {
                        concepto.Retencion = null;
                    }
                    else
                    {
                        if (concepto.Retencion.Importe > 0)
                        {
                            complementoCP.TotalImpuestoRetenidos += concepto.Retencion.Importe;
                        }
                        else { concepto.Retencion = null; }
                    }
                   

                    _db.Conceptos.AddOrUpdate(concepto);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }

            }
            else
            {
                var conceptosAnteriores = _db.Conceptos.Where(es => es.Complemento_Id == complementoCP.Id).ToList();

                _db.Conceptos.RemoveRange(conceptosAnteriores);
                _db.SaveChanges();
            }
            //carga Ubicaciones
            if (complementoCP.Ubicaciones != null)
            {
                var idsBorrar = complementoCP.Ubicaciones.Select(e => e.Id);
                var ubicacionesAnteriores = _db.UbicacionOrigen.Where(es => es.Complemento_Id == complementoCP.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.UbicacionOrigen.RemoveRange(ubicacionesAnteriores);
                _db.SaveChanges();
                complementoCP.TotalDistRec = 0;
                foreach (var ubicacion in complementoCP.Ubicaciones.Except(ubicacionesAnteriores))
                {
                    if (ubicacion.TipoUbicacion == "Destino")
                    {
                        complementoCP.TotalDistRec += ubicacion.DistanciaRecorrida;
                    }
                    if (ubicacion.Complemento_Id == null)
                    {
                        
                        if (ubicacion.RfcRemitenteDestinatario != "XEXX010101000")
                        {
                            ubicacion.NumRegIdTrib = null;
                            ubicacion.ResidenciaFiscal = null;
                        }
                        if (complementoCP.ClaveTransporteId == "01")
                        {
                            ubicacion.TipoEstacion_Id = null;
                            ubicacion.TipoEstaciones = null;
                            ubicacion.NumEstacion = null;
                            ubicacion.NavegacionTrafico = null;
                            ubicacion.NombreEstacion = null;
                        }
                    }
                    ubicacion.Complemento_Id = complementoCP.Id;
                    ubicacion.ComplementoCP = null;
                    _db.UbicacionOrigen.AddOrUpdate(ubicacion);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                var ubicacionAnteriores = _db.UbicacionOrigen.Where(es => es.Complemento_Id == complementoCP.Id).ToList();
                complementoCP.TotalDistRec = 0;
                _db.UbicacionOrigen.RemoveRange(ubicacionAnteriores);
                _db.SaveChanges();
            }

            //carga Figura Transporte
            if (complementoCP.FiguraTransporte != null)
            {
                var idsBorrar = complementoCP.FiguraTransporte.Select(e => e.Id);
                var figurasAnteriores = _db.Tiposfigura.Where(es => es.Complemento_Id == complementoCP.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.Tiposfigura.RemoveRange(figurasAnteriores);
                _db.SaveChanges();

                foreach (var figuraT in complementoCP.FiguraTransporte.Except(figurasAnteriores))
                {
                    figuraT.Complemento_Id = complementoCP.Id;
                    figuraT.ComplementoCP = null;
                    var copiaPartesTransporte = figuraT.PartesTransportes;
                    figuraT.PartesTransportes = null;
                    _db.Tiposfigura.AddOrUpdate(figuraT);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                    if (copiaPartesTransporte != null)
                    {
                        var idPTBorrar = copiaPartesTransporte.Select(e => e.Id);
                        var partesAnteriores = _db.PartesTransporte.Where(es => es.TiposFigura_Id == figuraT.Id && !idPTBorrar.Contains(es.Id)).ToList();

                        _db.PartesTransporte.RemoveRange(partesAnteriores);
                        _db.SaveChanges();
                        foreach (var partesT in copiaPartesTransporte.Except(partesAnteriores))
                        {
                            partesT.TiposFigura_Id = figuraT.Id;
                            partesT.TiposFigura = null;
                            _db.PartesTransporte.AddOrUpdate(partesT);
                            _db.SaveChanges();
                        }
                        copiaPartesTransporte = null;
                    }
                    else
                    {
                        var partesAnteriores = _db.PartesTransporte.Where(es => es.TiposFigura_Id == figuraT.Id).ToList();
                        _db.PartesTransporte.RemoveRange(partesAnteriores);
                        _db.SaveChanges();
                    }
                    
                }
            }
            else
            {
                var figuraAnteriores = _db.Tiposfigura.Where(es => es.Complemento_Id == complementoCP.Id).ToList();
                _db.Tiposfigura.RemoveRange(figuraAnteriores);
                _db.SaveChanges();
            }

            //carga Mercancias
            if (complementoCP.Mercancias.Mercanciass != null)
            {
                var idsMBorrar = complementoCP.Mercancias.Mercanciass.Select(e => e.Id);
                var mercanciasAnteriores = _db.Mercancia.Where(es => es.Mercancias_Id == complementoCP.Mercancias_Id && !idsMBorrar.Contains(es.Id)).ToList();

                _db.Mercancia.RemoveRange(mercanciasAnteriores);
                _db.SaveChanges();
                complementoCP.Mercancias.PesoBrutoTotal = 0;
                foreach (var mercancia in complementoCP.Mercancias.Mercanciass.Except(mercanciasAnteriores))
                {
                    complementoCP.Mercancias.PesoBrutoTotal += mercancia.PesoEnKg;
                    if (mercancia.Mercancias_Id == null)
                    {
                        
                        if (!mercancia.MaterialPeligrosos)
                        {
                            mercancia.ClaveMaterialPeligroso = null;
                            mercancia.DescripEmbalaje = null;
                            mercancia.TipoEmbalaje_Id = null;
                            mercancia.SectorCofepris = null;
                            mercancia.NombreIngredienteActivo = null;
                            mercancia.NomQuimico = null;
                            mercancia.DenominacionGenericaProd = null;
                            mercancia.DenominacionDistintivaProd = null;
                            mercancia.Fabricante = null;
                            mercancia.FechaCaducidad = null;
                            mercancia.LoteMedicamento = null;
                            mercancia.FormaFarmaceutica = null;
                            mercancia.CondicionesEspecialesTransp = null;
                            mercancia.RegistroSanitarioFolioAutorizacion = null;
                            mercancia.PermisoImportacion = null;
                            mercancia.FolioImpoVucem = null;
                            mercancia.NumCas = null;
                            mercancia.RazonSocialEmpImp = null;
                            mercancia.NumRegSanPlagCofepris = null;
                            mercancia.DatosFabricante = null;
                            mercancia.DatosFormulador = null;
                            mercancia.DatosMaquilador = null;
                            mercancia.UsoAutorizado = null;

                        }
                    }
                    mercancia.Mercancias_Id = complementoCP.Mercancias_Id;
                    mercancia.Mercancias = null;
                    

                    var copiaDocumentacionAduanera = mercancia.DocumentacionAduaneras;
                    mercancia.DocumentacionAduaneras = null;
                    var copiaGuiasIdentificacion = mercancia.GuiasIdentificacionss;
                    mercancia.GuiasIdentificacionss = null;
                    var copiaCantidadTransportada = mercancia.CantidadTransportadass;
                    mercancia.CantidadTransportada = null;
                    if (mercancia.DetalleMercancia != null && mercancia.DetalleMercanciaId == null)
                    {
                        if (mercancia.DetalleMercancia.ClaveUnidadPeso_Id == null || mercancia.DetalleMercancia.ClaveUnidadPeso_Id == "")
                        {
                            mercancia.DetalleMercancia = null;
                        }
                        else
                        {
                            /*if (complementoCP.ClaveTransporteId == "02" || complementoCP.ClaveTransporteId == "04")
                            {
                                complementoCP.Mercancias.PesoBrutoTotal += mercancia.DetalleMercancia.PesoBruto;
                                complementoCP.Mercancias.PesoNetoTotal += mercancia.DetalleMercancia.PesoNeto;

                            }*/
                        }
                    }
                    if (!mercancia.MaterialPeligrosos)
                    {
                        mercancia.ClaveMaterialPeligroso = null;
                        mercancia.DescripEmbalaje = null;
                        mercancia.TipoEmbalaje_Id = null;
                        mercancia.SectorCofepris = null;
                        mercancia.NombreIngredienteActivo = null;
                        mercancia.NomQuimico = null;
                        mercancia.DenominacionGenericaProd = null;
                        mercancia.DenominacionDistintivaProd = null;
                        mercancia.Fabricante = null;
                        mercancia.FechaCaducidad = null;
                        mercancia.LoteMedicamento = null;
                        mercancia.FormaFarmaceutica = null;
                        mercancia.CondicionesEspecialesTransp = null;
                        mercancia.RegistroSanitarioFolioAutorizacion = null;
                        mercancia.PermisoImportacion = null;
                        mercancia.FolioImpoVucem = null;
                        mercancia.NumCas = null;
                        mercancia.RazonSocialEmpImp = null;
                        mercancia.NumRegSanPlagCofepris = null;
                        mercancia.DatosFabricante = null;
                        mercancia.DatosFormulador = null;
                        mercancia.DatosMaquilador = null;
                        mercancia.UsoAutorizado = null;
                    }
                    _db.Mercancia.AddOrUpdate(mercancia);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                    if (copiaDocumentacionAduanera != null)
                    {
                        var idsPBorrar = copiaDocumentacionAduanera.Select(e => e.Id);
                        var DAduaneraAnteriores = _db.DocumentacionAduanera.Where(es => es.Mercancia_Id == mercancia.Id && !idsPBorrar.Contains(es.Id)).ToList();

                        _db.DocumentacionAduanera.RemoveRange(DAduaneraAnteriores);
                        _db.SaveChanges();
                        foreach (var DAduanera in copiaDocumentacionAduanera.Except(DAduaneraAnteriores))
                        {
                            DAduanera.Mercancia_Id = mercancia.Id;
                            DAduanera.Mercancia = null;
                            _db.DocumentacionAduanera.AddOrUpdate(DAduanera);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        var DAduaneraAnteriores = _db.DocumentacionAduanera.Where(es => es.Mercancia_Id == mercancia.Id).ToList();
                        _db.DocumentacionAduanera.RemoveRange(DAduaneraAnteriores);
                        _db.SaveChanges();
                    }
                    
                    if (copiaGuiasIdentificacion != null)
                    {
                        var idsGIBorrar = copiaGuiasIdentificacion.Select(e => e.Id);
                        var guiasIdentAnteriores = _db.GuiasIdentificacion.Where(es => es.Mercancia_Id == mercancia.Id && !idsGIBorrar.Contains(es.Id)).ToList();

                        _db.GuiasIdentificacion.RemoveRange(guiasIdentAnteriores);
                        _db.SaveChanges();
                        foreach (var guiaI in copiaGuiasIdentificacion.Except(guiasIdentAnteriores))
                        {
                            guiaI.Mercancia_Id = mercancia.Id;
                            guiaI.Mercancia = null;
                            _db.GuiasIdentificacion.AddOrUpdate(guiaI);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        var GIAnteriores = _db.GuiasIdentificacion.Where(es => es.Mercancia_Id == mercancia.Id).ToList();
                        _db.GuiasIdentificacion.RemoveRange(GIAnteriores);
                        _db.SaveChanges();
                    }
                    
                    if (copiaCantidadTransportada != null)
                    {
                        var idsCTBorrar = copiaCantidadTransportada.Select(e => e.Id);
                        var cantidadTAnteriores = _db.CantidadTransportadas.Where(es => es.Mercancia_Id == mercancia.Id && !idsCTBorrar.Contains(es.Id)).ToList();

                        _db.CantidadTransportadas.RemoveRange(cantidadTAnteriores);
                        _db.SaveChanges();
                        foreach (var cantidadT in copiaCantidadTransportada.Except(cantidadTAnteriores))
                        {
                            cantidadT.Mercancia_Id = mercancia.Id;
                            cantidadT.Mercancia = null;
                            _db.CantidadTransportadas.AddOrUpdate(cantidadT);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        var CTAnteriores = _db.CantidadTransportadas.Where(es => es.Mercancia_Id == mercancia.Id).ToList();
                        _db.CantidadTransportadas.RemoveRange(CTAnteriores);
                        _db.SaveChanges();
                    }
                    
                    
                    
                }
            }
            else
            {
                var mercanciaAnteriores = _db.Mercancia.Where(es => es.Mercancias_Id == complementoCP.Mercancias_Id).ToList();
                _db.Mercancia.RemoveRange(mercanciaAnteriores);
                _db.SaveChanges();
            }
        }

        public void cargaRemolques(ComplementoCartaPorte complementoCP)
        {
            if (complementoCP.Remolques != null && complementoCP.Mercancias.AutoTransporte.Remolquess == null)
            {
                complementoCP.Mercancias.AutoTransporte.Remolquess = new List<Remolques>();
                foreach (var remolque in complementoCP.Remolques)
                {
                    complementoCP.Mercancias.AutoTransporte.Remolquess.Add(remolque); 
                }
            }
            
            if (complementoCP.Mercancias.AutoTransporte.Remolquess != null)
            {
                var idsBorrar = complementoCP.Mercancias.AutoTransporte.Remolquess.Select(e => e.Id);
                var remolquesAnteriores = _db.Remolques.Where(es => es.AutoTransporte_Id == complementoCP.Mercancias.AutoTransporte_Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.Remolques.RemoveRange(remolquesAnteriores);
                _db.SaveChanges();

                foreach (var remolque in complementoCP.Mercancias.AutoTransporte.Remolquess.Except(remolquesAnteriores))
                {
                    remolque.AutoTransporte_Id = complementoCP.Mercancias.AutoTransporte_Id;
                    remolque.AutoTransporte = null;
                    
                    _db.Remolques.AddOrUpdate(remolque);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                var remolqueAnteriores = _db.Remolques.Where(es => es.AutoTransporte_Id == complementoCP.Mercancias.AutoTransporte_Id).ToList();

                _db.Remolques.RemoveRange(remolqueAnteriores);
                _db.SaveChanges();
            }
        }

        public void cargaContenedoresM(ComplementoCartaPorte complementoCP)
        {
            if (complementoCP.ContenedoresM != null && complementoCP.Mercancias.TransporteMaritimo.ContenedoresM == null)
            {
                complementoCP.Mercancias.TransporteMaritimo.ContenedoresM = new List<ContenedorM>();
                foreach (var contenedorM in complementoCP.ContenedoresM)
                {
                    complementoCP.Mercancias.TransporteMaritimo.ContenedoresM.Add(contenedorM);
                }
            }
            if (complementoCP.Mercancias.TransporteMaritimo.ContenedoresM != null)
            {
                var idsBorrar = complementoCP.Mercancias.TransporteMaritimo.ContenedoresM.Select(e => e.Id);
                var contenedorAnteriores = _db.ContenedoresM.Where(es => es.TransporteMaritimo_Id == complementoCP.Mercancias.TransporteMaritimo_Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.ContenedoresM.RemoveRange(contenedorAnteriores);
                _db.SaveChanges();

                foreach (var contenedorM in complementoCP.Mercancias.TransporteMaritimo.ContenedoresM.Except(contenedorAnteriores))
                {
                    contenedorM.TransporteMaritimo_Id = complementoCP.Mercancias.TransporteMaritimo_Id;
                    contenedorM.TransporteMaritimo = null;
                    _db.ContenedoresM.AddOrUpdate(contenedorM);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                var contenedorMAnteriores = _db.ContenedoresM.Where(es => es.TransporteMaritimo_Id == complementoCP.Mercancias.TransporteMaritimo_Id).ToList();

                _db.ContenedoresM.RemoveRange(contenedorMAnteriores);
                _db.SaveChanges();
            }
        }
        /*************************/
        public void cargaRemolqueCCP(ComplementoCartaPorte complementoCP)
        {
            if (complementoCP.RemolqueCCPs != null && complementoCP.Mercancias.TransporteMaritimo.RemolqueCCPs == null)
            {
                complementoCP.Mercancias.TransporteMaritimo.RemolqueCCPs = new List<RemolqueCCP>();
                foreach (var remolqueCCP in complementoCP.RemolqueCCPs)
                {
                    complementoCP.Mercancias.TransporteMaritimo.RemolqueCCPs.Add(remolqueCCP);
                }
            }
            if (complementoCP.Mercancias.TransporteMaritimo.RemolqueCCPs != null)
            {
                var idsBorrar = complementoCP.Mercancias.TransporteMaritimo.RemolqueCCPs.Select(e => e.Id);
                var remolqueCPCPAnteriores = _db.RemolqueCCP.Where(es => es.TransporteMaritimo_Id == complementoCP.Mercancias.TransporteMaritimo_Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.RemolqueCCP.RemoveRange(remolqueCPCPAnteriores);
                _db.SaveChanges();

                foreach (var remolqueCCP in complementoCP.Mercancias.TransporteMaritimo.RemolqueCCPs.Except(remolqueCPCPAnteriores))
                {
                    remolqueCCP.TransporteMaritimo_Id = complementoCP.Mercancias.TransporteMaritimo_Id;
                    remolqueCCP.TransporteMaritimo = null;
                    _db.RemolqueCCP.AddOrUpdate(remolqueCCP);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                var remolqueCCPAnteriores = _db.RemolqueCCP.Where(es => es.TransporteMaritimo_Id == complementoCP.Mercancias.TransporteMaritimo_Id).ToList();

                _db.RemolqueCCP.RemoveRange(remolqueCCPAnteriores);
                _db.SaveChanges();
            }
        }

        /************************/
        public void cargaDerechoPaso(ComplementoCartaPorte complementoCP)
        {
            if (complementoCP.DerechosDePasoss != null && complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss == null)
            {
                complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss = new List<DerechosDePasos>();
                foreach (var derechoPaso in complementoCP.DerechosDePasoss)
                {
                    complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss.Add(derechoPaso);
                }
            }
            if (complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss != null)
            {
                var idsBorrar = complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss.Select(e => e.Id);
                var derechoPasoAnteriores = _db.DerechoDePasos.Where(es => es.TransporteFerroviario_Id == complementoCP.Mercancias.TransporteFerroviario_Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.DerechoDePasos.RemoveRange(derechoPasoAnteriores);
                _db.SaveChanges();

                foreach (var derechoP in complementoCP.Mercancias.TransporteFerroviario.DerechosDePasoss.Except(derechoPasoAnteriores))
                {
                    derechoP.TransporteFerroviario_Id = complementoCP.Mercancias.TransporteFerroviario_Id;
                    derechoP.TransporteFerroviario = null;
                    

                    _db.DerechoDePasos.AddOrUpdate(derechoP);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                var derechoPAnteriores = _db.DerechoDePasos.Where(es => es.TransporteFerroviario_Id == complementoCP.Mercancias.TransporteFerroviario_Id).ToList();

                _db.DerechoDePasos.RemoveRange(derechoPAnteriores);
                _db.SaveChanges();
            }
        }
        public void cargaCarro(ComplementoCartaPorte complementoCP)
        {
            if (complementoCP.Carros != null && complementoCP.Mercancias.TransporteFerroviario.Carros == null)
            {
                complementoCP.Mercancias.TransporteFerroviario.Carros = new List<Carro>();
                foreach (var carro in complementoCP.Carros)
                {
                    complementoCP.Mercancias.TransporteFerroviario.Carros.Add(carro);
                }
            }
            if (complementoCP.Mercancias.TransporteFerroviario.Carros != null)
            {
                var idsBorrar = complementoCP.Mercancias.TransporteFerroviario.Carros.Select(e => e.Id);
                var carroAnteriores = _db.Carros.Where(es => es.TransporteFerroviario_Id == complementoCP.Mercancias.TransporteFerroviario_Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.Carros.RemoveRange(carroAnteriores);
                _db.SaveChanges();

                foreach (var carro in complementoCP.Mercancias.TransporteFerroviario.Carros.Except(carroAnteriores))
                {
                    carro.TransporteFerroviario_Id = complementoCP.Mercancias.TransporteFerroviario_Id;
                    carro.TransporteFerroviario = null;
                    if(carro.ContenedoresC != null)
                    {
                        var idsCBorrar = carro.ContenedoresC.Select(e => e.Id);
                        var contenedorCAnteriores = _db.ContenedoresC.Where(es => es.Carro_Id == carro.Id && !idsCBorrar.Contains(es.Id)).ToList();

                        _db.ContenedoresC.RemoveRange(contenedorCAnteriores);
                        _db.SaveChanges();
                        foreach(var contenedorC in carro.ContenedoresC.Except(contenedorCAnteriores))
                        {
                            contenedorC.Carro_Id = carro.Id;
                            contenedorC.Carro = null;
                            _db.ContenedoresC.AddOrUpdate(contenedorC);
                        }

                    }
                    else
                    {
                        var contenedorCAnteriores = _db.ContenedoresC.Where(es => es.Carro_Id == carro.Id).ToList();

                        _db.ContenedoresC.RemoveRange(contenedorCAnteriores);
                        _db.SaveChanges();
                    }
                    _db.Carros.AddOrUpdate(carro);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                var carroAnteriores = _db.Carros.Where(es => es.TransporteFerroviario_Id == complementoCP.Mercancias.TransporteFerroviario_Id).ToList();

                _db.Carros.RemoveRange(carroAnteriores);
                _db.SaveChanges();
            }
        }
        public void cargaValidaciones(ref ComplementoCartaPorte complementoCP)
        {
            if (complementoCP.TipoDeComprobante == c_TipoDeComprobante.T)
            {
                complementoCP.hidden = false;
                complementoCP.Moneda = c_Moneda.XXX;
                complementoCP.Subtotal = 0;
                complementoCP.Total = 0;
                complementoCP.UsoCfdiCP = c_UsoCfdiCP.S01;
                complementoCP.FormaPago = null;
                complementoCP.MetodoPago = null;
                complementoCP.TipoCambio = null;
                complementoCP.CondicionesPago = null;

            }
            complementoCP.Total = 0;
            //calcula total impuestos
            if (complementoCP.TotalImpuestoRetenidos > 0 && complementoCP.TotalImpuestoTrasladado > 0)
            {
                complementoCP.Total += (complementoCP.Subtotal + complementoCP.TotalImpuestoTrasladado) - complementoCP.TotalImpuestoRetenidos;
            }
            else if (complementoCP.TotalImpuestoTrasladado > 0)
            {
                complementoCP.Total += complementoCP.Subtotal + complementoCP.TotalImpuestoTrasladado;
            }
            else if (complementoCP.TotalImpuestoRetenidos > 0)
            {
                complementoCP.Total += complementoCP.Subtotal - complementoCP.TotalImpuestoRetenidos;
            }
            else
            {
                if(complementoCP.TipoDeComprobante == c_TipoDeComprobante.I)
                {
                    complementoCP.Total = complementoCP.Subtotal;
                }
            }
        }

        public String GeneraIdCCP()
        {
            Guid nuevoGuid = Guid.NewGuid();

            // Convertir el GUID a una cadena
            string guidString = nuevoGuid.ToString();
            string resultado = "CCC" + guidString.Substring(3);

            return resultado;
        }

    }
}
