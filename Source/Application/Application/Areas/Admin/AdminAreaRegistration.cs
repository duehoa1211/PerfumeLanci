using System.Web.Mvc;

namespace Application.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new
                {
                    controller = "Admin",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
            context.MapRoute(
                "Login_default",
                "loginadmin/",
                new
                {
                    controller = "Login",
                    action = "Index"
                }
            );
        }
    }
}