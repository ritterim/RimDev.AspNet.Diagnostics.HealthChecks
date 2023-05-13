// Copied from https://github.com/aspnet/AspNetCore/blob/9f202feafc335dd70ae640f2053b465be3879e82/src/Middleware/HealthChecks/src/HealthCheckResponseWriters.cs
// with modifications.

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Web;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    internal static class HealthCheckResponseWriters
    {
        public static void WriteMinimalPlaintext(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "text/plain";
            httpContext.Response.Write(result.Status.ToString());
        }
    }
}