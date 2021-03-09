// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent RenderAction(this IHtmlHelper<dynamic> helper, string name, string action)
        {
            if (helper is null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Raw($"NOT SUPPORTED {name}:{action}");
        }
    }
}
