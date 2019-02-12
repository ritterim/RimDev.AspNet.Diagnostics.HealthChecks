using HealthChecks.Network;
using HealthChecks.SqlServer;
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
            app.UseHealthChecks(
                "/_health",
                new NoopHealthCheck(),
                //new SlowNoopHealthCheck(),
                //new SqlServerHealthCheck(
                //    @"Data Source=(LocalDB)\v13.0;Integrated Security=True;Initial Catalog=master",
                //    "select 'a'"),
                new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)));
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
}