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

## Requirements

- .NET Framework 4.6.2 or later

## License

MIT
