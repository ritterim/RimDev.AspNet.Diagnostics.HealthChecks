// Adapted from https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/tree/2.2.0-upgrade-ui-client-2.2.3/src/HealthChecks.UI.Client
// Originally licensed under the Apache License, Version 2.0, https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/2.2.0-upgrade-ui-client-2.2.3/LICENSE

using System;
using System.Web;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace RimDev.AspNet.Diagnostics.HealthChecks.UI
{
    public static class UIResponseWriter
    {
        const string DEFAULT_CONTENT_TYPE = "application/json";

        public static void WriteHealthCheckUIResponse(HttpContext httpContext, HealthReport report) => WriteHealthCheckUIResponse(httpContext, report, null);

        public static void WriteHealthCheckUIResponse(HttpContext httpContext, HealthReport report, Action<JsonSerializerSettings>? jsonConfigurator)
        {
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

            httpContext.Response.Write(response);
        }
    }
}