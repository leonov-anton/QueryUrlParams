using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using QueryUrlParamsGenerator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers.Base
{
    internal abstract class PropertyHandlerBase : IPropertyHandler
    {
        protected const string queryParamBuilderNamespase = "global::QueryUrlParams.Helpers.QueryParamStringBuilder";

        public abstract bool CanHandle(PropertyInfo prop);

        public virtual StatementSyntax[] GetStatements(PropertyInfo prop)
        {
            var statment = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.ParseName(queryParamBuilderNamespase),
                        SyntaxFactory.IdentifierName("AppendParam")),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[] {
                                SyntaxFactory.Argument(
                                    SyntaxFactory.IdentifierName("sb")),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(prop.Name))),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("obj"),
                                        SyntaxFactory.IdentifierName(prop.OriginalName)))
                            }))));

            return [ statment ];
        }
    }
}
