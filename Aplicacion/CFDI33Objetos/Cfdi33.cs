
using API.Enums;
using API.Enums.CartaPorteEnums;

using System.Xml.Schema;
using System.Xml.Serialization;



[XmlRootAttribute("Comprobante", Namespace = "http://www.sat.gob.mx/cfd/3")]
public partial class ComprobanteCFDI33
{

    [XmlAttributeAttribute()]
    public string Version { get; set; }
    [XmlAttributeAttribute()]
    public string Serie { get; set; }
    [XmlAttributeAttribute()]
    public string Folio { get; set; }
    [XmlAttributeAttribute()]
    public string Fecha { get; set; }
    [XmlAttributeAttribute()]
    public string Sello { get; set; }
    [XmlAttributeAttribute()]
    public string FormaPago { get; set; }
    [XmlAttributeAttribute()]
    public string NoCertificado { get; set; }
    [XmlAttributeAttribute()]
    public string Certificado { get; set; }
    [XmlAttributeAttribute()]
    public string CondicionesDePago { get; set; }
    [XmlAttributeAttribute()]
    public decimal SubTotal { get; set; }
    [XmlAttributeAttribute()]
    public decimal Descuento { get; set; }
    [XmlAttributeAttribute()]
    public c_Moneda Moneda { get; set; }
    [XmlAttributeAttribute()]
    public decimal TipoCambio { get; set; }
    [XmlAttributeAttribute()]
    public decimal Total { get; set; }
    [XmlAttributeAttribute()]
    public c_TipoDeComprobante TipoDeComprobante { get; set; }
    [XmlAttributeAttribute()]
    public c_MetodoPago MetodoPago { get; set; }
    [XmlAttributeAttribute()]
    //c_codigoPostal
    public string LugarExpedicion { get; set; }
    [XmlAttributeAttribute()]
    public string Confirmacion { get; set; }

    public TimbreFiscalDigital TimbreFiscalDigital;

    public CartaPorte CartaPorte;

    public Pagos10 Pagos;

    public string CodigoQR;

    public string Logo;

    [XmlElementAttribute()]
    public ComprobanteCfdiRelacionados33 CfdiRelacionados { get; set; }

    [XmlElementAttribute()]
    public ComprobanteEmisor33 Emisor { get; set; }

    [XmlElementAttribute()]
    public ComprobanteReceptor33 Receptor { get; set; }

    [XmlArrayItemAttribute("Concepto")]
    public ComprobanteConcepto33[] Conceptos { get; set; }

    [XmlElementAttribute()]
    public ComprobanteImpuestos33 Impuestos { get; set; }

    [XmlElementAttribute()]
    public ComprobanteComplemento1020[] Complemento { get; set; }


}


public partial class ComprobanteCfdiRelacionados33 {

        [XmlArrayItemAttribute()]
        public ComprobanteCfdiRelacionadosCfdiRelacionado33[] CfdiRelacionado { get; set; }
        
        [XmlAttributeAttribute()]
        public string TipoRelacion { get; set; }
    
    
}


public partial class ComprobanteCfdiRelacionadosCfdiRelacionado33 {

        [XmlAttributeAttribute()]
        public string UUID { get; set; }
}



public partial class ComprobanteEmisor33 {

        [XmlAttributeAttribute()]
        public string Rfc { get; set; }
        
        [XmlAttributeAttribute()]
        public string Nombre { get; set; }
        
        [XmlAttributeAttribute()]
        public c_RegimenFiscal RegimenFiscal { get; set; }
    
    
}



public partial class ComprobanteReceptor33 {

        [XmlAttributeAttribute()]
        public string Rfc { get; set; }

        [XmlAttributeAttribute()]
        public string Nombre { get; set; }

        [XmlAttributeAttribute()]
        public c_Pais ResidenciaFiscal { get; set; }

        [XmlAttributeAttribute()]
        public string NumRegIdTrib { get; set; }

        [XmlAttributeAttribute()]
        public c_UsoCfdiCP UsoCFDI { get; set; }
    
}



public partial class ComprobanteConcepto33 {

    [XmlElementAttribute()]
    public ComprobanteConceptoImpuestos33 Impuestos { get; set; }

    [XmlAttributeAttribute()]
    public string ClaveProdServ { get; set; }

    [XmlAttributeAttribute()]
    public string NoIdentificacion { get; set; }

    [XmlAttributeAttribute()]
    public decimal Cantidad { get; set; }

    [XmlAttributeAttribute()]
    public string ClaveUnidad { get; set; }

    [XmlAttributeAttribute()]
    public string Unidad { get; set; }

    [XmlAttributeAttribute()]
    public string Descripcion { get; set; }

    [XmlAttributeAttribute()]
    public decimal ValorUnitario { get; set; }

    [XmlAttributeAttribute()]
    public decimal Importe { get; set; }

    [XmlAttributeAttribute()]
    public decimal Descuento { get; set; }

    [XmlAttributeAttribute()]
    public c_ObjetoImp ObjetoImp { get; set; } // ATRIBUTO VERSION 4.0
}


public partial class ComprobanteConceptoImpuestos33 {

    [XmlArrayItemAttribute("Traslado")]
    public ComprobanteConceptoImpuestosTraslado33[] Traslados { get; set; }

    [XmlArrayItemAttribute("Retencion")]
    public ComprobanteConceptoImpuestosRetencion33[] Retenciones { get; set; }
    
}


public partial class ComprobanteConceptoImpuestosTraslado33 {

