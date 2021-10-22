using API.Operaciones.ComplementosPagos;
using Aplicacion.Context;
using System.Linq;
using System.Data.Entity.Migrations;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones
{
    public class AcondicionarComplementosPagos
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void CargaInicial(ref ComplementoPago complementoPago)
        {
            if(complementoPago.Pagos != null)
            {
                foreach (var pago in complementoPago.Pagos)
                {
                    pago.BancoOrdenante = null;
                    pago.BancoBeneficiario = null;
                    pago.ComplementoPago = null;
                }
            }

            complementoPago.Pago = null;
        }

        public void Pagos(ComplementoPago complementoPago)
        {
            if (complementoPago.Pagos != null)
            {
                var idsBorrar = complementoPago.Pagos.Select(e => e.Id);
                var pagosAnteriores = _db.Pagos.Where(es => es.ComplementoPagoId == complementoPago.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.Pagos.RemoveRange(pagosAnteriores);
                _db.SaveChanges();

                foreach (var pago in complementoPago.Pagos.Except(pagosAnteriores))
                {
                    pago.ComplementoPagoId = complementoPago.Id;
                    pago.BancoOrdenante = null;
                    pago.BancoBeneficiario = null;
                    pago.ComplementoPago = null;

                    _db.Pagos.AddOrUpdate(pago);
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
                var pagosAnteriores = _db.Pagos.Where(es => es.ComplementoPagoId == complementoPago.Id).ToList();

                _db.Pagos.RemoveRange(pagosAnteriores);
                _db.SaveChanges();
            }
        }

        public void DocumentosRelacionados(ComplementoPago complementoPago)
        {
            //if (complementoPago.DocumentosRelacionados != null)
            //{
            //    foreach (var pago in complementoPago.Pagos)
            //    {
            //        var idsBorrar = complementoPago.DocumentosRelacionados.Select(e => e.Id);
            //        var documentosAnteriores = _db.DocumentosRelacionados.Where(es => es.PagoId == pago.Id && !idsBorrar.Contains(es.Id)).ToList();

            //        _db.DocumentosRelacionados.RemoveRange(documentosAnteriores);
            //        _db.SaveChanges();

            //        foreach (var documento in complementoPago.DocumentosRelacionados)
            //        {
            //            documento.PagoId = pago.Id;
            //            _db.DocumentosRelacionados.AddOrUpdate(documento);
            //        }

            //        _db.SaveChanges();
            //    }
            //}
            //else
            //{
                var documentosAnteriores = _db.DocumentosRelacionados.Where(es => es.Pago.ComplementoPagoId == complementoPago.Id).ToList();

                _db.DocumentosRelacionados.RemoveRange(documentosAnteriores);
                _db.SaveChanges();
            //}
        }
    }
}
