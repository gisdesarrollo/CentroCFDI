using DTOs.Validacion;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API.Operaciones.Facturacion
{
    [Table("fac_validaciones")]
    public class Validacion : ValidacionDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        #region Propiedades Calculadas

        [NotMapped]
        public bool Correcto
        {
            get
            {
                if (!AddendaCorrecto || !ConceptosCorrecto || !EsquemaCorrecto || !RetencionesCorrecto 
                    || !RetencionesLocalesCorrecto || !SubtotalCorrecto || !TotalCorrecto ||
                    !TrasladosCorrecto || !TrasladosLocalesCorrecto)
                {
                    return false;
                }

                return true;
            }
        }

        [NotMapped]
        public String Descripcion
        {
            get
            {
                if (Correcto)
                {
                    return "Comprobante Correcto";
                }

                var sb = new StringBuilder();

                if (!AddendaCorrecto)
                {
                    sb.AppendLine(DetallesAddenda);
                }

                if (!ConceptosCorrecto)
                {
                    sb.AppendLine(Conceptos);
                }

                if (!EsquemaCorrecto)
                {
                    sb.AppendLine(Esquema);
                }

                if (!RetencionesCorrecto)
                {
                    sb.AppendLine(Retenciones);
                }

                if (!RetencionesLocalesCorrecto)
                {
                    sb.AppendLine(RetencionesLocales);
                }

                if (!SubtotalCorrecto)
                {
                    sb.AppendLine(Subtotal);
                }

                if (!TotalCorrecto)
                {
                    sb.AppendLine(Total);
                }

                if (!TrasladosCorrecto)
                {
                    sb.AppendLine(Traslados);
                }

                if (!TrasladosLocalesCorrecto)
                {
                    sb.AppendLine(TrasladosLocales);
                }

                return sb.ToString();
            }
        }

        #endregion

    }
}
