// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

using CoreHttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace System.Web
{
    internal class SystemWebCookieSessionIdFactory : ISystemWebSessionIdFactory
    {
        private readonly ILogger<SystemWebCookieSessionIdFactory> _logger;

        public SystemWebCookieSessionIdFactory(ILogger<SystemWebCookieSessionIdFactory> logger)
        {
            _logger = logger;
        }

        public string GetSessionId(CoreHttpContext context)
        {
            const string SessionId = "SystemWebSessionStateId";

            if (context.Request.Cookies.TryGetValue(SessionId, out var result) && result is not null)
            {
                _logger.LogTrace("Found session {Id}", result);
                return result;
            }

            var sessionId = Guid.NewGuid().ToString();
            _logger.LogTrace("Created new session {Id}", sessionId);

            context.Response.Cookies.Append(SessionId, sessionId);
            return sessionId;
        }
    }
}
