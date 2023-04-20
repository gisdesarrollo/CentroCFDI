using API.Catalogos;
using API.Enums;
using API.Enums.CartaPorteEnums;
using Aplicacion.Context;
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

        public void ChecarUuidRepetido(ComprobanteCFDI comprobante40,ComprobanteCFDI33 comprobante33)
        {
            TimbreFiscalDigital timbreFiscalDigital = null;
            DateTime fecha;
            if (comprobante40 != null && comprobante33 == null)
            {
                timbreFiscalDigital = _decodificar.DecodificarTimbre(comprobante40,null);
                fecha = Convert.ToDateTime(comprobante40.Fecha);
            }
            if(comprobante40 == null && comprobante33 != null)
            {
                timbreFiscalDigital = _decodificar.DecodificarTimbre(null,comprobante33);
                fecha = Convert.ToDateTime(comprobante33.Fecha);
            }
            var fechaTimbrado = Convert.ToDateTime(timbreFiscalDigital.FechaTimbrado);

            var uuidRepetido = _db.FacturasEmitidasTemp.FirstOrDefault(f => f.Uuid == timbreFiscalDigital.UUID);
            if (uuidRepetido != null)
            {
                throw new Exception(String.Format("El folio fiscal {0} ya fue cargado al sistema", timbreFiscalDigital.UUID));
            }
        }

        public void ChecarRfcReceptor(ComprobanteCFDI comprobante40, ComprobanteCFDI33 comprobante33)
        {
            Sucursal emisor = null;
            String RFCComprobante = null;
            String RFCReceptor = null;
            String NombreReceptor = null;
            String DomicilioFiscal = null;
            c_RegimenFiscal? RegimenFiscal = new c_RegimenFiscal();
            c_UsoCfdiCP usoCfdi = new c_UsoCfdiCP();
            
            if (comprobante40 != null && comprobante33 == null)
            {
                RFCComprobante = comprobante40.Emisor.Rfc;
                RFCReceptor = comprobante40.Receptor.Rfc;
                NombreReceptor = comprobante40.Receptor.Nombre;
                DomicilioFiscal = comprobante40.Receptor.DomicilioFiscalReceptor;
                RegimenFiscal = comprobante40.Receptor.RegimenFiscalReceptor;
                usoCfdi = comprobante40.Receptor.UsoCFDI; 
                emisor = _db.Sucursales.FirstOrDefault(s => s.Rfc == comprobante40.Emisor.Rfc);
            }
            if (comprobante40 == null && comprobante33 != null)
            {
                RFCComprobante = comprobante33.Emisor.Rfc;
                RFCReceptor = comprobante33.Receptor.Rfc;
                NombreReceptor = comprobante33.Receptor.Nombre;
                emisor = _db.Sucursales.FirstOrDefault(s => s.Rfc == comprobante33.Emisor.Rfc);
                DomicilioFiscal = null;
                RegimenFiscal = null;
                usoCfdi = comprobante33.Receptor.UsoCFDI;
            }

            if (emisor == null)
            {
                throw new Exception(String.Format("El RFC del emisor {0} no fue encontrado en la base de datos", RFCComprobante));
            }

            Cliente cliente;

            if(RFCReceptor == "XEXX010101000" || RFCReceptor == "XAXX010101000")
            {
                cliente = _db.Clientes.FirstOrDefault(s => s.Rfc == RFCReceptor && s.RazonSocial == NombreReceptor && s.SucursalId == emisor.Id);
            }
            else
            {
                cliente = _db.Clientes.FirstOrDefault(s => s.Rfc == RFCReceptor && s.SucursalId == emisor.Id);
            }

            if (cliente == null)
            {
                if (comprobante33 != null)
                {
                    cliente = new Cliente
                    {
                        FechaAlta = DateTime.Now,
                        SucursalId = emisor.Id,
                        Pais = (API.Enums.c_Pais)c_Pais.MEX,
                        RazonSocial = NombreReceptor,
                        Rfc = RFCReceptor,
                        UsoCfdi = usoCfdi,
                        CodigoPostal = DomicilioFiscal,
                        Status = API.Enums.Status.Activo
                    };
                }
                else {
                    cliente = new Cliente
                    {
                        FechaAlta = DateTime.Now,
                        SucursalId = emisor.Id,
                        Pais = (API.Enums.c_Pais)c_Pais.MEX,
                        RazonSocial = NombreReceptor,
                        Rfc = RFCReceptor,
                        UsoCfdi = usoCfdi,
                        CodigoPostal = DomicilioFiscal,
                        RegimenFiscal = (API.Enums.c_RegimenFiscal)RegimenFiscal,
                        Status = API.Enums.Status.Activo
                    };
                }

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
