// Adapted from https://github.com/aspnet/Diagnostics/blob/release/2.2/src/Microsoft.Extensions.Diagnostics.HealthChecks/DefaultHealthCheckService.cs

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RimDev.AspNet.Diagnostics.HealthChecks
{
    internal class RimDevAspNetHealthCheckService // : HealthCheckService
    {
        private readonly ILogger _logger;

        public RimDevAspNetHealthCheckService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<HealthReport> CheckHealthAsync(
            // Func<HealthCheckRegistration, bool> predicate,
            IEnumerable<HealthCheckWrapper> healthChecks,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = new Dictionary<string, HealthReportEntry>(StringComparer.OrdinalIgnoreCase);

            var totalTime = Stopwatch.StartNew();
            Log.HealthCheckProcessingBegin(_logger);

            foreach (var wrapper in healthChecks)
            {
                /*
                if (predicate != null && !predicate(registration))
                {
                    continue;
                }
                */

                cancellationToken.ThrowIfCancellationRequested();

                // If the health check does things like make Database queries using EF or backend HTTP calls,
                // it may be valuable to know that logs it generates are part of a health check. So we start a scope.
                using (_logger.BeginScope(new HealthCheckLogScope(wrapper.GetType().Name)))
                {
                    var stopwatch = Stopwatch.StartNew();

                    Log.HealthCheckBegin(_logger, wrapper);

                    HealthReportEntry entry;
                    try
                    {
                        var context = new HealthCheckContext
                        {
                            Registration = new HealthCheckRegistration(
                                wrapper.Name,
                                wrapper.HealthCheck,
                                wrapper.FailureStatus,
                                Enumerable.Empty<string>())
                        };

                        var result = await wrapper.HealthCheck.CheckHealthAsync(context, cancellationToken);
                        var duration = stopwatch.Elapsed;

                        entry = new HealthReportEntry(
                            status: result.Status,
                            description: result.Description,
                            duration: duration,
                            exception: result.Exception,
                            data: result.Data);

                        Log.HealthCheckEnd(_logger, wrapper, entry, duration);
                        Log.HealthCheckData(_logger, wrapper, entry);
                    }

                    // Allow cancellation to propagate.
                    catch (Exception ex) when (ex as OperationCanceledException == null)
                    {
                        var duration = stopwatch.Elapsed;
                        entry = new HealthReportEntry(
                            status: HealthStatus.Unhealthy,
                            description: ex.Message,
                            duration: duration,
                            exception: ex,
                            data: null);

                        Log.HealthCheckError(_logger, wrapper, ex, duration);
                    }

                    entries[wrapper.Name] = entry;
                }
            }

            var totalElapsedTime = totalTime.Elapsed;
            var report = new HealthReport(entries, totalElapsedTime);
            Log.HealthCheckProcessingEnd(_logger, report.Status, totalElapsedTime);
            return report;
        }

        // Copied from https://github.com/aspnet/Diagnostics/blob/release/2.2/src/Microsoft.Extensions.Diagnostics.HealthChecks/DefaultHealthCheckService.cs
        internal static class EventIds
        {
            public static readonly EventId HealthCheckProcessingBegin = new EventId(100, "HealthCheckProcessingBegin");
            public static readonly EventId HealthCheckProcessingEnd = new EventId(101, "HealthCheckProcessingEnd");

            public static readonly EventId HealthCheckBegin = new EventId(102, "HealthCheckBegin");
            public static readonly EventId HealthCheckEnd = new EventId(103, "HealthCheckEnd");
            public static readonly EventId HealthCheckError = new EventId(104, "HealthCheckError");
            public static readonly EventId HealthCheckData = new EventId(105, "HealthCheckData");
        }

        // Copied from https://github.com/aspnet/Diagnostics/blob/release/2.2/src/Microsoft.Extensions.Diagnostics.HealthChecks/DefaultHealthCheckService.cs
        // with modifications.
        private static class Log
        {
            private static readonly Action<ILogger, Exception> _healthCheckProcessingBegin = LoggerMessage.Define(
                LogLevel.Debug,
                EventIds.HealthCheckProcessingBegin,
                "Running health checks");

            private static readonly Action<ILogger, double, HealthStatus, Exception> _healthCheckProcessingEnd = LoggerMessage.Define<double, HealthStatus>(
                LogLevel.Debug,
                EventIds.HealthCheckProcessingEnd,
                "Health check processing completed after {ElapsedMilliseconds}ms with combined status {HealthStatus}");

            private static readonly Action<ILogger, string, Exception> _healthCheckBegin = LoggerMessage.Define<string>(
                LogLevel.Debug,
                EventIds.HealthCheckBegin,
                "Running health check {HealthCheckName}");

            // These are separate so they can have different log levels
            private static readonly string HealthCheckEndText = "Health check {HealthCheckName} completed after {ElapsedMilliseconds}ms with status {HealthStatus} and '{HealthCheckDescription}'";

            private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndHealthy = LoggerMessage.Define<string, double, HealthStatus, string>(
                LogLevel.Debug,
                EventIds.HealthCheckEnd,
                HealthCheckEndText);

            private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndDegraded = LoggerMessage.Define<string, double, HealthStatus, string>(
                LogLevel.Warning,
                EventIds.HealthCheckEnd,
                HealthCheckEndText);

            private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndUnhealthy = LoggerMessage.Define<string, double, HealthStatus, string>(
                LogLevel.Error,
                EventIds.HealthCheckEnd,
                HealthCheckEndText);

            private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndFailed = LoggerMessage.Define<string, double, HealthStatus, string>(
                LogLevel.Error,
                EventIds.HealthCheckEnd,
                HealthCheckEndText);

            private static readonly Action<ILogger, string, double, Exception> _healthCheckError = LoggerMessage.Define<string, double>(
                LogLevel.Error,
                EventIds.HealthCheckError,
                "Health check {HealthCheckName} threw an unhandled exception after {ElapsedMilliseconds}ms");

            public static void HealthCheckProcessingBegin(ILogger logger)
            {
                _healthCheckProcessingBegin(logger, null);
            }

            public static void HealthCheckProcessingEnd(ILogger logger, HealthStatus status, TimeSpan duration)
            {
                _healthCheckProcessingEnd(logger, duration.TotalMilliseconds, status, null);
            }

            public static void HealthCheckBegin(ILogger logger, HealthCheckWrapper healthCheck)
            {
                _healthCheckBegin(logger, healthCheck.Name, null);
            }

            public static void HealthCheckEnd(ILogger logger, HealthCheckWrapper healthCheck, HealthReportEntry entry, TimeSpan duration)
            {
                switch (entry.Status)
                {
                    case HealthStatus.Healthy:
                        _healthCheckEndHealthy(logger, healthCheck.Name, duration.TotalMilliseconds, entry.Status, entry.Description, null);
                        break;

                    case HealthStatus.Degraded:
                        _healthCheckEndDegraded(logger, healthCheck.Name, duration.TotalMilliseconds, entry.Status, entry.Description, null);
                        break;

                    case HealthStatus.Unhealthy:
                        _healthCheckEndUnhealthy(logger, healthCheck.Name, duration.TotalMilliseconds, entry.Status, entry.Description, null);
                        break;
                }
            }

            public static void HealthCheckError(ILogger logger, HealthCheckWrapper healthCheck, Exception exception, TimeSpan duration)
            {
                _healthCheckError(logger, healthCheck.Name, duration.TotalMilliseconds, exception);
            }

            public static void HealthCheckData(ILogger logger, HealthCheckWrapper healthCheck, HealthReportEntry entry)
            {
                if (entry.Data.Count > 0 && logger.IsEnabled(LogLevel.Debug))
                {
                    logger.Log(
                        LogLevel.Debug,
                        EventIds.HealthCheckData,
                        new HealthCheckDataLogValue(healthCheck.Name, entry.Data),
                        null,
                        (state, ex) => state.ToString());
                }
            }
        }

        // Copied from https://github.com/aspnet/Diagnostics/blob/release/2.2/src/Microsoft.Extensions.Diagnostics.HealthChecks/DefaultHealthCheckService.cs
        internal class HealthCheckDataLogValue : IReadOnlyList<KeyValuePair<string, object>>
        {
            private readonly string _name;
            private readonly List<KeyValuePair<string, object>> _values;

            private string _formatted;

            public HealthCheckDataLogValue(string name, IReadOnlyDictionary<string, object> values)
            {
                _name = name;
                _values = values.ToList();

                // We add the name as a kvp so that you can filter by health check name in the logs.
                // This is the same parameter name used in the other logs.
                _values.Add(new KeyValuePair<string, object>("HealthCheckName", name));
            }

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new IndexOutOfRangeException(nameof(index));
                    }

                    return _values[index];
                }
            }

            public int Count => _values.Count;

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return _values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _values.GetEnumerator();
            }

            public override string ToString()
            {
                if (_formatted == null)
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"Health check data for {_name}:");

                    var values = _values;
                    for (var i = 0; i < values.Count; i++)
                    {
                        var kvp = values[i];
                        builder.Append("    ");
                        builder.Append(kvp.Key);
                        builder.Append(": ");

                        builder.AppendLine(kvp.Value?.ToString());
                    }

                    _formatted = builder.ToString();
                }

                return _formatted;
            }
        }
    }
}
