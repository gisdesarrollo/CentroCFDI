using API.Operaciones.ComplementosPagos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Facturas
{
   public class GetTipoCambioDocRel
    {
        public Decimal GetTipoCambioDocRelacionadoUSD(DocumentoRelacionado documentoRelacionado, double TipoCambioPago, double MontoPago)
        {
            Decimal valor = ((Decimal)documentoRelacionado.ImportePagado / (Decimal)documentoRelacionado.EquivalenciaDR) / (Decimal)documentoRelacionado.ImportePagado;
            Decimal valorFormat = decimal.Round(valor, 12);//7
            return valor;
        }
    }
}
