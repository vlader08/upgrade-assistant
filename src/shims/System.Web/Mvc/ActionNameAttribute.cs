// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc.Routing;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ActionNameAttribute : Attribute, IRouteTemplateProvider
    {
        public ActionNameAttribute(string template)
        {
            Template = template;
        }

        public string Template { get; }

        public int? Order { get; set; } = int.MinValue;

        public string Name { get; set; } = string.Empty;
    }
}
