﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.DotNet.UpgradeAssistant.Extensions.Default.CSharp.Analyzers
{
    [ApplicableComponents(ProjectComponents.Web)]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ResultTypeAnalyzer : IdentifierUpgradeAnalyzer
    {
        public const string DiagnosticId = "UA0003";
        private const string Category = "Upgrade";

        public override IEnumerable<IdentifierMapping> IdentifierMappings { get; } = new[]
        {
            new IdentifierMapping("System.Web.Mvc.ActionResult", "Microsoft.AspNetCore.Mvc.ActionResult"),
            new IdentifierMapping("System.Web.Mvc.ContentResult", "Microsoft.AspNetCore.Mvc.ContentResult"),
            new IdentifierMapping("System.Web.Mvc.FileResult", "Microsoft.AspNetCore.Mvc.FileResult"),
            new IdentifierMapping("System.Web.Mvc.HttpNotFoundResult", "Microsoft.AspNetCore.Mvc.NotFoundResult"),
            new IdentifierMapping("System.Web.Mvc.HttpStatusCodeResult", "Microsoft.AspNetCore.Mvc.StatusCodeResult"),
            new IdentifierMapping("System.Web.Mvc.HttpUnauthorizedResult", "Microsoft.AspNetCore.Mvc.UnauthorizedResult"),
            new IdentifierMapping("System.Web.Mvc.RedirectResult", "Microsoft.AspNetCore.Mvc.RedirectResult"),
            new IdentifierMapping("System.Web.Mvc.PartialViewResult", "Microsoft.AspNetCore.Mvc.PartialViewResult"),
            new IdentifierMapping("System.Web.Mvc.ViewResult", "Microsoft.AspNetCore.Mvc.ViewResult")
        };

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ResultTypeTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ResultTypeMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ResultTypeDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        protected override Diagnostic CreateDiagnostic(Location location, ImmutableDictionary<string, string?> properties, params object[] messageArgs) => Diagnostic.Create(Rule, location, properties, messageArgs);
    }
}
