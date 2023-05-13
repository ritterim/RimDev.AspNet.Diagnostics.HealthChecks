using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    public class HealthCheckHttpHandler : HttpTaskAsyncHandler
    {
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly RimDevAspNetHealthCheckService _healthCheckService;
        private readonly HealthCheckWrapper[] _healthChecks;

        public override bool IsReusable => false;

        public HealthCheckHttpHandler()
        {
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger(nameof(HealthCheckHttpHandler));

            var config = LegacyHealthCheckConfiguration.Current;

            _healthCheckOptions = config.HealthCheckOptions;
            _healthCheckService = new RimDevAspNetHealthCheckService(logger);
            _healthChecks = config.HealthChecks;
        }

        public override async Task ProcessRequestAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // Get results
            var result = await _healthCheckService.CheckHealthAsync(_healthChecks);

            // Map status to response code - this is customizable via options. 
            if (!_healthCheckOptions.ResultStatusCodes.TryGetValue(result.Status, out var statusCode))
            {
                var message =
                    $"No status code mapping found for {nameof(HealthStatus)} value: {result.Status}." +
                    $"{nameof(HealthCheckOptions)}.{nameof(HealthCheckOptions.ResultStatusCodes)} must contain" +
                    $"an entry for {result.Status}.";

                throw new InvalidOperationException(message);
            }
            httpContext.Response.StatusCode = statusCode;

            if (!_healthCheckOptions.AllowCachingResponses)
            {
                // Similar to: https://github.com/aspnet/Security/blob/7b6c9cf0eeb149f2142dedd55a17430e7831ea99/src/Microsoft.AspNetCore.Authentication.Cookies/CookieAuthenticationHandler.cs#L377-L379
                var headers = httpContext.Response.Headers;
                headers.Set("Cache-Control", "no-store, no-cache");
                headers.Set("Pragma", "no-cache");
                headers.Set("Expires", "Thu, 01 Jan 1970 00:00:00 GMT");
            }

            if (_healthCheckOptions.ResponseWriter != null)
            {
                _healthCheckOptions.ResponseWriter(httpContext, result);
            }
        }

        private static IHealthCheck[] FilterHealthChecks(
            IReadOnlyDictionary<string, IHealthCheck> checks,
            ISet<string> names)
        {
            // If there are no filters then include all checks.
            if (names.Count == 0)
            {
                return checks.Values.ToArray();
            }

            // Keep track of what we don't find so we can report errors.
            var notFound = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
            var matches = new List<IHealthCheck>();

            foreach (var kvp in checks)
            {
                if (!notFound.Remove(kvp.Key))
                {
                    // This check was excluded
                    continue;
                }

                matches.Add(kvp.Value);
            }

            if (notFound.Count > 0)
            {
                var message =
                    $"The following health checks were not found: '{string.Join(", ", notFound)}'. " +
                    $"Registered health checks: '{string.Join(", ", checks.Keys)}'.";
                throw new InvalidOperationException(message);
            }

            return matches.ToArray();
        }
    }
}