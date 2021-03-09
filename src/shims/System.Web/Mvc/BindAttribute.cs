// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class BindAttribute : Microsoft.AspNetCore.Mvc.BindAttribute
    {
        public BindAttribute(params string[] include)
            : base(include)
        {
        }

        [Obsolete("Not supported right now")]
        public string Exclude { get; set; } = string.Empty;
    }
}
