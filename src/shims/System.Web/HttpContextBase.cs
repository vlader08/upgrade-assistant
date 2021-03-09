// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Principal;

using CoreHttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace System.Web
{
    [Obsolete("System.Web shims should be temporary", DiagnosticId = "SystemWeb_HttpContextBase")]
    public abstract class HttpContextBase
    {
        private readonly CoreHttpContext _httpContext;

        private protected HttpContextBase(CoreHttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public CoreHttpContext AsAspNetCoreHttpContext() => _httpContext;

        public HttpSessionStateBase Session => _httpContext.GetSessionStateBase();

        public IPrincipal User => _httpContext.User;

#pragma warning disable CA1062 // Validate arguments of public methods
        public static implicit operator CoreHttpContext(HttpContextBase context) => context._httpContext;

        public static implicit operator HttpContextBase(CoreHttpContext context) => new HttpContext(context);
#pragma warning restore CA1062 // Validate arguments of public methods
    }
}
