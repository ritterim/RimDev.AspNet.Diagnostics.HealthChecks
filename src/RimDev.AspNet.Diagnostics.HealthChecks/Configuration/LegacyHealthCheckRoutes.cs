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

        public static LegacyHealthCheckRouteConfiguration? TryGetConfiguration(string route, string? virtualDirectory)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"{nameof(route)} cannot be empty or whitespace!", nameof(route));
            }

            if (RouteConfigs.TryGetValue(route, out var config))
            {
                return config;
            }

            string? routeWithoutVirtualDirectory = virtualDirectory != null && !string.IsNullOrWhiteSpace(virtualDirectory) && route.StartsWith(virtualDirectory, StringComparison.InvariantCultureIgnoreCase)
                ? route.Substring(virtualDirectory.Length)
                : null;

            if (routeWithoutVirtualDirectory != null && RouteConfigs.TryGetValue(routeWithoutVirtualDirectory, out var configWithoutVirtualDirectory))
            {
                return configWithoutVirtualDirectory;
            }

            return null;
        }

        public static LegacyHealthCheckRouteConfiguration MapDefaultHealthChecks()
        {
            return MapHealthChecks(DefaultHealthChecksRoute);
        }

        public static LegacyHealthCheckRouteConfiguration MapHealthChecks(string route)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"{nameof(route)} cannot be empty or whitespace!", nameof(route));
            }

            //  health -> /health
            // \health -> /health
            // /health -> /health
            var normalizedRoute = $"/{route.TrimStart('/', '\\').Trim()}";
            var config = new LegacyHealthCheckRouteConfiguration();

            if (!RouteConfigs.TryAdd(normalizedRoute, config))
            {
                throw new ArgumentException($"There is already a health check configuration mapped to route '{normalizedRoute}'!");
            }

            return config;
        }
    }
}