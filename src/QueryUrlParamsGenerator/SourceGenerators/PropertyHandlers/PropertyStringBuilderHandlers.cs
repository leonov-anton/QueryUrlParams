using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using QueryUrlParamsGenerator.Models;
using QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers.Base;
using System;
using System.Linq;

namespace QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers
{
    internal class InnerUrlParametersDtoPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        private const string queryParameterClassAttributeName = "GenerateQueryUrlAttribute";

        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.GetAttributes().Any(a => a.AttributeClass?.Name == queryParameterClassAttributeName);

        public override StatementSyntax[] GetStatements(PropertyInfo prop)
        {
            var classSymbol = (INamedTypeSymbol)prop.Type;

            var namespaceSymbol = classSymbol.ContainingNamespace;
            var namespaceName = namespaceSymbol?.IsGlobalNamespace == true
                ? string.Empty
                : namespaceSymbol?.ToString() ?? "Generated";

            return
            [
                SyntaxFactory.IfStatement(
                    SyntaxFactory.BinaryExpression(
                        SyntaxKind.GreaterThanExpression,
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("sb"),
                            SyntaxFactory.IdentifierName("Length")
                        ),
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(0)
                        )),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("sb"),
                                SyntaxFactory.IdentifierName("Append")
                            ),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal("&")
                                    ))))))
                    ),

                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("sb"),
                            SyntaxFactory.IdentifierName("Append")
                        ),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.ParseName($"global::{namespaceName}.{classSymbol.Name}Extensions"),
                                        SyntaxFactory.IdentifierName("GetObjectUrlParams")
                                    ),
                                    SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("obj"),
                                                SyntaxFactory.IdentifierName(prop.OriginalName)
                                            )))))))))
                )
            ];
        }
    }

    internal class DictionaryPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.MetadataName == "Dictionary`2";

        public override StatementSyntax[] GetStatements(PropertyInfo prop) =>
            [
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ParseName(queryParamBuilderNamespase),
                            SyntaxFactory.IdentifierName("AppendParams")),
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
                                }))))
            ];
    }

    internal class EnumerablePropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.AllInterfaces.Any(i =>
                i.OriginalDefinition.SpecialType == SpecialType.System_Collections_IEnumerable
                && prop.Type.SpecialType != SpecialType.System_String);

        public override StatementSyntax[] GetStatements(PropertyInfo prop) =>
            [
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ParseName(queryParamBuilderNamespase),
                            SyntaxFactory.IdentifierName("AppendParams")),
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
                                }))))
            ];
    }

    internal class StringPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_String;
    }

    internal class DoublePropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Double;
    }

    internal class IntPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Int32;
    }

    internal class DateTimePropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_DateTime;

        public override StatementSyntax[] GetStatements(PropertyInfo prop)
        {
            string defaultFormat = "yyyy-MM-ddTHH:mm:ssZ";
            var dateTimeAttr = prop.AttributeInfos.FirstOrDefault(a => a.TypeName == "DateTimeFormatAttribute");
            var format = dateTimeAttr == null
                        ? defaultFormat
                        : dateTimeAttr.NamedArgumentInfo
                              .OfType<StringArgumentInfo>()
                              .Concat(dateTimeAttr.ConstructorArgumentInfo.OfType<StringArgumentInfo>())
                              .Where(a => string.Equals(a.Name, "Format", StringComparison.OrdinalIgnoreCase))
                              .Select(a => a.Value)
                              .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? defaultFormat;
            
            return
            [
                //SyntaxFactory.ParseStatement($"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}?.ToString(\"{format}\"));"),

                SyntaxFactory.ExpressionStatement(
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
                                        SyntaxFactory.ConditionalAccessExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("obj"),
                                                SyntaxFactory.IdentifierName(prop.OriginalName)),
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberBindingExpression(
                                                    SyntaxFactory.IdentifierName("ToString")),
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                SyntaxFactory.Literal(format))))))))
                                }))))
            ];
        }
    }

    internal class BooleanPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Boolean;
    }

    internal class DecimalPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Decimal;
    }

    internal class EnumPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.TypeKind == TypeKind.Enum;

        public override StatementSyntax[] GetStatements(PropertyInfo prop)
        {
            bool isString = prop.AttributeInfos.Any(a => a.TypeName == "EnumAsStringAttribute");
            return
            [
                SyntaxFactory.ParseStatement($"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}, {isString.ToString().ToLowerInvariant()});"),

                SyntaxFactory.ExpressionStatement(
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
                                        SyntaxFactory.LiteralExpression(
                                            isString ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression))
                                }))))
            ];
        }
    }

    internal class DefaultPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) => true;
        
        public override StatementSyntax[] GetStatements(PropertyInfo prop) =>
            [
                SyntaxFactory.ExpressionStatement(
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
                                        SyntaxFactory.ConditionalAccessExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("obj"),
                                                SyntaxFactory.IdentifierName(prop.OriginalName)),
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberBindingExpression(
                                                    SyntaxFactory.IdentifierName("ToString")))))
                                }))))
            ];
    }
}
