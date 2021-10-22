using API.Catalogos;
using Aplicacion.Context;
using CFDI.API.CFDI33.CFDI;
using CFDI.API.Enums.CFDI33;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Facturas
{
    public class Validar
    {

        #region Variables

        private readonly Decodificar _decodificar = new Decodificar();
        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void ChecarUuidRepetido(Comprobante comprobante)
        {
            var timbreFiscalDigital = _decodificar.DecodificarTimbre(comprobante);
            var fecha = Convert.ToDateTime(comprobante.Fecha);
            var fechaTimbrado = Convert.ToDateTime(timbreFiscalDigital.FechaTimbrado);

            var uuidRepetido = _db.FacturasEmitidas.FirstOrDefault(f => f.Uuid == timbreFiscalDigital.UUID);
            if (uuidRepetido != null)
            {
                throw new Exception(String.Format("El folio fiscal {0} ya fue cargado al sistema", timbreFiscalDigital.UUID));
            }
        }

        public void ChecarRfcReceptor(Comprobante comprobante)
        {
            var emisor = _db.Sucursales.FirstOrDefault(s => s.Rfc == comprobante.Emisor.Rfc);
            if(emisor == null)
            {
                throw new Exception(String.Format("El RFC del emisor {0} no fue encontrado en la base de datos", comprobante.Emisor.Rfc));
            }

            Cliente cliente;

            if(comprobante.Receptor.Rfc == "XEXX010101000" || comprobante.Receptor.Rfc == "XAXX010101000")
            {
                cliente = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante.Receptor.Rfc && s.RazonSocial == comprobante.Receptor.Nombre && s.SucursalId == emisor.Id);
            }
            else
            {
                cliente = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante.Receptor.Rfc && s.SucursalId == emisor.Id);
            }

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    FechaAlta = DateTime.Now,
                    SucursalId = emisor.Id,
                    Pais = c_Pais.MEX,
                    RazonSocial = comprobante.Receptor.Nombre,
                    Rfc = comprobante.Receptor.Rfc,
                    Status = API.Enums.Status.Activo
                };

                cliente.Banco = null;

                _db.Clientes.Add(cliente);

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
            }
        }
    }
}
