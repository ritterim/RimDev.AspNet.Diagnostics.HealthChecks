using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RimDev.AspNet.Diagnostics.HealthChecks.UI;

namespace RimDev.AspNet.Diagnostics.HealthChecks.Configuration
{
    public class LegacyHealthCheckRouteConfiguration
    {
        public LegacyHealthCheckRouteConfiguration()
        {
            this.Options = new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            };

            this.CollectedHealthChecks = new List<Func<IEnumerable<NamedHealthCheck>>>();
        }

        public HealthCheckOptions Options { get; private set; }

        internal List<Func<IEnumerable<NamedHealthCheck>>> CollectedHealthChecks { get; set; }

        public LegacyHealthCheckRouteConfiguration WithOptions(HealthCheckOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;

            return this;
        }

        public LegacyHealthCheckRouteConfiguration WithOptions(Action<HealthCheckOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure(this.Options);

            return this;
        }

        public LegacyHealthCheckRouteConfiguration AddCheck<T>(string? name = null)
            where T : IHealthCheck, new()
        {
            return this.AddCheck(new T(), name);
        }

        public LegacyHealthCheckRouteConfiguration AddCheck<T>(T healthCheck, string? name = null)
            where T : IHealthCheck
        {
            if (healthCheck == null)
            {
                throw new ArgumentNullException(nameof(healthCheck));
            }

            return this.AddNamedCheck(new NamedHealthCheck(name, healthCheck));
        }

        public LegacyHealthCheckRouteConfiguration AddChecks(Func<IEnumerable<IHealthCheck>> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var map = () => factory()
                .Select(healthCheck => new NamedHealthCheck(healthCheck))
                .ToArray();

            return this.AddNamedChecks(map);
        }

        public LegacyHealthCheckRouteConfiguration AddNamedChecks(Func<IEnumerable<NamedHealthCheck>> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.CollectedHealthChecks.Add(factory);

            return this;
        }

        public LegacyHealthCheckRouteConfiguration AddChecks(params IHealthCheck[] healthChecks)
        {
            if (healthChecks == null)
            {
                throw new ArgumentNullException(nameof(healthChecks));
            }

            foreach (IHealthCheck healthCheck in healthChecks)
            {
                this.AddCheck(healthCheck);
            }

            return this;
        }

        public LegacyHealthCheckRouteConfiguration AddNamedChecks(params NamedHealthCheck[] healthChecks)
        {
            if (healthChecks == null)
            {
                throw new ArgumentNullException(nameof(healthChecks));
            }

            foreach (NamedHealthCheck healthCheck in healthChecks)
            {
                this.AddNamedCheck(healthCheck);
            }

            return this;
        }

        private LegacyHealthCheckRouteConfiguration AddNamedCheck(NamedHealthCheck healthCheck)
        {
            if (healthCheck == null)
            {
                throw new ArgumentNullException(nameof(healthCheck));
            }

            return this.AddCheck(() => healthCheck);
        }

        private LegacyHealthCheckRouteConfiguration AddCheck(Func<NamedHealthCheck> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.CollectedHealthChecks.Add(() => new[] { factory() });

            return this;
        }
    }
}
