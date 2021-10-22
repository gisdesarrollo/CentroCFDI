using APBox.Context;
using API.Relaciones;
using System.Linq;
using System.Web.Mvc;

namespace InventorAid.Controllers.Ajax
{
    public class AjaxCatalogosController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        public PartialViewResult SucursalesUsuarios(int sucursalId)
        {
            var usuarioSucursal = new UsuarioSucursal
            {
                SucursalId = sucursalId,
                Sucursal = _db.Sucursales.Find(sucursalId)
            };

            return PartialView("~/Views/Usuarios/Sucursales.cshtml", usuarioSucursal);
        }

        public PartialViewResult SucursalesProveedores(int sucursalId)
        {
            var proveedorSucursal = new ProveedorSucursal
            {
                SucursalId = sucursalId,
                Sucursal = _db.Sucursales.Find(sucursalId)
            };

            return PartialView("~/Views/Proveedores/Sucursales.cshtml", proveedorSucursal);
        }

        public PartialViewResult BancosClientes(int bancoId, string nombre, string numeroCuenta)
        {
            var bancoCliente = new BancoCliente
            {
                BancoId = bancoId,
                Banco = _db.Bancos.Find(bancoId),
                Nombre = nombre,
                NumeroCuenta = numeroCuenta
            };

            return PartialView("~/Views/Clientes/Bancos.cshtml", bancoCliente);
        }

        public PartialViewResult BancosSucursales(int bancoId, string nombre, string numeroCuenta)
        {
            var bancoSucursal = new BancoSucursal
            {
                BancoId = bancoId,
                Banco = _db.Bancos.Find(bancoId),
                Nombre = nombre,
                NumeroCuenta = numeroCuenta
            };

            return PartialView("~/Views/Sucursales/Bancos.cshtml", bancoSucursal);
        }

        #region Popular DropDowns

        public JsonResult GetSucursales(int grupoId)
        {
            var sucursales = _db.Sucursales.Where(s => s.GrupoId == grupoId).OrderBy(s => s.Nombre).ToList();

            foreach (var sucursal in sucursales)
            {
                sucursal.Grupo = null;

                if(sucursal.Bancos != null)
                {
                    sucursal.Bancos.Clear();
                    sucursal.Bancos = null;
                }
                sucursal.Banco = null;

                sucursal.ArchivoCer = null;
                sucursal.Cer = null;
                sucursal.ArchivoKey = null;
                sucursal.Key = null;
                sucursal.ArchivoLogo = null;
                sucursal.Logo = null;
            }

            return Json(sucursales, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBancosClientes(int clienteId)
        {
            var bancosClientes = _db.BancosClientes.Where(s => s.ClienteId == clienteId).OrderBy(s => s.Nombre).ToList();

            foreach (var banco in bancosClientes)
            {
                banco.Cliente.Sucursal = new API.Catalogos.Sucursal();
                
                if(banco.Cliente.Bancos != null)
                {
                    banco.Cliente.Bancos.Clear();
                    banco.Cliente.Bancos = null;
                }
            }

            return Json(bancosClientes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCfdisCancelados(int clienteId)
        {
            var cfdis = _db.ComplementosPago.Where(f => f.ReceptorId == clienteId && f.Status == API.Enums.Status.Cancelado).ToList();

            foreach (var cfdi in cfdis)
            {
                cfdi.FacturaEmitida.ArchivoFisicoXml = null;

                cfdi.FacturaEmitida.Emisor.Bancos.Clear();
                cfdi.FacturaEmitida.Emisor.Cer = null;
                cfdi.FacturaEmitida.Emisor.Key = null;
                cfdi.FacturaEmitida.Emisor.Grupo = null;
                cfdi.FacturaEmitida.Emisor.Logo = null;

                cfdi.FacturaEmitida.Receptor.Bancos.Clear();
                cfdi.FacturaEmitida.Receptor.Sucursal = null;

                cfdi.FacturaEmitida.CentroCosto = null;
                cfdi.FacturaEmitida.ComplementosPago.Clear();
                cfdi.FacturaEmitida.ComplementosPago = null;
                cfdi.FacturaEmitida.Departamento = null;

                cfdi.Pagos.Clear();
                cfdi.Pagos = null;
                cfdi.Receptor = null;
                cfdi.Sucursal = null;
            }

            return Json(cfdis, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}