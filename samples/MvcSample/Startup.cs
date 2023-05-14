using HealthChecks.Network;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Owin;
using RimDev.AspNet.Diagnostics.HealthChecks;
using RimDev.AspNet.Diagnostics.HealthChecks.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LegacyHealthCheckRoutes
                .MapHealthChecks("/health/plain")
                .WithOptions(new HealthCheckOptions()) // Plain response writer
                .WithOptions(options => options.AllowCachingResponses = false)
                .AddCheck<FailingHealthCheck>()
                .AddCheck<FailingHealthCheck>("Failing health check")
                .AddCheck(new FailingHealthCheck())
                .AddCheck(new FailingHealthCheck(), "Failing health check");

            LegacyHealthCheckRoutes
                .MapHealthChecks("/health/plain-ui")
                .AddCheck<FailingHealthCheck>()
                .AddCheck<FailingHealthCheck>("Failing health check")
                .AddCheck(new FailingHealthCheck())
                .AddCheck(new FailingHealthCheck(), "Failing health check");

            LegacyHealthCheckRoutes
                .MapDefaultHealthChecks() // /health
                .AddCheck<SlowNoopHealthCheck>()
                .AddChecks(
                    new NoopHealthCheck(),
                    new FailingHealthCheck(),
                    new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000))
                )
                .AddNamedChecks(() => new[]
                {
                    new NamedHealthCheck("Noop health check", new NoopHealthCheck()),
                    new NamedHealthCheck("Failing health check", new FailingHealthCheck()),
                    new NamedHealthCheck("Ping to localhost", new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)))
                });

            LegacyHealthCheckRoutes
                .MapHealthChecks("/health/noop")
                .AddCheck<NoopHealthCheck>("Noop health check");

            // Below example will throw due to reuse of route
            // LegacyHealthCheckRoutes
            //     .MapHealthChecks("health/noop") // Normalized to/health/noop - throws
            //     .AddCheck<NoopHealthCheck>("Noop health check");
        }
    }

    public class NoopHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy));
        }
    }

    public class SlowNoopHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Delay(1000);

            return new HealthCheckResult(HealthStatus.Healthy);
        }
    }

    public class FailingHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "This one is supposed to fail."));
        }
    }
}