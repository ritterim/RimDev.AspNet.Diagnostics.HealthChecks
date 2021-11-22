using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    public class HealthCheckWrapper
    {
        public HealthCheckWrapper(IHealthCheck healthCheck, string name = null, HealthStatus? failureStatus = null)
        {
            HealthCheck = healthCheck;
            
            if (string.IsNullOrWhiteSpace(name))
                name = healthCheck.GetType().Name;

            Name = name;
            FailureStatus = failureStatus;
        }
        public IHealthCheck HealthCheck { get; }
        public string Name { get; }
        public HealthStatus? FailureStatus { get; }
    }
}
