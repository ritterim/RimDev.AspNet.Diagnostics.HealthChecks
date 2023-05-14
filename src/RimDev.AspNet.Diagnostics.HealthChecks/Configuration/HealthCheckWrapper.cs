using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RimDev.AspNet.Diagnostics.HealthChecks.Configuration
{
    public class HealthCheckWrapper
    {
        public HealthCheckWrapper(IHealthCheck healthCheck)
            : this(healthCheck, name: null)
        {
        }

        public HealthCheckWrapper(IHealthCheck healthCheck, string? name)
        {
            HealthCheck = healthCheck;

            Name = name is null || string.IsNullOrWhiteSpace(name)
                ? healthCheck.GetType().Name
                : name;
        }

        public IHealthCheck HealthCheck { get; }

        public string Name { get; }
    }
}
