using HealthChecks.Network;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Owin;
using RimDev.AspNet.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LegacyHealthCheckConfiguration.UseHealthChecks(
                "/health/plain",
                new HealthCheckOptions(),
                new FailingHealthCheck()
            );

            LegacyHealthCheckConfiguration.UseHealthChecks("/health", () => new[]
            {
                new HealthCheckWrapper(new SlowNoopHealthCheck()),
                new HealthCheckWrapper(new NoopHealthCheck(), "Noop health check"),
                new HealthCheckWrapper(new FailingHealthCheck(), "Failing health check"),
                new HealthCheckWrapper(new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)), "Ping to localhost")
            });

            LegacyHealthCheckConfiguration.UseHealthChecks("/health/noop", () => new[]
            {
                new HealthCheckWrapper(new NoopHealthCheck(), "Noop health check"),
            });
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