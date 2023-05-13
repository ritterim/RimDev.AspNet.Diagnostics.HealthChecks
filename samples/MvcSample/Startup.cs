using HealthChecks.Network;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Owin;
using RimDev.AspNet.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using RimDev.AspNet.Diagnostics.HealthChecks.UI;

namespace MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // app.UseHealthChecks(
            //     "/_health",
            //     new NoopHealthCheck(),
            //     //new SlowNoopHealthCheck(),
            //     //new SqlServerHealthCheck(
            //     //    @"Data Source=(LocalDB)\v13.0;Integrated Security=True;Initial Catalog=master",
            //     //    "select 'a'"),
            //     new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)));
            // 
            // // Sample with named checks for Health Check UI project
            // app.UseHealthChecks(
            //     "/_health_ui",
            //     new HealthCheckOptions
            //     {
            //         ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //     },
            //     new HealthCheckWrapper(new NoopHealthCheck(), "Noop health check"),
            //     new HealthCheckWrapper(new FailingHealthCheck(), "Failing health check"),
            //     new HealthCheckWrapper(new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)), "Ping to localhost"));

            LegacyHealthCheckConfiguration.Current.UseHealthChecks(
                new HealthCheckWrapper(new SlowNoopHealthCheck()),
                new HealthCheckWrapper(new NoopHealthCheck(), "Noop health check"),
                new HealthCheckWrapper(new FailingHealthCheck(), "Failing health check"),
                new HealthCheckWrapper(new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)), "Ping to localhost")
            );
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