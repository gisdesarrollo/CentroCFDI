using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace APBox.Models
{
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("APBox")
        {
        }
        //cambia el nombre de las tablas predeterminadas generadas por ASP.net Identity
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserClaim>().ToTable("aspnetuserclaims");
            modelBuilder.Entity<IdentityUserRole>().ToTable("aspnetuserroles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("aspnetuserlogins");
            modelBuilder.Entity<IdentityRole>().ToTable("aspnetroles");
            modelBuilder.Entity<ApplicationUser>().ToTable("aspnetusers");

        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

}