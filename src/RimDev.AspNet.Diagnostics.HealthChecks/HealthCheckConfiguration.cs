using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    internal class HealthCheckConfiguration
    {
        public HealthCheckOptions Options { get; set; } = new HealthCheckOptions();

        public IEnumerable<HealthCheckWrapper> HealthChecks { get; set; } = Enumerable.Empty<HealthCheckWrapper>();

        public Func<IEnumerable<HealthCheckWrapper>> CollectHealthChecks { get; set; } = () => Enumerable.Empty<HealthCheckWrapper>();
    }
}