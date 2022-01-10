using API.Operaciones.ComplementoCartaPorte;
using API.RelacionesCartaPorte;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public List<ComplementoCartaPorte> Filtrar(DateTime fechaInicial, DateTime fechaFinal, int sucursalId)
        {
            var complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal).ToList();
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
                    concepto.ComplementoCP = null;
                    //concepto.ComplementoCP = complementoCP;
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
               // complementoCP.Total += complementoCP.Subtotal;
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
                complementoCP.Mercancias.Mercancia.Pedimentos = null;
                complementoCP.Mercancias.Mercancia.GuiasIdentificacion = null;
                complementoCP.Mercancias.Mercancia.CantidadTransportada = null;
                foreach (var mercancia in complementoCP.Mercanciass)
                {
                   
                    if (complementoCP.Pedimentoss != null)
                    {
                        foreach (var pedimento in complementoCP.Pedimentoss)
                        {
                            pedimento.Mercancia = null;
                            //pedimento.Mercancia = mercancia;
                            mercancia.Pedimentoss.Add(pedimento);
                        }

                    }
                    if (complementoCP.GuiasIdentificacioness != null)
                    {
                        foreach (var gIdentificacion in complementoCP.GuiasIdentificacioness)
                        {
                            gIdentificacion.Mercancia = null;
                            //gIdentificacion.Mercancia = mercancia;
                            mercancia.GuiasIdentificacionss.Add(gIdentificacion);
                        }

                    }
                    if (complementoCP.CantidadTransportadass != null)
                    {
                        foreach (var cTransportada in complementoCP.CantidadTransportadass)
                        {
                            cTransportada.Mercancia = null;
                            //cTransportada.Mercancia = mercancia;
                            mercancia.CantidadTransportadass.Add(cTransportada);
                        }

                    }


                    if (mercancia.DetalleMercancia == null)
                    {
                        mercancia.DetalleMercancia = null;
                    }
                    else
                    {
                        if (complementoCP.ClaveTransporteId == "02" || complementoCP.ClaveTransporteId == "04")
                        {
                            complementoCP.Mercancias.PesoBrutoTotal += mercancia.DetalleMercancia.PesoBruto;
                            complementoCP.Mercancias.PesoNetoTotal += mercancia.DetalleMercancia.PesoNeto;

                        }
                    }
                    if (!mercancia.MaterialPeligrosos)
                    {
                        mercancia.ClaveMaterialPeligroso = null;
                        mercancia.DescripEmbalaje = null;
                        mercancia.TipoEmbalaje_Id = null;
                        complementoCP.ValidaMaterialPeligroso = false;

                    }
                    else { complementoCP.ValidaMaterialPeligroso = true; }
                    complementoCP.Mercancias.Mercanciass = new List<Mercancia>();
                    complementoCP.Mercancias.Mercanciass.Add(mercancia);

                }
            }

            // carga transporte
            if (!complementoCP.TranspInternac)
            {
                complementoCP.EntradaSalidaMerc = null;
                complementoCP.PaisOrigendestino = null;
                complementoCP.viaEntradaSalida = null;
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
        

        public void cargaUbicaciones(ref ComplementoCartaPorte complementoCartaPorte)
        {
            var cartaPorteDb = _db.ComplementoCartaPortes.Find(complementoCartaPorte.Id);
            cartaPorteDb.TotalDistRec = 0;
            if (complementoCartaPorte.Ubicaciones != null)
            {
                foreach (var ubicacion in complementoCartaPorte.Ubicaciones)
                {
                    if (ubicacion.TipoUbicacion == "Destino")
                    {
                        cartaPorteDb.TotalDistRec += ubicacion.DistanciaRecorrida;
                    }
                    if(ubicacion.RfcRemitenteDestinatario != "XEXX010101000")
                    {
                        ubicacion.NumRegIdTrib = null;
                        ubicacion.ResidenciaFiscal = null;
                    }
                    if(complementoCartaPorte.ClaveTransporteId == "01") {
                        ubicacion.TipoEstacion_Id = null;
                        ubicacion.TipoEstaciones = null;
                        ubicacion.NumEstacion = null;
                        ubicacion.NavegacionTrafico = null;
                        ubicacion.NombreEstacion = null;
                    }
                    _db.Domicilios.Add(ubicacion.Domicilio);
                    _db.SaveChanges();
                   // ubicacion.Domicilio_Id = ubicacion.Domicilio.Id;
                    _db.UbicacionOrigen.Add(ubicacion);
                    _db.SaveChanges();

                }
                
                _db.Entry(cartaPorteDb).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public void cargaMercancias(ref ComplementoCartaPorte complementoCartPorte)
        {


            var mercanciaDb = _db.Mercancias.Find(complementoCartPorte.Mercancias.Id);
            if (complementoCartPorte.Mercanciass != null)
            {
                foreach(var mercancia in complementoCartPorte.Mercanciass)
                {
                    if(mercancia.DetalleMercancia == null)
                    {
                        
                        mercancia.DetalleMercancia = null;
                        //_db.DetalleMercancias.Add(mercancia.DetalleMercancia);
                        //_db.SaveChanges();
                    }
                    else
                    {
                        if(complementoCartPorte.ClaveTransporteId=="02" || complementoCartPorte.ClaveTransporteId == "04")
                        {
                            mercanciaDb.PesoBrutoTotal += mercancia.DetalleMercancia.PesoBruto;
                            mercanciaDb.PesoNetoTotal += mercancia.DetalleMercancia.PesoNeto;
                            _db.Entry(mercanciaDb).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    if (!mercancia.MaterialPeligrosos)
                    {
                        mercancia.ClaveMaterialPeligroso = null;
                        mercancia.DescripEmbalaje = null;
                        mercancia.TipoEmbalaje_Id = null;
                        complementoCartPorte.ValidaMaterialPeligroso = false;

                    }
                    else { complementoCartPorte.ValidaMaterialPeligroso = true; }
                    _db.Mercancia.Add(mercancia);
                    _db.SaveChanges();
                    if (mercancia.Pedimentoss != null)
                    {
                        var mercanciaPedimento = new MercanciaPedimentos();
                        foreach (var pe in mercancia.Pedimentoss)
                        {
                            _db.Pedimentos.Add(pe);
                            _db.SaveChanges();
                            
                            mercanciaPedimento.Mercancia_Id = mercancia.Id;
                            mercanciaPedimento.Pedimentos_Id = pe.Id;
                           // _db.MercanciaPedimentos.Add(mercanciaPedimento);
                            _db.SaveChanges();
                        }
                        
                        
                    }
                    if (mercancia.GuiasIdentificacionss != null)
                    {
                        foreach(var GI in mercancia.GuiasIdentificacionss)
                        {
                            _db.GuiasIdentificacion.Add(GI);
                            _db.SaveChanges();
                            var mercanciGuiasIdent = new MercanciaGuiasIdentificacion()
                            {
                                Mercancia_Id = mercancia.Id,
                                GuiasIdentificacion_Id = GI.Id
                            };

                            //_db.MercanciasGuiasIdentificacion.Add(mercanciGuiasIdent);
                            _db.SaveChanges();
                        }
                        
                    }
                    if (mercancia.CantidadTransportadass != null)
                    {
                        foreach(var CT in mercancia.CantidadTransportadass)
                        {
                            _db.CantidadTransportadas.Add(CT);
                            _db.SaveChanges();
                            var mercanciaCantidaTransp = new MercanciaCantidadTransportada()
                            {
                                Mercancia_Id = mercancia.Id,
                                CantidadTransportada_Id = CT.Id
                            };
                            //_db.MercanciaCantidadTransportadas.Add(mercanciaCantidaTransp);
                            _db.SaveChanges();
                        }
                       
                    }
                   
                    
                    var RelMercancias = new MercanciasMercancia()
                    {
                        Mercancias_Id = complementoCartPorte.Mercancias.Id,
                        Mercancia_Id = mercancia.Id
                    };
                    //_db.MercanciasMercancias.Add(RelMercancias);
                    _db.SaveChanges();
                }
            }
        }

        public void cargaTransporte(ref ComplementoCartaPorte complementoCartaPorte)
        {
            //var mercanciaDb = _db.Mercancias.Find(complementoCartaPorte.Mercancias.Id);
            DateTime fecha = complementoCartaPorte.FechaDocumento;
            DateTime hora = complementoCartaPorte.Hora;
            DateTime fechaDocumentoCompleto = new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, hora.Second);
            complementoCartaPorte.Hora = fechaDocumentoCompleto;
            complementoCartaPorte.FacturaEmitida = null;
            if (!complementoCartaPorte.TranspInternac)
            {
                complementoCartaPorte.EntradaSalidaMerc = null;
                complementoCartaPorte.PaisOrigendestino = null;
                complementoCartaPorte.viaEntradaSalida = null;
            }
            if (complementoCartaPorte.ClaveTransporteId == "01")
            {
                if (!complementoCartaPorte.ValidaMaterialPeligroso)
                {
                    if (complementoCartaPorte.Mercancias.AutoTransporte.Seguros != null)
                    {
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.AseguraMedAmbiente = null;
                        complementoCartaPorte.Mercancias.AutoTransporte.Seguros.PolizaMedAmbiente = null;
                    }
                }
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;

               
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
                if (complementoCartaPorte.Mercancias.AutoTransporte.Remolquess != null)
                {
                    foreach(var remolque in complementoCartaPorte.Mercancias.AutoTransporte.Remolquess)
                    {
                        _db.Remolques.Add(remolque);
                        _db.SaveChanges();
                        var relAutoTransporteRemolque = new AutotransporteRemolque()
                        {
                            Autotransporte_Id = complementoCartaPorte.Mercancias.AutoTransporte.Id,
                            Remolques_Id = remolque.Id
                        };
                       // _db.AutotransporteFederalRemolques.Add(relAutoTransporteRemolque);
                        _db.SaveChanges();
                    }
                    
                }
            }
            else if(complementoCartaPorte.ClaveTransporteId == "02")
            {
               
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
                if (complementoCartaPorte.Mercancias.TransporteMaritimo.ContenedoresM != null)
                {
                    foreach(var contenedor in complementoCartaPorte.Mercancias.TransporteMaritimo.ContenedoresM)
                    {
                        _db.ContenedoresM.Add(contenedor);
                        _db.SaveChanges();

                        var relMaritimoContenedor = new TransporteMaritimoContenedorM() { 
                        TransporteMaritimo_Id = complementoCartaPorte.Mercancias.TransporteMaritimo.Id,
                        ContenedorM_Id = contenedor.Id
                        };
                        //_db.TransporteMaritimoContenedoresM.Add(relMaritimoContenedor);
                        _db.SaveChanges();
                    }
                    


                }

            }
            else if(complementoCartaPorte.ClaveTransporteId == "03")
            {
              
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();

            }
            else if (complementoCartaPorte.ClaveTransporteId == "04")
            {
                
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
                if (complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss != null)
                {
                    foreach(var derechoDePaso in complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss)
                    {
                        _db.DerechoDePasos.Add(derechoDePaso);
                        _db.SaveChanges();

                        var relFerroviarioDDePaso = new TransporteFerroviarioDerechosDePaso() { 
                        TransporteFerroviario_Id = complementoCartaPorte.Mercancias.TransporteFerroviario.Id,
                        DerechosDePaso_Id = derechoDePaso.Id
                        };
                        //_db.TransporteFerroviarioDerechosDePasos.Add(relFerroviarioDDePaso);
                        _db.SaveChanges();
                    }
                   
                }
                if (complementoCartaPorte.Mercancias.TransporteFerroviario.Carros != null)
                {
                    foreach(var carro in complementoCartaPorte.Mercancias.TransporteFerroviario.Carros)
                    {
                        _db.Carros.Add(carro);
                        _db.SaveChanges();
                        if (carro.ContenedoresC != null)
                        {
                            foreach (var contenedorC in carro.ContenedoresC)
                            {
                                _db.ContenedoresC.Add(contenedorC);
                                _db.SaveChanges();
                                var relCarroContenedorC = new CarroContenedorC() { 
                                Carro_Id = carro.Id,
                                ContenedorC_Id = contenedorC.Id
                                };
                               // _db.CarrosContenedorC.Add(relCarroContenedorC);
                                _db.SaveChanges();
                            }
                        }
                        var relFerroviarioCarro = new TransporteFerroviarioCarro()
                        {
                            TransporteFerroviario_Id = complementoCartaPorte.Mercancias.TransporteFerroviario.Id,
                            Carro_Id = carro.Id
                        };
                        //_db.TransporteFerroviarioCarros.Add(relFerroviarioCarro);
                        _db.SaveChanges();

                    }
                }

            }
            else
            {
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
            }
        }

        public void cargaFiguraTransporte(ref ComplementoCartaPorte complementoCartaPorte)
        {
            if (complementoCartaPorte.FiguraTransporte != null)
            {
                foreach(var FT in complementoCartaPorte.FiguraTransporte)
                {
                    if (FT.Domicilio != null)
                    {
                        if (FT.Domicilio.Pais != null)
                        {
                            _db.Domicilios.Add(FT.Domicilio);
                            _db.SaveChanges();
                        }
                        else
                        {
                            FT.Domicilio = null;
                        }
                    }
                    else
                    {
                        FT.Domicilio = null;
                    }
                    _db.Tiposfigura.Add(FT);
                    _db.SaveChanges();
                    if (FT.PartesTransportes != null)
                    {
                        foreach(var PT in FT.PartesTransportes)
                        {

                            _db.PartesTransporte.Add(PT);
                            _db.SaveChanges();

                            var FiguraParteTransporte = new TiposFiguraPartesTransporte() { 
                                TiposFigura_Id = FT.Id,
                                PartesTransporte_Id = PT.Id
                            };
                            //_db.TiposFiguraPartesTransporte.Add(FiguraParteTransporte);
                            _db.SaveChanges();
                           
                        }
                    }
                }
            }
        }

        public void cargaConceptosEdit(ref ComplementoCartaPorte complementoCP)
        {
            complementoCP.Conceptoss.ForEach(p => _db.Entry(p).State = EntityState.Modified);
            //_db.Entry(complementoCP.Conceptoss).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public void cargaMercanciasEdit(ref ComplementoCartaPorte complementoCP)
        {
            _db.Entry(complementoCP.Mercanciass).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void cargaUbicacionesEdit(ref ComplementoCartaPorte complementoCP)
        {

        }

        public void cargaTransportesEdit(ref ComplementoCartaPorte complementoCP) 
        {
        
        }

        public void cargaFiguraTransporteEdit(ref ComplementoCartaPorte complementoCP)
        {

        }
    }
}
