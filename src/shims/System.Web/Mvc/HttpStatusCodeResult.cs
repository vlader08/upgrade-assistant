// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace System.Web.Mvc
{
    public class HttpStatusCodeResult : StatusCodeResult
    {
        public HttpStatusCodeResult(HttpStatusCode code)
            : base((int)code)
        {
        }
    }
}
