
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class RouteCollectionExtensions
    {
        public static void IgnoreHealthCheckRoutes(this RouteCollection routes)
        {
            routes.IgnoreRoute("health");
            routes.IgnoreRoute("health/{*path}");

            // TODO: How can we ignore `health-*` ?
        }
    }
}
