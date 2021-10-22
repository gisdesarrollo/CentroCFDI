using API.Catalogos;
using API.Control;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.Facturacion;
using API.Operaciones.OperacionesProveedores;
using API.Relaciones;
using MySql.Data.Entity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace API.Context
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        public DataContext()
            :base ("APBox")
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

    }
}
