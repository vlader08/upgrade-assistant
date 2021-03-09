// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using CoreHttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace System.Web
{
    internal static class FeatureRetriever
    {
        public static HttpServerUtilityBase GetServerUtilityBase(this CoreHttpContext context)
            => context.GetFeature(static _ => new HttpServerUtilityBase());

        public static HttpContext GetSystemWebContext(this CoreHttpContext context)
            => context.GetFeature(static ctx => new HttpContext(ctx));

        public static HttpSessionStateBase GetSessionStateBase(this CoreHttpContext context)
            => context.GetFeature(static ctx =>
            {
                var sessionId = ctx.RequestServices.GetRequiredService<ISystemWebSessionIdFactory>().GetSessionId(ctx);
                return ctx.RequestServices.GetRequiredService<ISessionManager>().GetState(sessionId);
            });

        private static T GetFeature<T>(this CoreHttpContext context, Func<CoreHttpContext, T> factory)
            where T : class
        {
            var feature = context.Features.Get<T>();

            if (feature is not null)
            {
                return feature;
            }

            feature = factory(context);
            context.Features.Set(feature);
            return feature;
        }
    }
}
