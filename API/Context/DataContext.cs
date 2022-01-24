using API.Catalogos;
using API.Control;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.Facturacion;
using API.Operaciones.OperacionesProveedores;
using API.Relaciones;
using API.CatalogosCartaPorte;
using API.Operaciones.ComplementoCartaPorte;
using API.RelacionesCartaPorte;
using MySql.Data.Entity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using API.CatalogosCartaPorte.Domicilio;

namespace API.Context
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        public DataContext()
            : base("APBox")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioSucursal>()
                        .HasRequired(s => s.Usuario)
                        .WithMany()
                        .HasForeignKey(s => s.UsuarioId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<PagoProveedor>()
                        .HasRequired(s => s.Proveedor)
                        .WithMany()
                        .HasForeignKey(s => s.ProveedorId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Perfil>()
                        .HasRequired(s => s.Grupo)
                        .WithMany()
                        .HasForeignKey(s => s.GrupoId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pago>()
                        .HasOptional(s => s.BancoBeneficiario)
                        .WithMany()
                        .HasForeignKey(s => s.BancoBeneficiarioId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<ComplementoPago>()
                        .HasRequired(s => s.Sucursal)
                        .WithMany()
                        .HasForeignKey(s => s.SucursalId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sucursal>()
                        .HasRequired(s => s.Grupo)
                        .WithMany()
                        .HasForeignKey(s => s.GrupoId)
                        .WillCascadeOnDelete(false);
            
            //complemento carta porte delete on update

            modelBuilder.Entity<ComplementoPago>()
                        .HasRequired(s => s.Receptor)
                        .WithMany()
                        .HasForeignKey(s => s.ReceptorId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<CentroCosto>()
                        .HasRequired(s => s.Sucursal)
                        .WithMany()
                        .HasForeignKey(s => s.SucursalId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocumentoRelacionado>()
                        .HasRequired(s => s.Pago)
                        .WithMany(s => s.DocumentosRelacionados)
                        .HasForeignKey(s => s.PagoId)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<Pago>()
                        .HasRequired(s => s.Sucursal)
                        .WithMany()
                        .HasForeignKey(s => s.SucursalId)
                        .WillCascadeOnDelete(false);

          
        }

        #region Catalogos

        public DbSet<Banco> Bancos { get; set; }
        public DbSet<CentroCosto> CentrosCostos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        #endregion

        #region Control

        public DbSet<Configuracion> Configuraciones { get; set; }

        #endregion

        #region Operaciones

        //Complementos de Pago
        public DbSet<ComplementoPago> ComplementosPago { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<DocumentoRelacionado> DocumentosRelacionados { get; set; }
        public DbSet<Impuesto> Impuestos { get; set; }
        public DbSet<Retencion> Retenciones { get; set; }
        public DbSet<Traslado> Traslados { get; set; }

        //Facturacion
        public DbSet<DocumentoExtranjero> DocumentosExtranjeros { get; set; }
        public DbSet<FacturaEmitida> FacturasEmitidas { get; set; }
        public DbSet<FacturaRecibida> FacturasRecibidas { get; set; }
        public DbSet<Validacion> Validaciones { get; set; }

        //OperacionesProveedores
        public DbSet<PagoProveedor> PagosProveedores { get; set; }
        public DbSet<SolicitudAcceso> SolicitudesAccesos { get; set; }

        #endregion

        #region Relaciones

        public DbSet<BancoCliente> BancosClientes { get; set; }
        public DbSet<BancoSucursal> BancosSucursales { get; set; }
        public DbSet<ProveedorSucursal> ProveedoresSucursales { get; set; }
        public DbSet<UsuarioSucursal> UsuariosSucursales { get; set; }

        #endregion

        #region CartaPorte

        //CatalogosCartaPorte

        public DbSet<RetencionCP> RetencionesCP { get; set; }
        public DbSet<TrasladoCP> TrasladosCP { get; set; }
        public DbSet<c_impuestoCFDI> ImpuestoCP { get; set; }
        public DbSet<FormaPagos> Cat_FormaPago { get; set; }
        public DbSet<Cat_Conceptos> Cat_Conceptos { get; set; }

        public DbSet<ClaveProdServ> claveProdServ { get; set; }
        public DbSet<Cat_SubImpuestoC> Cat_Impuestos { get; set; }
        public DbSet<Conceptos> Conceptos { get; set; }
        public DbSet<ClaveProdServCP> ClavesProdServCP { get; set; }
        public DbSet<ClaveProdSTCC> ClavesProdSTCC { get; set; }
        public DbSet<ClaveTipoCarga> ClavesTipoCarga { get; set; }
        public DbSet<ClaveUnidad> ClavesUnidad { get; set; }
        public DbSet<ClaveUnidadPeso> ClavesUnidadPeso { get; set; }
        public DbSet<CodigoTransporteAereo> CodigosTransporteAereo { get; set; }
        public DbSet<ConfigAutotransporte> ConfigAutotransportes { get; set; }
        public DbSet<ConfigMaritima> ConfigMaritimas { get; set; }
        public DbSet<Contenedor> Contenedores { get; set; }
        public DbSet<ContenedorMaritimo> ContenedoresMaritimos { get; set; }
        public DbSet<CveTransporte> CveTransportes { get; set; }
        public DbSet<DerechosDePaso> DerechosDePasos { get; set; }
        public DbSet<Estaciones> Estaciones { get; set; }
        public DbSet<FraccionArancelaria> FraccionesArancelarias { get; set; }
        public DbSet<MaterialPeligroso> MaterialesPeligrosos { get; set; }
        public DbSet<NumAutorizacionNaviero> NumAutorizacionNavieros { get; set; }
        public DbSet<SubTipoRem> SubTipoRems { get; set; }
        public DbSet<TipoCarro> TipoCarros { get; set; }
        public DbSet<TipoDeServicio> TipoDeServicios { get; set; }
        public DbSet<TipoEmbalaje> TipoEmbalajes { get; set; }
        public DbSet<TipoEstacion> TipoEstaciones { get; set; }
        public DbSet<TipoPermiso> TipoPermisos { get; set; }
        public DbSet<TipoRelacion> TiposRelaciones { get; set; }

        //CatalogosCartaPorte Domicilio
        public DbSet<Pais> Paises {get;set;}
        public DbSet<Estado> Estados {get;set;}
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Colonia> Colonias  { get; set; }
        public DbSet<CodigoPostal> CodigosPostales { get; set; }
        public DbSet<Localidad> Localidades { get; set; }

        //Complementos CartaPorte
        //public DbSet<Arrendatario> Arrendatarios { get; set; }
        public DbSet<AutoTransporte> AutoTransporte { get; set; }
        public DbSet<CantidadTransportada> CantidadTransportadas { get; set; }
        public DbSet<Carro> Carros { get; set; }
        public DbSet<ComplementoCartaPorte> ComplementoCartaPortes { get; set; }

        public DbSet<ContenedorM> ContenedoresM { get; set; }
        public DbSet<ContenedorC> ContenedoresC { get; set; }
        public DbSet<DerechosDePasos> DerechoDePasos { get; set; }
        public DbSet<DetalleMercancia> DetalleMercancias { get; set; }
        public DbSet<Domicilio> Domicilios { get; set; }
        public DbSet<IdentificacionVehicular> IdentificacionVehiculares { get; set; }
        public DbSet<Mercancia> Mercancia { get; set; }
        public DbSet<Mercancias> Mercancias { get; set; }
        //public DbSet<Notificado> Notificados { get; set; }
        //public DbSet<Operador> Operadores { get; set; }
        public DbSet<PartesTransporte> PartesTransporte { get; set; }
        public DbSet<Pedimentos> Pedimentos { get; set; }
        //public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Remolques> Remolques { get; set; }
        public DbSet<Seguros> Seguros { get; set; }
        public DbSet<TiposFigura> Tiposfigura { get; set; }
        public DbSet<TransporteAereo> TransporteAereos { get; set; }
        public DbSet<TransporteFerroviario> TransporteFerroviarios { get; set; }
        public DbSet<TransporteMaritimo> TransporteMaritimos { get; set; }
        public DbSet<Ubicacion> UbicacionOrigen { get; set; }
       // public DbSet<UbicacionDestino> UbicacionesDestino { get; set; }

        /*public DbSet<UbicacionOrigen> UbuicacionesOrigen { get; set; }*/
        public DbSet<UsoCfdi> UsoCfdis { get; set; }

       // public DbSet<SubImpuestoC> SubImpuestoC { get; set; }
        public DbSet<GuiasIdentificacion> GuiasIdentificacion { get; set; }

        //Relaciones CartaPorte
        //public DbSet<AutotransporteRemolque> AutotransporteFederalRemolques { get; set; }
        //public DbSet<CarroContenedorC> CarrosContenedorC { get; set; }

        //public DbSet<ConceptoSubImpuestoConcepto> ConceptoSubImpuestoConcepto { get; set; }

        //public DbSet<ComplementoCartaPorteConceptos> ComplementoCartaPorteConceptos { get; set; }
        //public DbSet<ComplementoCartaPorteUbicacion> ComplementoCartaPorteUbicaciones { get; set; }
        //public DbSet<ComplementoCartaPorteFiguraTransporte> ComplementoCartaPorteFiguraTransporte { get; set; }
        //public DbSet<TransporteMaritimoContenedorM> TransporteMaritimoContenedoresM { get; set; }
        //public DbSet<MercanciaCantidadTransportada> MercanciaCantidadTransportadas { get; set; }
        //public DbSet<MercanciaGuiasIdentificacion> MercanciasGuiasIdentificacion { get; set; }
        //public DbSet<MercanciaPedimentos> MercanciaPedimentos { get; set; }
        //public DbSet<MercanciasMercancia> MercanciasMercancias  { get; set; }
        //public DbSet<TiposFiguraPartesTransporte>TiposFiguraPartesTransporte { get; set; }
        //public DbSet<TransporteFerroviarioDerechosDePaso> TransporteFerroviarioDerechosDePasos  { get; set; }
        //public DbSet<TransporteFerroviarioCarro> TransporteFerroviarioCarros { get; set; }

       // public DbSet<MercanciaDetalleMercancia> MercanciaDetalleMercancia { get; set; }
        #endregion



    }
}
