// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using CoreRedirectToActionResult = Microsoft.AspNetCore.Mvc.RedirectToActionResult;

namespace System.Web.Mvc
{
    public class RedirectToActionResult : IKeepTempDataResult, IActionResult
    {
        private readonly CoreRedirectToActionResult _other;

        public RedirectToActionResult(CoreRedirectToActionResult other)
        {
            _other = other;
        }

        public Task ExecuteResultAsync(ActionContext context)
            => _other.ExecuteResultAsync(context);

        public static implicit operator RedirectToActionResult(CoreRedirectToActionResult result) => new RedirectToActionResult(result);
    }
}
