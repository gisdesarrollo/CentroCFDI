namespace API.Models.Spei
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPEI_TerceroBeneficiario
    {

        private string bancoReceptorField;

        private string nombreField;

        private sbyte tipoCuentaField;

        private bool tipoCuentaFieldSpecified;

        private long cuentaField;

        private bool cuentaFieldSpecified;

        private string rFCField;

        private string conceptoField;

        private sbyte iVAField;

        private bool iVAFieldSpecified;

        private float montoPagoField;

        private bool montoPagoFieldSpecified;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BancoReceptor
        {
            get
            {
                return this.bancoReceptorField;
            }
            set
            {
                this.bancoReceptorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Nombre
        {
            get
            {
                return this.nombreField;
            }
            set
            {
                this.nombreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public sbyte TipoCuenta
        {
            get
            {
                return this.tipoCuentaField;
            }
            set
            {
                this.tipoCuentaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TipoCuentaSpecified
        {
            get
            {
                return this.tipoCuentaFieldSpecified;
            }
            set
            {
                this.tipoCuentaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long Cuenta
        {
            get
            {
                return this.cuentaField;
            }
            set
            {
                this.cuentaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CuentaSpecified
        {
            get
            {
                return this.cuentaFieldSpecified;
            }
            set
            {
                this.cuentaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RFC
        {
            get
            {
                return this.rFCField;
            }
            set
            {
                this.rFCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Concepto
        {
            get
            {
                return this.conceptoField;
            }
            set
            {
                this.conceptoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public sbyte IVA
        {
            get
            {
                return this.iVAField;
            }
            set
            {
                this.iVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IVASpecified
        {
            get
            {
                return this.iVAFieldSpecified;
            }
            set
            {
                this.iVAFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float MontoPago
        {
            get
            {
                return this.montoPagoField;
            }
            set
            {
                this.montoPagoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MontoPagoSpecified
        {
            get
            {
                return this.montoPagoFieldSpecified;
            }
            set
            {
                this.montoPagoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
