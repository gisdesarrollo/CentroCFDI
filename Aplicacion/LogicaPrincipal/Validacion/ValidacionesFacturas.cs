using API.Catalogos;
using API.Context;
using API.Operaciones.Facturacion;
using Infodextra.LogicaPrincipal;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Utilerias.LogicaPrincipal;

namespace Aplicacion.LogicaPrincipal.Validacion
{
    public class ValidacionesFacturas
    {

        #region Variables

        private readonly DataContext _db = new DataContext();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();
        private readonly ValidacionInfodextra _validacionInfodextra = new ValidacionInfodextra();

        #endregion

        public void Negocios(FacturaRecibida facturaRecibida, int sucursalId)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);
            if (facturaRecibida.Receptor == null)
            {
                throw new Exception(String.Format("El receptor de la factura {0} no coincide con el RFC {1}", facturaRecibida.NombreArchivoXml, sucursal.Rfc));
            }

            //Factura Repetida
            var facturaValidadaPreviamente = _db.FacturasRecibidas.FirstOrDefault(f => f.Uuid == facturaRecibida.Uuid);
            if (facturaValidadaPreviamente != null)
            {
                throw new Exception(String.Format("La factura {0} ya fue validada", facturaValidadaPreviamente.NombreArchivoXml));
            }

            //La factura es de la empresa
            if (facturaRecibida.Receptor.Rfc != sucursal.Rfc)
            {
                throw new Exception(String.Format("El rfc {0} no corresponde con la sucursal actual ({1})", facturaRecibida.Receptor.Rfc, sucursal.Rfc));
            }

            //El proveedor Existe
            facturaRecibida.Emisor = ChecarProveedor(facturaRecibida, sucursalId);
        }

        public API.Operaciones.Facturacion.Validacion Sat(String pathFactura, string rfc)
        {
            var xml = _operacionesStreams.ArchivoByteArray(pathFactura);
            var validacion = _validacionInfodextra.Validar(xml, rfc);
            return new API.Operaciones.Facturacion.Validacion
            {
                AddendaCorrecto = validacion.AddendaCorrecto,
                CadenaOriginal = validacion.CadenaOriginal,
                CadenaSat = validacion.CadenaSat,
                Certificado = validacion.Certificado,
                Cifrado = validacion.Cifrado,
                Conceptos = validacion.Conceptos,
                ConceptosCorrecto = validacion.ConceptosCorrecto,
                DescripcionSat = validacion.DescripcionSat,
                DetallesAddenda = validacion.DetallesAddenda,
                DigestionCadenaOriginal = validacion.DigestionCadenaOriginal,
                DigestionCadenaSat = validacion.DigestionCadenaSat,
                EntidadCertificadora = validacion.EntidadCertificadora,
                Esquema = validacion.Esquema,
                EsquemaCorrecto = validacion.EsquemaCorrecto,
                EstadoSat = validacion.EstadoSat,
                ExisteAddenda = validacion.ExisteAddenda,
                NumeroCertificadoSat = validacion.NumeroCertificadoSat,
                Retenciones = validacion.Retenciones,
                RetencionesCorrecto = validacion.RetencionesCorrecto,
                RetencionesLocales = validacion.RetencionesLocales,
                RetencionesLocalesCorrecto = validacion.RetencionesLocalesCorrecto,
                SelloCfdi = validacion.SelloCfdi,
                SelloCfdiValido = validacion.SelloCfdiValido,
                SelloSat = validacion.SelloSat,
                SelloSatValido = validacion.SelloSatValido,
                Subtotal = validacion.Subtotal,
                SubtotalCorrecto = validacion.SubtotalCorrecto,
                Total = validacion.Total,
                TotalCorrecto = validacion.TotalCorrecto,
                Traslados = validacion.Traslados,
                TrasladosCorrecto = validacion.TrasladosCorrecto,
                TrasladosLocales = validacion.TrasladosLocales,
                TrasladosLocalesCorrecto = validacion.TrasladosLocalesCorrecto,
                VigenciaCertificado = validacion.VigenciaCertificado
            };
        }

        private Proveedor ChecarProveedor(FacturaRecibida facturaRecibida, int sucursalId)
        {
            var proveedor = _db.Proveedores.FirstOrDefault(p => p.Rfc == facturaRecibida.Emisor.Rfc);
            if (proveedor != null)
            {
                return proveedor;
            }

            var sucursal = _db.Sucursales.Find(sucursalId);
            proveedor = new Proveedor
            {
                RazonSocial = facturaRecibida.Emisor.RazonSocial,
                Rfc = facturaRecibida.Emisor.Rfc,
                Pais = "MEX",
                GrupoId = sucursal.GrupoId
            };
            _db.Proveedores.Add(proveedor);

            try
            {
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

            return proveedor;
        }
    }
}
