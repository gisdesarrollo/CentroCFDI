using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.CARTAPORTE20Xsd
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/CartaPorte20")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sat.gob.mx/CartaPorte20", IsNullable = false)]
    public partial class CartaPorte
    {

        private CartaPorteUbicacion[] ubicacionesField;

        private CartaPorteMercancias mercanciasField;

        private CartaPorteTiposFigura[] figuraTransporteField;

        private string versionField;

        private string idCCPField;

        private string transpInternacField;

        private string regimenAduaneroField;

        private string entradaSalidaMercField;

        private bool entradaSalidaMercFieldSpecified;

        private string paisOrigenDestinoField;

        private bool paisOrigenDestinoFieldSpecified;

        private string viaEntradaSalidaField;

        private bool viaEntradaSalidaFieldSpecified;

        private decimal totalDistRecField;

        private bool totalDistRecFieldSpecified;

        private string registroISTMOField;

        private bool registroISTMOFieldSpecified;

        private string ubicacionPoloOrigenField;

        private bool ubicacionPoloOrigenFieldSpecified;

        private string ubicacionPoloDestinoField;

        private bool ubicacionPoloDestinoFieldSpecified;

        public CartaPorte()
        {
            this.versionField = "3.0";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Ubicacion", IsNullable = false)]
        public CartaPorteUbicacion[] Ubicaciones
        {
            get
            {
                return this.ubicacionesField;
            }
            set
            {
                this.ubicacionesField = value;
            }
        }

        /// <remarks/>
        public CartaPorteMercancias Mercancias
        {
            get
            {
                return this.mercanciasField;
            }
            set
            {
                this.mercanciasField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("TiposFigura", IsNullable = false)]
        public CartaPorteTiposFigura[] FiguraTransporte
        {
            get
            {
                return this.figuraTransporteField;
            }
            set
            {
                this.figuraTransporteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IdCCP
        {
            get
            {
                return this.idCCPField;
            }
            set
            {
                this.idCCPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TranspInternac
        {
            get
            {
                return this.transpInternacField;
            }
            set
            {
                this.transpInternacField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegimenAduanero
        {
            get
            {
                return this.regimenAduaneroField;
            }
            set
            {
                this.regimenAduaneroField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EntradaSalidaMerc
        {
            get
            {
                return this.entradaSalidaMercField;
            }
            set
            {
                entradaSalidaMercFieldSpecified = true;
                this.entradaSalidaMercField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EntradaSalidaMercSpecified
        {
            get
            {
                return this.entradaSalidaMercFieldSpecified;
            }
            set
            {
                this.entradaSalidaMercFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PaisOrigenDestino
        {
            get
            {
                return this.paisOrigenDestinoField;
            }
            set
            {
                paisOrigenDestinoFieldSpecified = true;
                this.paisOrigenDestinoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PaisOrigenDestinoSpecified
        {
            get
            {
                return this.paisOrigenDestinoFieldSpecified;
            }
            set
            {
                this.paisOrigenDestinoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ViaEntradaSalida
        {
            get
            {
                return this.viaEntradaSalidaField;
            }
            set
            {
                viaEntradaSalidaFieldSpecified = true;
                this.viaEntradaSalidaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ViaEntradaSalidaSpecified
        {
            get
            {
                return this.viaEntradaSalidaFieldSpecified;
            }
            set
            {
                this.viaEntradaSalidaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalDistRec
        {
            get
            {
                return this.totalDistRecField;
            }
            set
            {
                totalDistRecFieldSpecified = true;
                this.totalDistRecField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalDistRecSpecified
        {
            get
            {
                return this.totalDistRecFieldSpecified;
            }
            set
            {
                this.totalDistRecFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegistroISTMO
        {
            get
            {
                return this.registroISTMOField;
            }
            set
            {
                this.registroISTMOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RegistroISTMOSpecified
        {
            get
            {
                return this.registroISTMOFieldSpecified;
            }
            set
            {
                this.registroISTMOFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UbicacionPoloOrigen
        {
            get
            {
                return this.ubicacionPoloOrigenField;
            }
            set
            {
                this.ubicacionPoloOrigenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UbicacionPoloOrigenSpecified
        {
            get
            {
                return this.ubicacionPoloOrigenFieldSpecified;
            }
            set
            {
                this.ubicacionPoloOrigenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UbicacionPoloDestino
        {
            get
            {
                return this.ubicacionPoloDestinoField;
            }
            set
            {
                this.ubicacionPoloDestinoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UbicacionPoloDestinoSpecified
        {
            get
            {
                return this.ubicacionPoloDestinoFieldSpecified;
            }
            set
            {
                this.ubicacionPoloDestinoFieldSpecified = value;
            }
        }
    }




}
