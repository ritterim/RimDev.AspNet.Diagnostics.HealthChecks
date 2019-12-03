# RimDev.AspNet.Diagnostics.HealthChecks

ASP.NET full framework implementation of ASP.NET Core health checks.

## Usage

The recommended installation method is the [RimDev.AspNet.Diagnostics.HealthChecks](https://www.nuget.org/packages/RimDev.AspNet.Diagnostics.HealthChecks) NuGet package.

```
PM> Install-Package RimDev.AspNet.Diagnostics.HealthChecks
```

Then, use it like this in an OWIN Startup class:

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseHealthChecks(
            "/_health",
            new NoopHealthCheck(),
            new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)));
    }
}
```

### Usage with Health Checks UI

To use with the [Health Checks UI project](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#healthcheckui-and-failure-notifications)
named health checks should be used. and a special ResponseWriter needs to be confiugred. This returns the checks with more specific information
about each check in a format that the UI project can read.

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseHealthChecks(
            "/_health_ui",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            },
            new HealthCheckWrapper(new NoopHealthCheck(), "Noop health check"),
            new HealthCheckWrapper(new PingHealthCheck(new PingHealthCheckOptions().AddHost("localhost", 1000)), "Ping to localhost"));
    }
}
```

The UI can't be hosted in a full framework app but can easily be setup using the official [docker image](https://hub.docker.com/r/xabarilcoding/healthchecksui/)
for those who doesn't already have a UI project set up.

## Requirements

- .NET Framework 4.6.2 or later

## License

MIT
