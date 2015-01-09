﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace CodeCracker.Style
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyPrivateSetAnalyzer
        : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CC0047 ";
        internal const string Title = "You should change to 'private set' whenever possible.";
        internal const string MessageFormat = "Consider use a 'private set'.";
        internal const string Category = "Style";
        const string Description = "Use private set for automatic properties.";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: Description,
            helpLink: HelpLink.ForDiagnostic(DiagnosticId));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.PropertyDeclaration);
        }
       
        private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (PropertyDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            if (invocationExpression.AccessorList == null || invocationExpression.AccessorList.Accessors.Count == 1) return;

            var setAcessor = (invocationExpression.AccessorList.Accessors[0].Keyword.Text == "set") ? invocationExpression.AccessorList.Accessors[0] : invocationExpression.AccessorList.Accessors[1];

            if (setAcessor.Modifiers.Count != 0) return;

            var error = string.Format(MessageFormat, MessageFormat);

            var diag = Diagnostic.Create(Rule, invocationExpression.GetLocation(), error);
            context.ReportDiagnostic(diag);
        }

    }
}
