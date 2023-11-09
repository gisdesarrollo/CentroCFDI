using API.Models.Facturas;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Validacion;
using System;
using System.Data.Entity;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Facturas
{

    public class OperacionesCfdisEmitidos
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly DecodificaFacturas _decodifica = new DecodificaFacturas();

        #endregion

        public void ObtenerFacturas(ref FacturasEmitidasModel facturasEmitidasModel)
        {
            var fechaInicial = facturasEmitidasModel.FechaInicial;
            var fechaFinal = facturasEmitidasModel.FechaFinal.AddDays(1); //SE AGREGA UN DIA A LA FECHA FINAL
            var sucursalId = facturasEmitidasModel.SucursalId;
            facturasEmitidasModel.FacturaEmitidasTemporal = _db.FacturasEmitidasTemp.Where(fe => fe.EmisorId == sucursalId && fe.Fecha >= fechaInicial && fe.Fecha < fechaFinal).ToList();
            
        }

        public void Cancelar(FacturaEmitida facturaEmitida)
        {
            var emitida = _db.FacturasEmitidas.Find(facturaEmitida.Id);
            var sucursal = _db.Sucursales.Find(emitida.EmisorId);


            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(emitida.ArchivoFisicoXml);
            string UUID = _decodifica.LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }

            //Creamos el objeto de cancelación de la DLL.
            RVCFDI33.RVCancelacion.Cancelacion objCan = new RVCFDI33.RVCancelacion.Cancelacion();
            //Definimos la ruta en donde se guardará el XML de Solicitud de Cancelación en el disco duro.
            string ArchivoCancelacion = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCancelados//{0}-{1}-{2}.xml", emitida.Serie, emitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            //ruta temp cer y key produccion
            string cerRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.cer", emitida.Serie, emitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string keyRuta = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//{0}-{1}-{2}.key", emitida.Serie, emitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            System.IO.File.WriteAllBytes(cerRuta, sucursal.Cer);
            System.IO.File.WriteAllBytes(keyRuta, sucursal.Key);
            //Creamos el XML de Solicitud de Cancelación.
            string folioSustitucion = (facturaEmitida.FolioSustitucion == null ? "" : facturaEmitida.FolioSustitucion);

            objCan.crearXMLCancelacionArchivo(cerRuta, keyRuta, sucursal.PasswordKey, UUID, ArchivoCancelacion, facturaEmitida.MotivoCancelacion, folioSustitucion);
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
                MarcarNoFacturado(emitida.Id);
            }
            else
            {
                throw new Exception(String.Format("Ocurrió un error al cancelar el XML: " + objCan.MensajeDeError));
            }

        }

        private void MarcarNoFacturado(int facturaEmitidaId)
        {
            //Actualizar status relacion con algun complemento o solo comprobante
            //CartaPorte
            var cartaPorte = _db.ComplementoCartaPortes.Where(cp => cp.FacturaEmitidaId == facturaEmitidaId).FirstOrDefault();
            if(cartaPorte != null)
            {
                cartaPorte.Status = API.Enums.Status.Cancelado;
                _db.Entry(cartaPorte).State = EntityState.Modified;
                _db.SaveChanges();
            }
            //Pagos
            var pagos = _db.ComplementosPago.Where(p => p.FacturaEmitidaId == facturaEmitidaId).FirstOrDefault();
            if(pagos != null)
            {
                pagos.Status = API.Enums.Status.Cancelado;
                _db.Entry(pagos).State = EntityState.Modified;
                _db.SaveChanges();
            }
            //Cfdi Internos
            var cfdi = _db.ComprobantesCfdi.Where(c => c.FacturaEmitidaId == facturaEmitidaId).FirstOrDefault();
            if (cfdi != null)
            {
                cfdi.Status = API.Enums.Status.Cancelado;
                _db.Entry(cfdi).State = EntityState.Modified;
                _db.SaveChanges();
            }
             
             var facturaEmitida = _db.FacturasEmitidas.Find(facturaEmitidaId);
            if(facturaEmitida != null)
            {  
                facturaEmitida.Status = API.Enums.Status.Cancelado;
                _db.Entry(facturaEmitida).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
    }
}
