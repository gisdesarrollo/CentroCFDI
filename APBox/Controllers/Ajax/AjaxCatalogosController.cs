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

        //public PartialViewResult SucursalesProveedores(int sucursalId)
        //{
        //    var proveedorSucursal = new ProveedorSucursal
        //    {
        //        SucursalId = sucursalId,
        //        Sucursal = _db.Sucursales.Find(sucursalId)
        //    };
        //    return PartialView("~/Views/Proveedores/Sucursales.cshtml", proveedorSucursal);
        //}

        public PartialViewResult BancosClientes(int bancoId, string nombre, string numeroCuenta)
        {
            var bancoCliente = new BancoSocioComercial
            {
                BancoId = bancoId,
                Banco = _db.Bancos.Find(bancoId),
                Nombre = nombre,
                NumeroCuenta = numeroCuenta
            };

            return PartialView("~/Views/SociosComerciales/Bancos.cshtml", bancoCliente);
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
            var bancosClientes = _db.BancosSociosComerciales.Where(s => s.SocioComercialId == clienteId).OrderBy(s => s.Nombre).ToList();

            foreach (var banco in bancosClientes)
            {
                banco.SocioComercial.Sucursal = new API.Catalogos.Sucursal();
                
                if(banco.SocioComercial.Bancos != null)
                {
                    banco.SocioComercial.Bancos.Clear();
                    banco.SocioComercial.Bancos = null;
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
                cfdi.FacturaEmitida.CodigoQR = null;
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

        public JsonResult GetCfdisCanceladosCP(int? clienteId)
        {
            var cfdis = _db.ComplementoCartaPortes.Where(f => f.ReceptorId == clienteId && f.Status == API.Enums.Status.Cancelado).ToList();

            foreach (var cfdi in cfdis)
            {
                //ModelState.Remove("Banco.Id");
                cfdi.FacturaEmitida.ArchivoFisicoXml = null;
                cfdi.FacturaEmitida.CodigoQR = null;
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

                cfdi.Conceptoss.Clear();
                cfdi.Conceptoss = null;
                cfdi.Ubicaciones.Clear();
                cfdi.Ubicaciones = null;
                cfdi.Mercanciass = null;
                cfdi.Mercancias.Mercanciass.Clear();
                cfdi.Mercancias.Mercanciass = null;
                cfdi.FiguraTransporte.Clear();
                cfdi.FiguraTransporte = null;
                cfdi.Receptor = null;
                cfdi.Sucursal = null;
            }
            cfdis.ForEach(c => c.Mercancias = null);
            return Json(cfdis, JsonRequestBehavior.AllowGet);
        }

        public int GetReceptorGlobal(int clienteId)
        {
            var cliente = _db.SociosComerciales.Find(clienteId);
            int result = 0;
            if (cliente.Rfc == "XAXX010101000" && cliente.RazonSocial == "PUBLICO EN GENERAL")
            {
                result = 1;
            }
            

            return result;
        }

        #endregion
    }
}