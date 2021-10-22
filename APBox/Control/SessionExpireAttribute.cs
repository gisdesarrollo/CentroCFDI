using System.Web;
using System.Web.Mvc;

namespace APBox.Control
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["UsuarioId"] == null || HttpContext.Current.Session["SucursalId"] == null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.User = null;
                System.Web.Security.FormsAuthentication.SignOut();

                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}