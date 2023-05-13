using System;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    public static class LegacyHealthCheckConfiguration
    {
        private static HealthCheckConfiguration current = new HealthCheckConfiguration();

        public static HealthCheckConfiguration Current
        {
            get => current;
            set => current = value ?? throw new ArgumentNullException($"The {nameof(LegacyHealthCheckConfiguration)} cannot be null!");
        }
    }
}