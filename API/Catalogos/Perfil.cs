using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Enums;
using System.ComponentModel;

namespace API.Catalogos
{
    [Table("cat_perfiles")]
    public class Perfil
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Nombre { get; set; }

        public Status Status { get; set; }

        //Catalogos
        public bool Grupos { get; set; }

        public bool Sucursales { get; set; }

        public bool Departamentos { get; set; }

        [DisplayName("Centros de Costos")]
        public bool CentrosCostos { get; set; }

        public bool Bancos { get; set; }

        public bool Perfiles { get; set; }

        public bool Usuarios { get; set; }

        public bool Proveedores { get; set; }

        public bool Clientes { get; set; }

        //Operaciones Catalogos
        public bool Consulta { get; set; }

        [DisplayName("Inserción")]
        public bool Insercion { get; set; }

        [DisplayName("Edición")]
        public bool Edicion { get; set; }

        public bool Borrado { get; set; }

        //Recepción de CFDi
        [DisplayName("Autorizar Gastos Internos")]
        public bool AutorizarGastosInternos { get; set; }

        [DisplayName("Autorizar Gastos Proveedores")]
        public bool AutorizarGastosProveedores { get; set; }

        [DisplayName("Solicitudes de Acceso")]
        public bool SolicitudesAcceso { get; set; }

        [DisplayName("Validación Rápida")]
        public bool ValidacionRapida { get; set; }

        //Complementos de Pago
        [DisplayName("Facturas Emitidas")]
        public bool FacturasEmitidas { get; set; }

        [DisplayName("Generación Manual")]
        public bool GeneracionManual { get; set; }

        [DisplayName("Generación Layout")]
        public bool GeneracionLayout { get; set; }

        //Portales
        [DisplayName("Portal de Usuarios")]
        public bool PortalUsuarios { get; set; }

        [DisplayName("Portal de Proveedores")]
        public bool PortalProveedores { get; set; }

        //Reportes
        [DisplayName("Documentos")]
        public bool ReporteDocumentos { get; set; }

        [DisplayName("Gastos")]
        public bool ReporteGastos { get; set; }

        [DisplayName("Estadísticas por Usuario")]
        public bool ReporteEstadisticasPorUsuario { get; set; }

        [DisplayName("Estadísticas por Proveedor")]
        public bool ReporteEstadisticasPorProveedor { get; set; }

        [DisplayName("Estadísitcas de Complementos de Pago")]
        public bool ReporteEstadisticasComplementosPago { get; set; }

        #region Grupo

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Grupo")]
        public int GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }

        #endregion
        }
}
