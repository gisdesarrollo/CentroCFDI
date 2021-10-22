using API.Catalogos;
using Aplicacion.Context;
using System;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos
{
    public class AcondicionarSucursales
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void CargaInicial(ref Sucursal Sucursal)
        {
            if (Sucursal.Bancos != null)
            {
                foreach (var banco in Sucursal.Bancos)
                {
                    banco.Banco = null;
                    banco.Sucursal = null;
                }
            }
        }

        public void Validaciones(Sucursal sucursal)
        {
            if(sucursal.Servidor != null)
            {
                if(sucursal.KeyXsa == null || sucursal.FechaInicial == null)
                {
                    throw new Exception("Favor de capturar Key de XSA y Fecha Inicial");
                }
            }

            var sucursalAnterior = _db.Sucursales.FirstOrDefault(s => s.Rfc == sucursal.Rfc);
            if(sucursalAnterior != null)
            {
                if(sucursal.Id == 0)
                {
                    throw new Exception(String.Format("Ya existe una sucursal con el RFC {0}", sucursal.Rfc));
                }
                else
                {
                    if(sucursal.Id != sucursalAnterior.Id)
                    {
                        throw new Exception(String.Format("Ya existe una sucursal con el RFC {0}", sucursal.Rfc));
                    }
                }
            }
        }

        public void Bancos(Sucursal Sucursal)
        {
            if (Sucursal.Bancos != null)
            {
                var idsBorrar = Sucursal.Bancos.Select(e => e.Id);
                var bancosAnteriores = _db.BancosSucursales.Where(es => es.SucursalId == Sucursal.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.BancosSucursales.RemoveRange(bancosAnteriores);
                _db.SaveChanges();

                foreach (var banco in Sucursal.Bancos)
                {
                    banco.SucursalId = Sucursal.Id;
                    banco.Sucursal = null;
                    banco.Banco = null;
                    _db.BancosSucursales.AddOrUpdate(banco);
                }

                _db.SaveChanges();
            }
            else
            {
                var bancosAnteriores = _db.BancosSucursales.Where(es => es.SucursalId == Sucursal.Id).ToList();

                _db.BancosSucursales.RemoveRange(bancosAnteriores);
                _db.SaveChanges();
            }
        }
    }
}
