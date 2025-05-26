using Microsoft.CodeAnalysis;
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
        
        public override string GetStatement(PropertyInfo prop)
        {
            var classSymbol = (INamedTypeSymbol)prop.Type;

            var namespaceSymbol = classSymbol.ContainingNamespace;
            var namespaceName = namespaceSymbol?.IsGlobalNamespace == true
                ? string.Empty
                : namespaceSymbol?.ToString() ?? "Generated";

            return $$"""
                    if (sb.Length > 0) 
                        sb.Append("&");

                    sb.Append(global::{{namespaceName}}.{{classSymbol.Name}}Extensions.GetObjectUrlParams(obj.{{prop.OriginalName}}));
                   """;
        }
    }

    internal class DictionaryPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.MetadataName == "Dictionary`2";

        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParams(sb, \"{prop.Name}\", obj.{prop.OriginalName});";
    }

    internal class EnumerablePropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.AllInterfaces.Any(i =>
                i.OriginalDefinition.SpecialType == SpecialType.System_Collections_IEnumerable
                && prop.Type.SpecialType != SpecialType.System_String);

        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParams(sb, \"{prop.Name}\", obj.{prop.OriginalName});";
    }

    internal class StringPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_String;
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName});";
    }

    internal class DoublePropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Double;
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName});";
    }

    internal class IntPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Int32;
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName});";
    }

    internal class DateTimePropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_DateTime;
        
        public override string GetStatement(PropertyInfo prop)
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

            return $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}?.ToString(\"{format}\"));";
        }
    }

    internal class BooleanPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Boolean;
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName});";
    }

    internal class DecimalPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Decimal;

        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName});";
    }

    internal class EnumPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.TypeKind == TypeKind.Enum;
        public override string GetStatement(PropertyInfo prop)
        {
            bool isString = prop.AttributeInfos.Any(a => a.TypeName == "EnumAsStringAttribute");
            return $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}, {isString.ToString().ToLowerInvariant()});";
        }
    }

    internal class DefaultPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) => true;
        
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}?.ToString());";
    }
}
