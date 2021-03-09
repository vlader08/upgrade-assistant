// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CoreActionResult = Microsoft.AspNetCore.Mvc.ActionResult;

namespace System.Web.Mvc
{
    [Obsolete("System.Web shims should be temporary", DiagnosticId = "SystemWeb_ActionResult")]
    public class ActionResult : IActionResult
    {
        private readonly CoreActionResult _other;

        public ActionResult(CoreActionResult other)
        {
            _other = other;
        }

        public Task ExecuteResultAsync(ActionContext context)
            => _other.ExecuteResultAsync(context);

        public static implicit operator ActionResult(CoreActionResult result) => new ActionResult(result);
        }
}
