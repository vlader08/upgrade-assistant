// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using CoreViewResult = Microsoft.AspNetCore.Mvc.ViewResult;

namespace System.Web.Mvc
{
    [Obsolete("System.Web shims should be temporary", DiagnosticId = "SystemWeb_ViewResult")]
    public class ViewResult : IStatusCodeActionResult, IActionResult
    {
        private readonly CoreViewResult _other;

        public ViewResult(CoreViewResult other)
        {
            _other = other;
        }

        public int? StatusCode => _other.StatusCode;

        public Task ExecuteResultAsync(ActionContext context)
            => _other.ExecuteResultAsync(context);

        public static implicit operator ViewResult(CoreViewResult result) => new ViewResult(result);
    }
}
