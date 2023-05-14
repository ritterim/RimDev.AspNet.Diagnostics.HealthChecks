using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RimDev.AspNet.Diagnostics.HealthChecks.Configuration
{
    public class NamedHealthCheck
    {
        public NamedHealthCheck(IHealthCheck healthCheck)
            : this(healthCheck, name: null)
        {
        }

        public NamedHealthCheck(IHealthCheck healthCheck, string? name)
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
