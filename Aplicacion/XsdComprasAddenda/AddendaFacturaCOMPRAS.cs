
using System.Xml.Serialization;


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="https://www.interfactura.com/Schemas/Documentos", IsNullable=false)]
public partial class FacturaInterfactura {
    
    private FacturaInterfacturaEmisor emisorField;
    
    private FacturaInterfacturaReceptor receptorField;
    
    private FacturaInterfacturaEncabezado encabezadoField;
    
    private t_TipoDocumento tipoDocumentoField;
    
    private System.Xml.XmlAttribute[] anyAttrField;
    
    /// <remarks/>
    public FacturaInterfacturaEmisor Emisor {
        get {
            return this.emisorField;
        }
        set {
            this.emisorField = value;
        }
    }
    
    /// <remarks/>
    public FacturaInterfacturaReceptor Receptor {
        get {
            return this.receptorField;
        }
        set {
            this.receptorField = value;
        }
    }
    
    /// <remarks/>
    public FacturaInterfacturaEncabezado Encabezado {
        get {
            return this.encabezadoField;
        }
        set {
            this.encabezadoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public t_TipoDocumento TipoDocumento {
        get {
            return this.tipoDocumentoField;
        }
        set {
            this.tipoDocumentoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyAttributeAttribute()]
    public System.Xml.XmlAttribute[] AnyAttr {
        get {
            return this.anyAttrField;
        }
        set {
            this.anyAttrField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
public partial class FacturaInterfacturaEmisor {
    
    private string riField;
    
    private System.Xml.XmlAttribute[] anyAttrField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RI {
        get {
            return this.riField;
        }
        set {
            this.riField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyAttributeAttribute()]
    public System.Xml.XmlAttribute[] AnyAttr {
        get {
            return this.anyAttrField;
        }
        set {
            this.anyAttrField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
public partial class FacturaInterfacturaReceptor {
    
    private string riField;
    
    private System.Xml.XmlAttribute[] anyAttrField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string RI {
        get {
            return this.riField;
        }
        set {
            this.riField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyAttributeAttribute()]
    public System.Xml.XmlAttribute[] AnyAttr {
        get {
            return this.anyAttrField;
        }
        set {
            this.anyAttrField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
public partial class FacturaInterfacturaEncabezado {
    
    private FacturaInterfacturaEncabezadoCuerpo[] cuerpoField;
    
    private FacturaInterfacturaEncabezadoTipoProveedorEKT tipoProveedorEKTField;
    
    private System.DateTime fechaField;
    
    private FacturaInterfacturaEncabezadoMonedaDoc monedaDocField;
    
    private decimal subTotalField;
    
    private FacturaInterfacturaEncabezadoIVAPCT iVAPCTField;
    
    private decimal ivaField;
    
    private decimal totalField;
    
    private string numProveedorField;
    
    private string folioOrdenCompraField;
    
    private string observacionesField;
    
    private System.Xml.XmlAttribute[] anyAttrField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Cuerpo")]
    public FacturaInterfacturaEncabezadoCuerpo[] Cuerpo {
        get {
            return this.cuerpoField;
        }
        set {
            this.cuerpoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public FacturaInterfacturaEncabezadoTipoProveedorEKT TipoProveedorEKT {
        get {
            return this.tipoProveedorEKTField;
        }
        set {
            this.tipoProveedorEKTField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime Fecha {
        get {
            return this.fechaField;
        }
        set {
            this.fechaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public FacturaInterfacturaEncabezadoMonedaDoc MonedaDoc {
        get {
            return this.monedaDocField;
        }
        set {
            this.monedaDocField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal SubTotal {
        get {
            return this.subTotalField;
        }
        set {
            this.subTotalField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public FacturaInterfacturaEncabezadoIVAPCT IVAPCT {
        get {
            return this.iVAPCTField;
        }
        set {
            this.iVAPCTField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Iva {
        get {
            return this.ivaField;
        }
        set {
            this.ivaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Total {
        get {
            return this.totalField;
        }
        set {
            this.totalField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NumProveedor {
        get {
            return this.numProveedorField;
        }
        set {
            this.numProveedorField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string FolioOrdenCompra {
        get {
            return this.folioOrdenCompraField;
        }
        set {
            this.folioOrdenCompraField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Observaciones {
        get {
            return this.observacionesField;
        }
        set {
            this.observacionesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyAttributeAttribute()]
    public System.Xml.XmlAttribute[] AnyAttr {
        get {
            return this.anyAttrField;
        }
        set {
            this.anyAttrField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
public partial class FacturaInterfacturaEncabezadoCuerpo {
    
    private string renglonField;
    
    private decimal cantidadField;
    
    private string conceptoField;
    
    private decimal pUnitarioField;
    
    private decimal importeField;
    
    private System.Xml.XmlAttribute[] anyAttrField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
    public string Renglon {
        get {
            return this.renglonField;
        }
        set {
            this.renglonField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Cantidad {
        get {
            return this.cantidadField;
        }
        set {
            this.cantidadField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Concepto {
        get {
            return this.conceptoField;
        }
        set {
            this.conceptoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal PUnitario {
        get {
            return this.pUnitarioField;
        }
        set {
            this.pUnitarioField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Importe {
        get {
            return this.importeField;
        }
        set {
            this.importeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyAttributeAttribute()]
    public System.Xml.XmlAttribute[] AnyAttr {
        get {
            return this.anyAttrField;
        }
        set {
            this.anyAttrField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
public enum FacturaInterfacturaEncabezadoTipoProveedorEKT {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("3")]
    Item3,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
public enum FacturaInterfacturaEncabezadoMonedaDoc {
    
    /// <remarks/>
    MXN,
    
    /// <remarks/>
    USD,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://www.interfactura.com/Schemas/Documentos")]
public enum FacturaInterfacturaEncabezadoIVAPCT {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("15")]
    Item15,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("10")]
    Item10,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="https://www.interfactura.com/Schemas/Documentos")]
public enum t_TipoDocumento {
    
    /// <remarks/>
    Factura,
}
