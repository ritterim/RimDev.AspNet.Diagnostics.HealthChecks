// Adapted from https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/tree/2.2.0-upgrade-ui-client-2.2.3/src/HealthChecks.UI.Client
// Originally licensed under the Apache License, Version 2.0, https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/2.2.0-upgrade-ui-client-2.2.3/LICENSE

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace RimDev.AspNet.Diagnostics.HealthChecks.UI
{
    public static class UIResponseWriter
    {
        const string DEFAULT_CONTENT_TYPE = "application/json";

        public static Task WriteHealthCheckUIResponse(IOwinContext httpContext, HealthReport report) => WriteHealthCheckUIResponse(httpContext, report, null);
        public static Task WriteHealthCheckUIResponse(IOwinContext httpContext, HealthReport report, string role) => WriteHealthCheckUIResponse(httpContext, report, null, null);

        public static Task WriteHealthCheckUIResponse(IOwinContext httpContext, HealthReport report, Action<JsonSerializerSettings> jsonConfigurator, string role)
        {
            if (!string.IsNullOrEmpty(role) && !httpContext.Authentication.User.IsInRole(role))
            {
                return httpContext.Response.WriteAsync($"You are not authorized for this {role}!");
            }
            var response = "{}";

            if (report != null)
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy
                        {
                            ProcessDictionaryKeys = false
                        }
                    },
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                jsonConfigurator?.Invoke(settings);

                settings.Converters.Add(new StringEnumConverter());

                httpContext.Response.ContentType = DEFAULT_CONTENT_TYPE;

                var uiReport = UIHealthReport
                    .CreateFrom(report);

                response = JsonConvert.SerializeObject(uiReport, settings);
            }

            return httpContext.Response.WriteAsync(response);
        }
    }
}