    [XmlAttributeAttribute()]
    public decimal Base { get; set; }

    [XmlAttributeAttribute()]
    public string Impuesto { get; set; }

    [XmlAttributeAttribute()]
    public c_TipoFactor TipoFactor { get; set; }

    [XmlAttributeAttribute()]
    public decimal TasaOCuota { get; set; }

    [XmlAttributeAttribute()]
    public decimal Importe { get; set; }
}


public partial class ComprobanteConceptoImpuestosRetencion33 {

    [XmlAttributeAttribute()]
    public decimal Base { get; set; }

    [XmlAttributeAttribute()]
    public string Impuesto { get; set; }

    [XmlAttributeAttribute()]
    public c_TipoFactor TipoFactor { get; set; }

    [XmlAttributeAttribute()]
    public decimal TasaOCuota { get; set; }

    [XmlAttributeAttribute()]
    public decimal Importe { get; set; }

}

public partial class ComprobanteImpuestos33 {

    [XmlArrayItemAttribute("Retencion")]
    public ComprobanteImpuestosRetencion33[] Retenciones { get; set; }

    [XmlArrayItemAttribute("Traslado")]
    public ComprobanteImpuestosTraslado33[] Traslados { get; set; }

    [XmlAttributeAttribute()]
    public decimal TotalImpuestosRetenidos { get; set; }

    [XmlAttributeAttribute()]
    public decimal TotalImpuestosTrasladados { get; set; }
    
    
}


public partial class ComprobanteImpuestosRetencion33 {

    [XmlAttributeAttribute()]
    public string Impuesto { get; set; }

    [XmlAttributeAttribute()]
    public decimal Importe { get; set; }
    
    
}

public partial class ComprobanteImpuestosTraslado33 {


    [XmlAttributeAttribute()]
    public decimal Base { get; set; } // ATRIBUTO VERSION 4.0

    [XmlAttributeAttribute()]
    public string Impuesto { get; set; }

    [XmlAttributeAttribute()]
    public c_TipoFactor TipoFactor { get; set; }

    [XmlAttributeAttribute()]
    public decimal TasaOCuota { get; set; }

    [XmlAttributeAttribute()]
    public decimal Importe { get; set; }
   
}



[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
public partial class ComprobanteComplemento1020 {

    [XmlAnyElementAttribute()]
    //[XmlArrayItemAttribute()]
    public System.Xml.XmlElement[] Any { get; set; }
    
    
}


[XmlRootAttribute("Pagos", Namespace = "http://www.sat.gob.mx/Pagos")]
//[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos10")]
//[System.Xml.Serialization.XmlRootAttribute("Pagos",Namespace = "http://www.sat.gob.mx/Pagos10", IsNullable = false)]
public partial class Pagos10
{

    [XmlElementAttribute()]
    public PagosTotales Totales { get; set; }

    //[XmlArrayItemAttribute()]
    [System.Xml.Serialization.XmlElementAttribute("Pago")]
    public PagosPago10[] Pago { get; set; }

    [XmlAttributeAttribute()]
    public string Version { get; set; }
}

public partial class PagosPago10 
{
    [XmlArrayItemAttribute()]
    public PagosPagoDoctoRelacionado33[] DoctoRelacionado { get; set; }

    [XmlAttributeAttribute()]
    public System.DateTime FechaPago { get; set; }

    [XmlAttributeAttribute()]
    public string FormaDePagoP { get; set; }

    [XmlAttributeAttribute()]
    public c_Moneda MonedaP { get; set; }

    [XmlAttributeAttribute()]
    public decimal TipoCambioP { get; set; }

    [XmlAttributeAttribute()]
    public decimal Monto { get; set; }

    [XmlAttributeAttribute()]
    public string NumOperacion { get; set; }

    [XmlAttributeAttribute()]
    public string RfcEmisorCtaOrd { get; set; }

    [XmlAttributeAttribute()]
    public string NomBancoOrdExt { get; set; }

    [XmlAttributeAttribute()]
    public string CtaOrdenante { get; set; }

    [XmlAttributeAttribute()]
    public string RfcEmisorCtaBen { get; set; }

    [XmlAttributeAttribute()]
    public string CtaBeneficiario { get; set; }

    [XmlAttributeAttribute()]
    public c_TipoCadenaPago TipoCadPago { get; set; }


}

public partial class PagosPagoDoctoRelacionado33
{
    [XmlElementAttribute()]
    public PagosPagoDoctoRelacionadoImpuestosDR ImpuestosDR { get { return null; } set { } }

    [XmlAttributeAttribute()]
    public string IdDocumento { get; set; }

    [XmlAttributeAttribute()]
    public string Serie { get; set; }

    [XmlAttributeAttribute()]
    public string Folio { get; set; }

    [XmlAttributeAttribute()]
    public c_Moneda MonedaDR { get; set; }

    [XmlAttributeAttribute()]
    public decimal EquivalenciaDR { get; set; }

    [XmlAttributeAttribute()]
    public decimal TipoCambioDR { get; set; }

    [XmlAttributeAttribute()]
    public string NumParcialidad { get; set; }

    [XmlAttributeAttribute()]
    public decimal ImpSaldoAnt { get; set; }

    [XmlAttributeAttribute()]
    public decimal ImpPagado { get; set; }

    [XmlAttributeAttribute()]
    public decimal ImpSaldoInsoluto { get; set; }

    [XmlAttributeAttribute()]
    public c_ObjetoImp ObjetoImpDR { get; set; }

}







