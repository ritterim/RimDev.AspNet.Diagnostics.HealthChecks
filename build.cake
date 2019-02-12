var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./RimDev.AspNet.Diagnostics.HealthChecks.sln";
var publishDirectory = Directory("./artifacts");

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(publishDirectory);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore(solution);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        MSBuild(solution, settings =>
            settings.SetConfiguration(configuration)
                .WithProperty("TreatWarningsAsErrors", "False")
                .SetVerbosity(Verbosity.Minimal)
                .AddFileLogger());
    });

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
    {
        NuGetPack("./src/RimDev.AspNet.Diagnostics.HealthChecks/RimDev.AspNet.Diagnostics.HealthChecks.csproj", new NuGetPackSettings
        {
            OutputDirectory = publishDirectory,
            Properties = new Dictionary<string, string>
            {
                { "Configuration", configuration }
            }
        });
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);
