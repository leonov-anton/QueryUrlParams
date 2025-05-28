using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using QueryUrlParamsGenerator.Extensions;
using QueryUrlParamsGenerator.Models;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace QueryUrlParamsGenerator.SourceGenerators
{
    [Generator(LanguageNames.CSharp)]
    public sealed class QueryUrlGenerator : IIncrementalGenerator
    {
        private const string fullyQualifiedMetadataName = "QueryUrlParams.Attributes.GenerateQueryUrlAttribute";
        private const string queryParameterClassAttributeName = "GenerateQueryUrlAttribute";
        private const string queryParameterNameAttributeName = "QueryParameterNameAttribute";
        private const string queryParameterIgnoreAttributeName = "QueryParameterIgnoreAttribute";
        private const string queryParameterDateTimeFormatAttributeName = "DateTimeFormatAttribute";
        private const string queryParameterEnumAsStringAttributeName = "EnumAsStringAttribute";

        private static readonly PropertyHandlerResolver _propHandlers;

        static QueryUrlGenerator()
        {
            _propHandlers = new PropertyHandlerResolver();
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //if (!Debugger.IsAttached)
            //    Debugger.Launch();

            IncrementalValuesProvider<Result<ClassInfo>> allClassInfo = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    fullyQualifiedMetadataName: fullyQualifiedMetadataName,
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (syntax, _) => GetClassInfo(syntax))
                .Where(static target => target is not null);

            // Register a source output for all classes with errors
            // and report the diagnostics
            var errors = allClassInfo
                .Where(static target => target.Diagnostics.Any())
                .SelectMany(static (item, _) => item.Diagnostics);

            context.RegisterSourceOutput(errors, static (context, diagnostic) =>
            {
                context.ReportDiagnostic(diagnostic);
            });

            // Select only those classes that have no errors and are not null
            // and cache the result
            var withoutErrors = allClassInfo
                .Where(static target => !target.Diagnostics.Any(d => d.IsWarningAsError) && target.Value != null)
                .Select(static (item, _) => item.Value!);

            // Register a source output for all classes without errors
            // and generate the source code
            context.RegisterSourceOutput(withoutErrors, static (context, classInfo) =>
            {
                var compilationUnit = GetCompilationUnit(classInfo);

                context.AddSource($"{classInfo.Name}Extensions.g.cs", compilationUnit.ToFullString());
            });
        }

        /// <summary>
        /// Generates the source code for the extentions class.
        /// </summary>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        private static CompilationUnitSyntax GetCompilationUnit(ClassInfo classInfo)
        {
            // Create the base URL field
            var baseUrlField = FieldDeclaration(
                VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
                .WithVariables(SingletonSeparatedList(
                    VariableDeclarator(Identifier("_defaultBaseUrl"))
                    .WithInitializer(EqualsValueClause(
                        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(classInfo.BaseUrl)))))))
                .AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ConstKeyword))
                .WithLeadingTrivia(TriviaList(
                    Comment($"/// <summary>Base uri from GenerateQueryUrlAttribute.</summary>")
                    ));

            // Create ToQueryUrl method
            var urlMethod = MethodDeclaration(
                PredefinedType(Token(SyntaxKind.StringKeyword)), "ToQueryUrl")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .WithReturnType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                .AddParameterListParameters(
                    Parameter(Identifier("obj"))
                    .WithType(IdentifierName(classInfo.Name))
                    .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))))
                .WithExpressionBody(ArrowExpressionClause(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("obj"),
                            IdentifierName("ToQueryUrl")),
                        ArgumentList(
                            SingletonSeparatedList(Argument(IdentifierName("_defaultBaseUrl")))))))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .WithLeadingTrivia(TriviaList(
                    Comment("/// <summary>"),
                    Comment($"/// Converts the <see cref=\"{classInfo.Name}\"/> object to a query URL using the default base URL."),
                    Comment("/// <summary>"),
                    Comment($"/// <param name=\"obj\">The source object containing query parameters.</param>"),
                    Comment($"/// <returns>A query URL string.</returns>")
                    ))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            // Create ToQueryUrl method with base URL
            var withUrlMethod = MethodDeclaration(
                PredefinedType(Token(SyntaxKind.StringKeyword)), "ToQueryUrl")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .WithReturnType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                .AddParameterListParameters(
                    Parameter(Identifier("obj"))
                    .WithType(IdentifierName(classInfo.Name))
                    .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))),
                    Parameter(Identifier("baseUrl"))
                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))))
                .WithBody(GetToQueryUrlWithBaseMethodSyntax())
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .WithLeadingTrivia(TriviaList(
                    Comment("/// <summary>"),
                    Comment($"/// Converts the <see cref=\"{classInfo.Name}\"/> object to a query URL using the specified base URL."),
                    Comment("/// <summary>"),
                    Comment($"/// <param name=\"obj\">The source object containing query parameters.</param>"),
                    Comment($"/// <param name=\"baseUrl\">The base URL to use.</param>"),
                    Comment($"/// <returns>A query URL string.</returns>")
                    ));

            // Create the GetObjectUrlParams method
            var objectUrlMethod = MethodDeclaration(
                PredefinedType(Token(SyntaxKind.StringKeyword)), "GetObjectUrlParams")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .WithReturnType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                .AddParameterListParameters(
                    Parameter(Identifier("obj")).WithType(IdentifierName(classInfo.Name)))
                .WithBody(GetObjectUrlParamsMethodSyntax(classInfo))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .WithLeadingTrivia(TriviaList(
                    Comment("/// <summary>"),
                    Comment($"/// Extracts the list of query parameters from the <see cref=\"{classInfo.Name}\"/> object."),
                    Comment("/// <summary>"),
                    Comment($"/// <param name=\"obj\">The source object containing parameter values.</param>"),
                    Comment($"/// <returns>A string of query parameter with & separator.</returns>")
                    ));

            // Create the class declaration
            var classDecl = ClassDeclaration(classInfo.Name + "Extensions")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .AddMembers(baseUrlField, urlMethod, withUrlMethod, objectUrlMethod)
                .WithLeadingTrivia(TriviaList(
                    Comment("/// <summary>"),
                    Comment($"/// Extension methods for generating query URLs from <see cref=\"{classInfo.Name}\"/>."),
                    Comment("/// <summary>")
                    ));

            // Create the namespace declaration
            var namespaceDecl = NamespaceDeclaration(ParseName(classInfo.Namespace));

            var compilationUnit = CompilationUnit()
                .AddMembers(
                    namespaceDecl.AddMembers(classDecl))
                .WithLeadingTrivia(
                        TriviaList(
                            Comment("// <auto-generated/>"),
                            Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)),
                            Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
               .NormalizeWhitespace();

            return compilationUnit;
        }

        /// <summary>
        /// Generates the body of the ToQueryUrl method with a base URL.
        /// </summary>
        /// <returns></returns>
        private static BlockSyntax GetToQueryUrlWithBaseMethodSyntax()
        {
            var block = Block();

            // var parameters = GetObjectUrlParams(obj);
            block = block.AddStatements(
                LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier("parameters"))
                            .WithInitializer(
                                EqualsValueClause(
                                    InvocationExpression(IdentifierName("GetObjectUrlParams"))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList(
                                                Argument(IdentifierName("obj")))
                                            ))))))
                    )
                );
                            

            // if (string.IsNullOrEmpty(parameters)) return baseUrl;
            block = block.AddStatements(
                IfStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("string"),
                            IdentifierName("IsNullOrEmpty")),
                            ArgumentList(
                                SingletonSeparatedList(Argument(IdentifierName("parameters"))
                            ))
                        ),
                    ReturnStatement(IdentifierName("baseUrl"))
                    )
                );

            // return string.Concat(baseUrl, "?", parameters);
            block = block.AddStatements(
                ReturnStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("string"),
                            IdentifierName("Concat")),
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[] {
                                    Argument(IdentifierName("baseUrl")),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("?"))),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(IdentifierName("parameters"))
                                })))
                    )
                );

            return block;
        }

        /// <summary>
        /// Generates the body of the GetObjectUrlParams method.
        /// </summary>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        private static BlockSyntax GetObjectUrlParamsMethodSyntax(ClassInfo classInfo)
        {
            BlockSyntax block = Block();

            // if (obj == null) return string.Empty;
            block = block.AddStatements(
                IfStatement(
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        IdentifierName("obj"),
                        LiteralExpression(SyntaxKind.NullLiteralExpression)),
                    ReturnStatement(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("string"),
                            IdentifierName("Empty")))
                    )
                );

            // var sb = new global::System.Text.StringBuilder(256);
            block = block.AddStatements(
                LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                Identifier("sb"))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(
                                        IdentifierName("global::System.Text.StringBuilder"))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList(
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(256))))))))))
                    )
                );

            foreach (var prop in classInfo.Properties)
            {
                var statement = _propHandlers.GetStatement(prop);
                block = block.AddStatements(statement);
            }

            // return sb.ToString();
            block = block.AddStatements(
                ReturnStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("sb"),
                            IdentifierName("ToString")))
                    )
                );

            return block;
        }

        /// <summary>
        /// Gets the class information from the context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Result<ClassInfo> GetClassInfo(GeneratorAttributeSyntaxContext context)
        {
            var classSymbol = (INamedTypeSymbol)context.TargetSymbol;

            var namespaceSymbol = classSymbol.ContainingNamespace;
            var namespaceName = namespaceSymbol?.IsGlobalNamespace == true
                ? string.Empty
                : namespaceSymbol?.ToString() ?? "Generated";

            string baseUrl = string.Empty;
            bool toSnakeCase = true;

            var attribute = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == queryParameterClassAttributeName);
            if (attribute != null)
            {
                baseUrl = attribute.GetConstructorStringArgument("BaseUrl") ?? 
                            attribute.GetNamedArgument<string>("BaseUrl") ?? 
                            string.Empty;

                toSnakeCase = attribute.GetConstructorBoolArgument("SnakeCaseNameConvert") ??
                              attribute.GetNamedVauetTypeArgument<bool>("SnakeCaseNameConvert") ??
                              true;
            }

            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            var properties = classSymbol.GetMembers()
                                        .OfType<IPropertySymbol>()
                                        .Where(p => !p.IsReadOnly && !IsIgnoredProperty(p))
                                        .Select(p =>
                                        {
                                            var attrInfos = ImmutableArray.CreateBuilder<AttributeInfo>();

                                            foreach (var attr in p.GetAttributes())
                                            {
                                                if (attr.AttributeClass?.Name == queryParameterDateTimeFormatAttributeName &&
                                                    GetNullableSymbolType(p.Type).SpecialType != SpecialType.System_DateTime)
                                                {
                                                    var diagnostic = Diagnostic.Create(
                                                        new DiagnosticDescriptor(
                                                            "QUPG010",
                                                            "DateTime format attribute is not supported",
                                                            "DateTime format attribute is not supported for property '{0}'",
                                                            "Usage",
                                                            DiagnosticSeverity.Warning,
                                                            true),
                                                    p.Locations[0],
                                                    p.Name);

                                                    diagnostics.Add(diagnostic);

                                                    continue;
                                                }

                                                if (attr.AttributeClass?.Name == queryParameterEnumAsStringAttributeName &&
                                                    GetNullableSymbolType(p.Type).TypeKind != TypeKind.Enum)
                                                {
                                                    var diagnostic = Diagnostic.Create(
                                                        new DiagnosticDescriptor(
                                                            "QUPG011",
                                                            "Enum as string attribute is not supported",
                                                            "Enum as string attribute is not supported for property '{0}'",
                                                            "Usage",
                                                            DiagnosticSeverity.Warning,
                                                            true),
                                                    p.Locations[0],
                                                    p.Name);

                                                    diagnostics.Add(diagnostic);

                                                    continue;
                                                }

                                                // Create an AttributeInfo for each attribute
                                                var attrInfo = new AttributeInfo(
                                                attr.AttributeClass?.Name ?? string.Empty,
                                                attr.GetAttributeConstructureArgs(),
                                                attr.GetAttributeNamedArgs());

                                                attrInfos.Add(attrInfo);
                                            }

                                            return new PropertyInfo(
                                                        GetParameterName(p, toSnakeCase),
                                                        p.Name,
                                                        GetNullableSymbolType(p.Type),
                                                        attrInfos.DrainToImmutable());
                                        })
                                        .ToImmutableArray();

            return new Result<ClassInfo>(
                new ClassInfo(classSymbol.Name, namespaceName, baseUrl, properties),
                diagnostics.DrainToImmutable());
        }

        /// <summary>
        /// Gets the nullable type of the symbol.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        private static ITypeSymbol GetNullableSymbolType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol namedType &&
                namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
            {
                return namedType.TypeArguments[0];
            }

            return typeSymbol;
        }

        /// <summary>
        /// Checks if the property is ignored by the QueryUrlParams generator.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsIgnoredProperty(IPropertySymbol property)
        {
            return property.GetAttributes()
                .Any(a => a.AttributeClass?.Name == queryParameterIgnoreAttributeName);
        }

        /// <summary>
        /// Gets the parameter name for the property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="toSnakeCase"></param>
        /// <returns></returns>
        private static string GetParameterName(IPropertySymbol property, bool toSnakeCase = true)
        {
            var attribute = property.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == queryParameterNameAttributeName);

            if (attribute != null && 
                attribute.ConstructorArguments[0].Value != null)
            {
                return attribute.ConstructorArguments[0].Value!.ToString().ToLower();
            }

            return toSnakeCase ? 
                Regex.Replace(property.Name, "(?<=.)([A-Z]+)", "_$1").ToLower() :
                property.Name.ToLower();
        }
    }
}
