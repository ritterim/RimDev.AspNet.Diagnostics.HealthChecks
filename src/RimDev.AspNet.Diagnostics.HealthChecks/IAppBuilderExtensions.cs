using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Owin;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    public static class IAppBuilderExtensions
    {
        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            ILogger logger,
            HealthCheckOptions options,
            params HealthCheckWrapper[] healthChecks)
        {
            if (logger == null)
            {
                var loggerFactory = new LoggerFactory();
                logger = loggerFactory.CreateLogger(nameof(HealthCheckMiddleware));
            }

            app.Map(url, appBuilder =>
            {
                appBuilder.Use<HealthCheckMiddleware>(
                    logger,
                    options,
                    healthChecks);
            });
        }
        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            ILogger logger,
            HealthCheckOptions options,
            params IHealthCheck[] healthChecks)
        {
            if (logger == null)
            {
                var loggerFactory = new LoggerFactory();
                logger = loggerFactory.CreateLogger(nameof(HealthCheckMiddleware));
            }

            app.Map(url, appBuilder =>
            {
                appBuilder.Use<HealthCheckMiddleware>(
                    logger,
                    options,
                    healthChecks);
            });
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            HealthCheckOptions options,
            params IHealthCheck[] healthChecks)
        {
            UseHealthChecks(app, url, null, options, healthChecks);
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            params IHealthCheck[] healthChecks)
        {
            UseHealthChecks(app, url, new HealthCheckOptions(), healthChecks);
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            HealthCheckOptions options,
            IEnumerable<IHealthCheck> healthChecks)
        {
            UseHealthChecks(app, url, options, healthChecks.ToArray());
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            IEnumerable<IHealthCheck> healthChecks)
        {
            UseHealthChecks(app, url, new HealthCheckOptions(), healthChecks.ToArray());
        }
      public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            params HealthCheckWrapper[] healthChecks)
        {
            UseHealthChecks(app, url, new HealthCheckOptions(), healthChecks);
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            HealthCheckOptions options,
            IEnumerable<HealthCheckWrapper> healthChecks)
        {
            UseHealthChecks(app, url, null, options, healthChecks.ToArray());
        }
    }
}