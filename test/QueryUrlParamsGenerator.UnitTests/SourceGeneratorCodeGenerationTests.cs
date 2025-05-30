using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using FluentAssertions;
using QueryUrlParamsGenerator.SourceGenerators;

namespace QueryUrlParamsGenerator.UnitTests
{
    public class SourceGeneratorCodeGenerationTests
    {
        [Fact]
        public void GenerateQueryUrl_WithoutPropertyAttributes()
        {
            string source_code = """
                using QueryUrlParams.Attributes;
                using System.Collections.Generic;
                using System;

                namespace TestApp
                {
                    [GenerateQueryUrl]
                    public class SomeUrlParams
                    {
                        public string? Name { get; set; }
                        public int? Age { get; set; }
                        public double? Address { get; set; }
                        public string? Email { get; set; }
                        public bool? IsValid { get; set; }
                        public List<string>? Tags { get; set; }
                        public Dictionary<string, string>? Metadata { get; set; }
                        public DateTime? StartTime { get; set; }
                    }
                }
                """;

            string expected_code = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "name", obj.Name);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "age", obj.Age);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "address", obj.Address);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "email", obj.Email);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "is_valid", obj.IsValid);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParams(sb, "tags", obj.Tags);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParams(sb, "metadata", obj.Metadata);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "start_time", obj.StartTime, "yyyy-MM-ddTHH:mm:ssZ");
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(source_code, expected_code, "SomeUrlParamsExtensions.g.cs");
        }

        [Fact]
        public void GenerateQueryUrl_WithoutPropertyAttributes_WithUrlBase()
        {
            string source_code = """
                using QueryUrlParams.Attributes;

                namespace TestApp
                {
                    [GenerateQueryUrl("example.com/api")]
                    public class SomeUrlParams
                    {
                        public string? Name { get; set; }
                        public int? Age { get; set; }
                        public string? Address { get; set; }
                        public string? PhoneNumber { get; set; }
                        public string? Email { get; set; }
                    }
                }
                """;

            string expected_code = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "example.com/api";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "name", obj.Name);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "age", obj.Age);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "address", obj.Address);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "phone_number", obj.PhoneNumber);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "email", obj.Email);
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(source_code, expected_code, "SomeUrlParamsExtensions.g.cs");
        }

        [Fact]
        public void GenerateQueryUrl_IgnoreproprtyAttribute_GetObjectUrlParamsNotIncludeIgnoringParam()
        {
            string source_code = """
                using QueryUrlParams.Attributes;

                namespace TestApp
                {
                    [GenerateQueryUrl]
                    public class SomeUrlParams
                    {
                        public string? Name { get; set; }
                        public int? Age { get; set; }
                        public string? Address { get; set; }
                        public string? PhoneNumber { get; set; }
                        [QueryParameterIgnore]
                        public string? Email { get; set; }
                    }
                }
                """;

            string expected_code = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "name", obj.Name);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "age", obj.Age);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "address", obj.Address);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "phone_number", obj.PhoneNumber);
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(source_code, expected_code, "SomeUrlParamsExtensions.g.cs");
        }

        [Fact]
        public void GenerateQueryUrl_QueryParameterNameAttribute_RenameParameter()
        {
            string source_code = """
                using QueryUrlParams.Attributes;

                namespace TestApp
                {
                    [GenerateQueryUrl]
                    public class SomeUrlParams
                    {
                        [QueryParameterName("user_name")]
                        public string? Name { get; set; }
                        public int? Age { get; set; }
                        public string? Address { get; set; }
                        public string? PhoneNumber { get; set; }
                        public string? Email { get; set; }
                    }
                }
                """;

            string expected_code = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "user_name", obj.Name);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "age", obj.Age);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "address", obj.Address);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "phone_number", obj.PhoneNumber);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "email", obj.Email);
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(source_code, expected_code, "SomeUrlParamsExtensions.g.cs");
        }

        [Fact]
        public void GenerateQueryUrl_WrongDateTimeFormatAttributeMarking_GenerateDiagnosticWarning()
        {
            string source_code = """
                using QueryUrlParams.Attributes;

                namespace TestApp
                {
                    [GenerateQueryUrl]
                    public class SomeUrlParams
                    {
                        [DateTimeFormat("o")]
                        public string? Name { get; set; }
                    }
                }
                """;

            string expected_code = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "name", obj.Name);
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(source_code, expected_code, "SomeUrlParamsExtensions.g.cs", "DateTime format attribute is not supported");
        }

        [Fact]
        public void GenerateQueryUrl_EnumAsStringAttributeMarking_GenerateDiagnosticWarning()
        {
            string source_code = """
                using QueryUrlParams.Attributes;

                namespace TestApp
                {
                    [GenerateQueryUrl]
                    public class SomeUrlParams
                    {
                        [EnumAsString]
                        public string? Name { get; set; }
                    }
                }
                """;

            string expected_code = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "name", obj.Name);
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(source_code, expected_code, "SomeUrlParamsExtensions.g.cs", "Enum as string attribute is not supported");
        }

        [Fact]
        public void GenerateQueryUrl_WithInnerMarkedClass()
        {

            string sourceCode = """
                using QueryUrlParams.Attributes;

                namespace TestApp
                {
                    [GenerateQueryUrl]
                    public class InnerUrlParams
                    {
                        public string? Name { get; set; }
                    }

                    [GenerateQueryUrl]
                    public class SomeUrlParams
                    {
                        public string? Description { get; set; }
                        public InnerUrlParams? InnerParams { get; set; }
                    }
                }
                """;

            string expectedCode = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "description", obj.Description);
                            if (sb.Length > 0)
                                sb.Append("&");
                            sb.Append(global::TestApp.InnerUrlParamsExtensions.GetObjectUrlParams(obj.InnerParams));
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(sourceCode, expectedCode, "SomeUrlParamsExtensions.g.cs");
        }

        [Fact]
        public void GenerateQueryUrl_Enum_Params()
        {
            string sourceCode = """
                using QueryUrlParams.Attributes;

                namespace TestApp
                {
                    [GenerateQueryUrl]
                    public class SomeUrlParams
                    {
                        public Status CurrentStatus { get; set; }
                    }

                    public enum Status
                    {
                        Active,
                        Inactive,
                        Pending
                    }
                }
                """;

            string expectedCode = """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                namespace TestApp
                {
                    /// <summary>
                    /// Extension methods for generating query URLs from <see cref="SomeUrlParams"/>.
                    /// <summary>
                    public static class SomeUrlParamsExtensions
                    {
                        /// <summary>Base uri from GenerateQueryUrlAttribute.</summary>
                        private const string _defaultBaseUrl = "";
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the default base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj) => obj.ToQueryUrl(_defaultBaseUrl);
                        /// <summary>
                        /// Converts the <see cref="SomeUrlParams"/> object to a query URL using the specified base URL.
                        /// <summary>
                        /// <param name="obj">The source object containing query parameters.</param>
                        /// <param name="baseUrl">The base URL to use.</param>
                        /// <returns>A query URL string.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string ToQueryUrl(this SomeUrlParams obj, string baseUrl)
                        {
                            var parameters = GetObjectUrlParams(obj);
                            if (string.IsNullOrEmpty(parameters))
                                return baseUrl;
                            return string.Concat(baseUrl, "?", parameters);
                        }

                        /// <summary>
                        /// Extracts the list of query parameters from the <see cref="SomeUrlParams"/> object.
                        /// <summary>
                        /// <param name="obj">The source object containing parameter values.</param>
                        /// <returns>A string of query parameter with & separator.</returns>
                        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                        public static string GetObjectUrlParams(SomeUrlParams obj)
                        {
                            if (obj == null)
                                return string.Empty;
                            var sb = new global::System.Text.StringBuilder(256);
                            global::QueryUrlParams.Helpers.QueryParamStringBuilder.AppendParam(sb, "current_status", obj.CurrentStatus, false);
                            return sb.ToString();
                        }
                    }
                }
                """;

            VerifyGeneratedCode(sourceCode, expectedCode, "SomeUrlParamsExtensions.g.cs");
        }

        private static void VerifyGeneratedCode(string sourceCode, string expectedCode, string filePath, params string[] diagnosticTitles)
        {
            // Simple way load all assembly references from the current domain
            // Load query url params assembly
            Type generateQueryUrlAttribute = typeof(QueryUrlParams.Attributes.GenerateQueryUrlAttribute);

            // Get all assembly references for the loaded assemblies
            var references = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location));

            // Simulate the code generation process
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));

            CSharpCompilation compilation = CSharpCompilation.Create(
                                                assemblyName: "UnitTests",
                                                syntaxTrees: [syntaxTree],
                                                references: references,
                                                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var dtSymbol = compilation.GetTypeByMetadataName("System.DateTime");
            Console.WriteLine(dtSymbol?.SpecialType); // ������ ����: System_DateTime
            Console.WriteLine(dtSymbol?.ContainingAssembly.Identity);

            var generators = new[] { new QueryUrlGenerator() };

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generators).WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

            _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

            if (diagnosticTitles.Length != 0)
            {
                var generatedDiagnostics = diagnostics.Select(d => d.Descriptor.Title.ToString());
                generatedDiagnostics.Should().BeEquivalentTo(diagnosticTitles, "Generated diagnostics titels should match expected titles.");
            }

            SyntaxTree generatedTree = outputCompilation.SyntaxTrees.Single(tree => Path.GetFileName(tree.FilePath) == filePath);

            generatedTree.Should().NotBeNull("Generated tree should not be null.");

            var compilationResultText = generatedTree.ToString();

            // Compare the generated code with the expected code
            // replace line endings to ensure consistency across different environments
            compilationResultText.Replace("\r\n", "\n").Should().Be(expectedCode.Replace("\r\n", "\n"), "Generated code should match the expected code.");
        }
    }
}