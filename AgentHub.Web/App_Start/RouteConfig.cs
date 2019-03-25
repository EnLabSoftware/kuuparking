using System.Web.Mvc;
using System.Web.Routing;

namespace AgentHub.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = false;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             "Default",
             "{*FriendlyUrl}"
            ).RouteHandler = new FriendlyUrlRouteHandler();
        }
    }
}
