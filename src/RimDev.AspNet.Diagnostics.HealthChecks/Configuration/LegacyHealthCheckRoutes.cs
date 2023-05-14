using RimDev.AspNet.Diagnostics.HealthChecks.UI;
using System;
using System.Collections.Concurrent;

namespace RimDev.AspNet.Diagnostics.HealthChecks.Configuration
{
    public static class LegacyHealthCheckRoutes
    {
        public static readonly string DefaultHealthChecksRoute = "/health";

        private static readonly ConcurrentDictionary<string, LegacyHealthCheckRouteConfiguration> RouteConfigs
            = new ConcurrentDictionary<string, LegacyHealthCheckRouteConfiguration>(StringComparer.InvariantCultureIgnoreCase);

        public static LegacyHealthCheckRouteConfiguration MapDefaultHealthChecks()
        {
            return MapHealthChecks(DefaultHealthChecksRoute);
        }

        public static LegacyHealthCheckRouteConfiguration MapHealthChecks(string route)
        {
            //  health -> /health
            // \health -> /health
            // /health -> /health
            var normalizedRoute = $"/{route.TrimStart('/', '\\')}";
            var config = new LegacyHealthCheckRouteConfiguration();

            UseHealthChecks(normalizedRoute, config);

            return config;
        }

        public static LegacyHealthCheckRouteConfiguration TryGetConfiguration(string route)
        {
            RouteConfigs.TryGetValue(route, out var config);

            return config;
        }

        internal static void UseHealthChecks(string route, LegacyHealthCheckRouteConfiguration config)
        {
            RouteConfigs.AddOrUpdate(route, config, (_, existing) => config);
        }

        private static HealthCheckOptions DefaultOptions()
        {
            return new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            };
        }
    }
}