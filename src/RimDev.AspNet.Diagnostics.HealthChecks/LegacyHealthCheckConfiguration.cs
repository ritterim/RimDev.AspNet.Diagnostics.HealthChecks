using Microsoft.Extensions.Diagnostics.HealthChecks;
using RimDev.AspNet.Diagnostics.HealthChecks.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    public static class LegacyHealthCheckConfiguration
    {
        private static readonly ConcurrentDictionary<string, HealthCheckConfiguration> RouteConfigs
            = new ConcurrentDictionary<string, HealthCheckConfiguration>(StringComparer.InvariantCultureIgnoreCase);

        public static void UseHealthChecks(string route, params IHealthCheck[] healthChecks)
        {
            var options = DefaultOptions();

            UseHealthChecks(route, options, healthChecks);
        }

        public static void UseHealthChecks(string route, HealthCheckOptions options, params IHealthCheck[] healthChecks)
        {
            if (healthChecks == null)
            {
                throw new ArgumentNullException(nameof(healthChecks));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            UseHealthChecks(route, options, healthChecks.Select(healthCheck => new HealthCheckWrapper(healthCheck)).ToArray());
        }

        public static void UseHealthChecks(string route, Func<IEnumerable<IHealthCheck>> collect)
        {
            var options = DefaultOptions();

            UseHealthChecks(route, options, collect);
        }

        public static void UseHealthChecks(string route, HealthCheckOptions options, Func<IEnumerable<IHealthCheck>> collect)
        {
            if (collect == null)
            {
                throw new ArgumentNullException(nameof(collect));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            UseHealthChecks(route, options, () => collect().Select(healthCheck => new HealthCheckWrapper(healthCheck)).ToArray());
        }

        public static void UseHealthChecks(string route, params HealthCheckWrapper[] healthChecks)
        {
            var options = DefaultOptions();

            UseHealthChecks(route, options, healthChecks);
        }

        public static void UseHealthChecks(string route, HealthCheckOptions options, params HealthCheckWrapper[] healthChecks)
        {
            if (healthChecks == null)
            {
                throw new ArgumentNullException(nameof(healthChecks));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var config = new HealthCheckConfiguration
            {
                HealthChecks = healthChecks,
                CollectHealthChecks = null,

                Options = options
            };

            UseHealthChecks(route, config);
        }

        public static void UseHealthChecks(string route, Func<IEnumerable<HealthCheckWrapper>> collect)
        {
            var options = DefaultOptions();

            UseHealthChecks(route, options, collect);
        }

        public static void UseHealthChecks(string route, HealthCheckOptions options, Func<IEnumerable<HealthCheckWrapper>> collect)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (collect == null)
            {
                throw new ArgumentNullException(nameof(collect));
            }

            var config = new HealthCheckConfiguration
            {
                HealthChecks = null,
                CollectHealthChecks = collect,

                Options = options
            };

            UseHealthChecks(route, config);
        }

        internal static HealthCheckConfiguration TryGetForRoute(string route)
        {
            RouteConfigs.TryGetValue(route, out var config);

            return config;
        }

        internal static void UseHealthChecks(string route, HealthCheckConfiguration config)
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