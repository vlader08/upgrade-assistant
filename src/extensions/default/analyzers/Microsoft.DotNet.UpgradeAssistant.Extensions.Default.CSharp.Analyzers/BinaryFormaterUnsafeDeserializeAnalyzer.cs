﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.DotNet.UpgradeAssistant.Extensions.Default.CSharp.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class BinaryFormaterUnsafeDeserializeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UA0012";
        private const string Category = "Upgrade";

        private const string TargetTypeSimpleName = "formatter";
        private const string TargetTypeSymbolName = "System.Runtime.Serialization.Formatters.Binary";
        private const string TargetMember = "UnsafeDeserialize";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.BinaryFormatterUnsafeDeserializeTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.FilterMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.HttpContextCurrentDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);

            context.RegisterSyntaxNodeAction(AnalyzeMemberAccessExpressions, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeMemberAccessExpressions(SyntaxNodeAnalysisContext context)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node;

            // If the accessed member isn't named "UnsafeDeserialize" bail out
            if (!TargetMember.Equals(memberAccessExpression.Name.ToString(), StringComparison.Ordinal))
            {
                return;
            }

            // Continue, only if the call is to a method called UnsafeDeserialize
            if (!(memberAccessExpression.Parent is InvocationExpressionSyntax))
            {
                return;
            }

            // Get the identifier accessed
            var accessedIdentifier = memberAccessExpression.Expression switch
            {
                IdentifierNameSyntax i => i,
                MemberAccessExpressionSyntax m => m.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault(),
                _ => null
            };

            // Return if the accessed identifier wasn't from a simple member access expression or identifier, or if it doesn't match HttpContext
            if (accessedIdentifier is null || !TargetTypeSimpleName.Equals(accessedIdentifier.Identifier.ValueText, StringComparison.Ordinal))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
