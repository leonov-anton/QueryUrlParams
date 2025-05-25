using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using QueryUrlParamsGenerator.Extensions;
using QueryUrlParamsGenerator.Models;
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

        private static PropertyHandlerResolver _propHandlers;

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
                .Where(static target => !target.Diagnostics.Any() && target.Value != null)
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
            // Create the using directives
            var usings = new[]
            {
                UsingDirective(ParseName("System")),
                UsingDirective(ParseName("System.Text")),
                UsingDirective(ParseName("System.Collections.Generic")),
                UsingDirective(ParseName("System.Linq")),
            };

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
                .WithBody(GetToQueryUrlMethodSyntax())
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .WithLeadingTrivia(TriviaList(
                    Comment("/// <summary>"),
                    Comment($"/// Converts the <see cref=\"{classInfo.Name}\"/> object to a query URL using the default base URL."),
                    Comment("/// <summary>"),
                    Comment($"/// <param name=\"obj\">The source object containing query parameters.</param>"),
                    Comment($"/// <returns>A query URL string.</returns>")
                    ));

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
                .WithUsings(List(usings))
                .WithLeadingTrivia(
                        TriviaList(
                            Comment("// <auto-generated/>"),
                            Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)),
                            Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                .AddMembers(
                    namespaceDecl.AddMembers(classDecl))
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

            block = block.AddStatements(ParseStatement("var parameters = GetObjectUrlParams(obj);"));
            block = block.AddStatements(ParseStatement("if (parameters.Count() == 0) return baseUrl;"));
            block = block.AddStatements(ParseStatement($"return baseUrl + \"?\" + string.Join(\"&\", parameters);"));

            return block;
        }

        /// <summary>
        /// Generates the body of the ToQueryUrl method without a base URL.
        /// Based on the default base URL from the class attribute.
        /// </summary>
        /// <returns></returns>
        private static BlockSyntax GetToQueryUrlMethodSyntax()
        {
            var block = Block();

            block = block.AddStatements(ParseStatement($"return obj.ToQueryUrl(_defaultBaseUrl);"));

            return block;
        }

        /// <summary>
        /// Generates the body of the GetObjectUrlParams method.
        /// </summary>
        /// <param name="classInfo"></param>
        /// <returns></returns>
        private static BlockSyntax GetObjectUrlParamsMethodSyntax(ClassInfo classInfo)
        {
            var block = Block();

            block = block.AddStatements(ParseStatement($"var sb = new StringBuilder(256);"));

            //block = block.AddStatements(ParseStatement("var parameters = new List<string>();"));

            foreach (var prop in classInfo.Properties)
            {
                var statement = _propHandlers.GetStatement(prop);
                block = block.AddStatements(ParseStatement(statement));
            }

            //block = block.AddStatements(ParseStatement("return parameters.Where(p => !string.IsNullOrWhiteSpace(p));"));.

            block = block.AddStatements(ParseStatement("return sb.ToString();"));

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
                                            if (p.GetAttributes().Any(a => a.AttributeClass?.Name == queryParameterDateTimeFormatAttributeName) &&
                                                GetNullableSymbolType(p.Type).SpecialType != SpecialType.System_DateTime)
                                            {
                                                var diagnostic = Diagnostic.Create(
                                                    new DiagnosticDescriptor(
                                                        "QUPG002",
                                                        "DateTime format attribute is not supported",
                                                        "DateTime format attribute is not supported for property '{0}'",
                                                        "Usage",
                                                        DiagnosticSeverity.Error,
                                                        true),
                                                    p.Locations[0],
                                                    p.Name);
                                                    
                                                diagnostics.Add(diagnostic);

                                                return null;
                                            }

                                            var attrInfos = ImmutableArray.CreateBuilder<AttributeInfo>();

                                            foreach (var attr in p.GetAttributes())
                                            {
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
