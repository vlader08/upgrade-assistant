// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Web.Security
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Membership is not available on .NET Core and later", DiagnosticId = "SYSWEB0002", UrlFormat = Constants.UrlFormat)]
    public static class Membership
    {
        public static string PasswordStrengthRegularExpression { get; } = string.Empty;

        public static int MinRequiredPasswordLength { get; }
    }
}
