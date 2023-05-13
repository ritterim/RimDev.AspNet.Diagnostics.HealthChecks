using Microsoft.Extensions.Diagnostics.HealthChecks;
using RimDev.AspNet.Diagnostics.HealthChecks.UI;
using System;
using System.Linq;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    public class HealthCheckConfiguration
    {
        public HealthCheckOptions HealthCheckOptions { get; private set; } = new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        };

        public HealthCheckWrapper[] HealthChecks { get; private set; } = Array.Empty<HealthCheckWrapper>();

        public HealthCheckConfiguration UseHealthCheckOptions(HealthCheckOptions healthCheckOptions)
        {
            if (healthCheckOptions == null)
            {
                throw new ArgumentNullException(nameof(healthCheckOptions));
            }

            this.HealthCheckOptions = healthCheckOptions;

            return this;
        }

        public HealthCheckConfiguration UseHealthChecks(params IHealthCheck[] healthChecks)
        {
            if (healthChecks == null)
            {
                throw new ArgumentNullException(nameof(healthChecks));
            }

            this.UseHealthChecks(healthChecks.Select(healthCheck => new HealthCheckWrapper(healthCheck)).ToArray());

            return this;
        }

        public HealthCheckConfiguration UseHealthChecks(params HealthCheckWrapper[] healthChecks)
        {
            if (healthChecks == null)
            {
                throw new ArgumentNullException(nameof(healthChecks));
            }

            this.HealthChecks = healthChecks.ToArray();

            return this;
        }
    }
}