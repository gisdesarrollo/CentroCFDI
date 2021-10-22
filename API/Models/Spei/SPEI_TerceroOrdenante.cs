namespace API.Models.Spei
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPEI_TerceroOrdenante
    {

        private string bancoEmisorField;

        private string nombreField;

        private sbyte tipoCuentaField;

        private bool tipoCuentaFieldSpecified;

        private long cuentaField;

        private bool cuentaFieldSpecified;

        private string rFCField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BancoEmisor
        {
            get
            {
                return this.bancoEmisorField;
            }
            set
            {
                this.bancoEmisorField = value;
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
