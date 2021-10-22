using APBox.Models;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;

namespace APBox
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // se agrega para cambiar el nombre de las tablas predeterminadas de ASP.net Identity
            Database.SetInitializer<ApplicationDbContext>(null);
        }
    }
}
