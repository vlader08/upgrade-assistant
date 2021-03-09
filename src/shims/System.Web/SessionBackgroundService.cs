// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace System.Web
{
    internal class SessionBackgroundService : BackgroundService
    {
        private readonly ISessionManager _manager;
        private readonly IOptions<SessionShimOptions> _options;
        private readonly ILogger<SessionBackgroundService> _logger;

        public SessionBackgroundService(
            ISessionManager manager,
            IOptions<SessionShimOptions> options,
            ILogger<SessionBackgroundService> logger)
        {
            _manager = manager;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = _options.Value;
            var stopwatch = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(options.CleanupFrequency, stoppingToken).ConfigureAwait(false);
                stoppingToken.ThrowIfCancellationRequested();

                _logger.LogTrace("System.Web session emulation is cleaning up stale sessions");
                stopwatch.Restart();

                try
                {
                    var count = _manager.RemoveStaleSessions(options.SessionAge);
                    stopwatch.Stop();
                    _logger.LogInformation("System.Web session emulation cleaned up {Count} stale sessions in {Time}ms", count, stopwatch.ElapsedMilliseconds);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    stopwatch.Stop();
                    _logger.LogError(e, "System.Web session emulation clean up failed");
                }
            }
        }
    }
}
