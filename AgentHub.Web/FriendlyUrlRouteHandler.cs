using System.Web;
using AgentHub.Service;

namespace AgentHub.Web
{
    public class FriendlyUrlRouteHandler : System.Web.Mvc.MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
            var friendlyUrl = (string)requestContext.RouteData.Values["FriendlyUrl"];

            PageService.GetPageByFriendlyUrl(friendlyUrl);

            requestContext.RouteData.Values["controller"] = PageService.CurrentPageItem.ControllerName;
            requestContext.RouteData.Values["action"] = PageService.CurrentPageItem.ActionName;
            requestContext.RouteData.Values["id"] = PageService.CurrentPageItem.ID;
            
            return base.GetHttpHandler(requestContext);
        }
    }
}