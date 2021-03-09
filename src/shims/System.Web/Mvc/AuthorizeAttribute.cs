// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web.Mvc
{
    [Obsolete("System.Web shims should be temporary", DiagnosticId = "SystemWeb_Authorize")]
    public class AuthorizeAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute
    {
    }
}
