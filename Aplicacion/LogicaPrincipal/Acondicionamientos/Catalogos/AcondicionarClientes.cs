using API.Catalogos;
using Aplicacion.Context;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Catalogos
{
    public class AcondicionarClientes
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void CargaInicial(ref SocioComercial sociocomercial)
        {
            if (sociocomercial.Bancos != null)
            {
                foreach (var banco in sociocomercial.Bancos)
                {
                    banco.Banco = null;
                    banco.SocioComercial = null;
                }
            }
        }

        public void Bancos(SocioComercial SocioComercial)
        {
            if (SocioComercial.Bancos != null)
            {
                var idsBorrar = SocioComercial.Bancos.Select(e => e.Id);
                var bancosAnteriores = _db.BancosSociosComerciales.Where(es => es.SocioComercialId == SocioComercial.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.BancosSociosComerciales.RemoveRange(bancosAnteriores);
                _db.SaveChanges();

                foreach (var banco in SocioComercial.Bancos)
                {
                    banco.SocioComercialId = SocioComercial.Id;
                    banco.Banco = null;
                    _db.BancosSociosComerciales.AddOrUpdate(banco);
                }

                _db.SaveChanges();
            }
            else
            {
                var bancosAnteriores = _db.BancosSociosComerciales.Where(es => es.SocioComercialId == SocioComercial.Id).ToList();

                _db.BancosSociosComerciales.RemoveRange(bancosAnteriores);
                _db.SaveChanges();
            }
        }
    }
}
