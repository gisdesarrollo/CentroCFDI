namespace API.Models.Spei
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SPEI_Tercero
    {

        private SPEI_TerceroBeneficiario beneficiarioField;

        private SPEI_TerceroOrdenante ordenanteField;

        private System.DateTime fechaOperacionField;

        private bool fechaOperacionFieldSpecified;

        private System.DateTime horaField;

        private bool horaFieldSpecified;

        private int claveSPEIField;

        private bool claveSPEIFieldSpecified;

        private string selloField;

        private string numeroCertificadoField;

        private bool numeroCertificadoFieldSpecified;

        private string cadenaCDAField;

        /// <remarks/>
        public SPEI_TerceroBeneficiario Beneficiario
        {
            get
            {
                return this.beneficiarioField;
            }
            set
            {
                this.beneficiarioField = value;
            }
        }

        /// <remarks/>
        public SPEI_TerceroOrdenante Ordenante
        {
            get
            {
                return this.ordenanteField;
            }
            set
            {
                this.ordenanteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime FechaOperacion
        {
            get
            {
                return this.fechaOperacionField;
            }
            set
            {
                this.fechaOperacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FechaOperacionSpecified
        {
            get
            {
                return this.fechaOperacionFieldSpecified;
            }
            set
            {
                this.fechaOperacionFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "time")]
        public System.DateTime Hora
        {
            get
            {
                return this.horaField;
            }
            set
            {
                this.horaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool HoraSpecified
        {
            get
            {
                return this.horaFieldSpecified;
            }
            set
            {
                this.horaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ClaveSPEI
        {
            get
            {
                return this.claveSPEIField;
            }
            set
            {
                this.claveSPEIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ClaveSPEISpecified
        {
            get
            {
                return this.claveSPEIFieldSpecified;
            }
            set
            {
                this.claveSPEIFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string sello
        {
            get
            {
                return this.selloField;
            }
            set
            {
                this.selloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string numeroCertificado
        {
            get
            {
                return this.numeroCertificadoField;
            }
            set
            {
                this.numeroCertificadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool numeroCertificadoSpecified
        {
            get
            {
                return this.numeroCertificadoFieldSpecified;
            }
            set
            {
                this.numeroCertificadoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cadenaCDA
        {
            get
            {
                return this.cadenaCDAField;
            }
            set
            {
                this.cadenaCDAField = value;
            }
        }
    }
}
