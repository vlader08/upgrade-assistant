// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web
{
    public class SessionShimOptions
    {
        public TimeSpan SessionAge { get; set; } = TimeSpan.FromMinutes(20);

        public TimeSpan CleanupFrequency { get; set; } = TimeSpan.FromMinutes(1);
    }
}
