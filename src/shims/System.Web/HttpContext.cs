// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Microsoft.AspNetCore.Http;

using CoreHttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace System.Web
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Should use Microsoft.AspNetCore.Http.HttpContext instead", DiagnosticId = "SYSWEB0002", UrlFormat = Constants.UrlFormat)]
    public sealed class HttpContext : HttpContextBase
    {
        public HttpContext(CoreHttpContext httpContext)
            : base(httpContext)
        {
        }

        public static HttpContext? Current
        {
            get
            {
                if (Accessor is null)
                {
                    throw new InvalidOperationException("Must call services.AddSystemWebShim() in your ConfigureServices method. HttpContext.Current will not be available until after all services are registered.");
                }

                return Accessor.HttpContext is CoreHttpContext current ? new HttpContext(current) : null;
            }
        }

        internal static IHttpContextAccessor? Accessor { get; set; }
    }
}
