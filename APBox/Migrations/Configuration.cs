namespace APBox.Migrations
{
    using APBox.Models;
    using MySql.Data.Entity;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.APBoxContext>
    {
        public Configuration()
        {
            //Add-Migration FKNotes;
            AutomaticMigrationsEnabled = true;
            //SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
            //se agrega new dbMigrationSQLGenerator para resolver error del Hash al crear un index en tablas mysql
            SetSqlGenerator("MySql.Data.MySqlClient", new dbMigrationSQLGenerator());
            CodeGenerator = new MySqlMigrationCodeGenerator();/*new MySql.Data.Entity.MySqlMigrationCodeGenerator();*/
            AutomaticMigrationDataLossAllowed = true;  // or false in case data loss is not allowed.
        }

        protected override void Seed(Context.APBoxContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
