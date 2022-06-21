
using API.Enums;
using API.Enums.CartaPorteEnums;
using System;
//using CFDI.API.Enums.CFDI33;
using System.Xml.Schema;
using System.Xml.Serialization;



[XmlRootAttribute(Namespace = "http://www.sat.gob.mx/cfd/4")]
public partial class Comprobante {

    [XmlAttributeAttribute()]
    public string Version { get; set; }

    [XmlAttributeAttribute()]
    public string Folio { get; set; }

    [XmlAttributeAttribute()]
    public string Fecha { get; set; }

    [XmlAttributeAttribute()]
    public string Sello { get; set; }

    [XmlAttributeAttribute()]
    public string FormaDePago { get; set; }

    [XmlAttributeAttribute()]
    public string NoCertificado { get; set; }

    [XmlAttributeAttribute()]
    public string Certificado { get; set; }

    [XmlAttributeAttribute()]
    public string SubTotal { get; set; }

    [XmlAttributeAttribute()]
    public string Moneda { get; set; }

    [XmlAttributeAttribute()]
    public string Total { get; set; }

    [XmlAttributeAttribute()]
    public string TipoDeComprobante { get; set; }

    [XmlAttributeAttribute()]
    public string MetodoDePago { get; set; }

    [XmlAttributeAttribute()]
    public string LugarExpedicion { get; set; }

   // [XmlElementAttribute()]
 //   public TEmisor Emisor { get; set; }

  //  [XmlElementAttribute()]
//    public TReceptor Receptor { get; set; }

    [XmlArrayItemAttribute("Concepto")]
    public TConcepto[] Conceptos { get; set; }

    [XmlElementAttribute()]
    public TImpuestos Impuestos { get; set; }

}

/*public partial class TEmisor
{
    [XmlAttributeAttribute()]
    public string rfc;

    [XmlAttributeAttribute()]
    public string nombre;

    [XmlElementAttribute()]
    public TDomicilioFiscal DomicilioFiscal;

    [XmlElementAttribute()]
    public TRegimenFiscal[] RegimenFiscal;

}

public partial class TDomicilioFiscal
{
    [XmlAttributeAttribute()]
    public string calle;

    [XmlAttributeAttribute()]
    public string municipio;

    [XmlAttributeAttribute()]
    public string estado;

    [XmlAttributeAttribute()]
    public string pais;

    [XmlAttributeAttribute()]
    public string codigoPostal;

}

public partial class TRegimenFiscal
{
    [XmlAttributeAttribute()]
    public string Regimen;

}

public partial class TReceptor
{
    [XmlAttributeAttribute()]
    public string rfc;

    [XmlAttributeAttribute()]
    public string nombre;

    [XmlElementAttribute()]
    public TDomicilio Domicilio;
}

public partial class TDomicilio
{
    [XmlAttributeAttribute()]
    public string calle;

    [XmlAttributeAttribute()]
    public string noExterior;

    [XmlAttributeAttribute()]
    public string colonia;

    [XmlAttributeAttribute()]
    public string municipio;

    [XmlAttributeAttribute()]
    public string estado;

    [XmlAttributeAttribute()]
    public string pais;

    [XmlAttributeAttribute()]
    public string codigoPostal;
}*/

public partial class TConcepto
{
    [XmlAttributeAttribute()]
    public string cantidad;

    [XmlAttributeAttribute()]
    public string unidad;

    [XmlAttributeAttribute()]
    public string descripcion;

    [XmlAttributeAttribute()]
    public string valorUnitario;

    [XmlAttributeAttribute()]
    public string importe;

    [XmlElementAttribute()]
    public TImpuestos Impuestos { get; set; }

}

public partial class TImpuestos
{
    [XmlArrayItemAttribute("Traslado")]
    public ITraslado[] Traslados { get; set; }

    [XmlArrayItemAttribute("Retencion")]
    public IRetencion[] Retenciones { get; set; }
}

public partial class ITraslado
{
    [XmlAttributeAttribute()]
    public Decimal Base { get; set; }

    [XmlAttributeAttribute()]
    public string Impuesto { get; set; }

    [XmlAttributeAttribute()]
    public string TipoFactor { get; set; }

    [XmlAttributeAttribute()]
    public Decimal TasaOCuota { get; set; }

    [XmlAttributeAttribute()]
    public Decimal Importe { get; set; }

}
public partial class IRetencion
{
    [XmlAttributeAttribute()]
    public Decimal Base { get; set; }

    [XmlAttributeAttribute()]
    public string Impuesto { get; set; }

    [XmlAttributeAttribute()]
    public string TipoFactor { get; set; }

    [XmlAttributeAttribute()]
    public Decimal TasaOCuota { get; set; }

    [XmlAttributeAttribute()]
    public Decimal Importe { get; set; }

}












