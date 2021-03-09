// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;

namespace System.Web
{
    public class HttpServerUtilityBase
    {
#pragma warning disable CA1822 // Mark members as static
        public string HtmlEncode(string str) => HttpUtility.UrlEncode(str, Encoding.UTF8);
#pragma warning restore CA1822 // Mark members as static
    }
}
