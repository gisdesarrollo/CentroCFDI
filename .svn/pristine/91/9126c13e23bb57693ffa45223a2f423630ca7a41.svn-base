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

        public void CargaInicial(ref Cliente Cliente)
        {
            if (Cliente.Bancos != null)
            {
                foreach (var banco in Cliente.Bancos)
                {
                    banco.Banco = null;
                    banco.Cliente = null;
                }
            }
        }

        public void Bancos(Cliente Cliente)
        {
            if (Cliente.Bancos != null)
            {
                var idsBorrar = Cliente.Bancos.Select(e => e.Id);
                var bancosAnteriores = _db.BancosClientes.Where(es => es.ClienteId == Cliente.Id && !idsBorrar.Contains(es.Id)).ToList();

                _db.BancosClientes.RemoveRange(bancosAnteriores);
                _db.SaveChanges();

                foreach (var banco in Cliente.Bancos)
                {
                    banco.ClienteId = Cliente.Id;
                    banco.Banco = null;
                    _db.BancosClientes.AddOrUpdate(banco);
                }

                _db.SaveChanges();
            }
            else
            {
                var bancosAnteriores = _db.BancosClientes.Where(es => es.ClienteId == Cliente.Id).ToList();

                _db.BancosClientes.RemoveRange(bancosAnteriores);
                _db.SaveChanges();
            }
        }
    }
}
