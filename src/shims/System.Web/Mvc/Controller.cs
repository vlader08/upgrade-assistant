// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CoreController = Microsoft.AspNetCore.Mvc.Controller;

namespace System.Web.Mvc
{
    [Obsolete("System.Web shims should be temporary", DiagnosticId = "SYSWEB00001", UrlFormat = Constants.UrlFormat)]
    public class Controller : CoreController
    {
        public new HttpContextBase HttpContext => base.HttpContext.GetSystemWebContext();

        protected HttpSessionStateBase Session => base.HttpContext.GetSessionStateBase();

        protected HttpServerUtilityBase Server => base.HttpContext.GetServerUtilityBase();

        [Obsolete("TryUpdateModel is now async and code should be adapted to be async", DiagnosticId = "SYSWEB00002", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
        public bool TryUpdateModel<T>(T model)
                where T : class
                => TryUpdateModelAsync(model).GetAwaiter().GetResult();

        public ActionResult HttpNotFound() => NotFound();
    }
}
