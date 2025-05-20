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

            return $"if (sb.Length > 0)\r\n" +
                   $"   sb.Append(\"&\");\r\n" +
                   $"sb.Append(global::{namespaceName}.{classSymbol.Name}Extensions.GetObjectUrlParams(obj.{prop.OriginalName}));";
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
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}?.ToString());";
    }

    internal class IntPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Int32;
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}?.ToString());";
    }

    internal class DateTimePropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_DateTime;
        
        public override string GetStatement(PropertyInfo prop)
        {
            string format = "yyyy-MM-ddTHH:mm:ssZ";

            var dateTimeAttr = prop.AttributeInfos.FirstOrDefault(a => a.TypeName == "DateTimeFormatAttribute");
            if (dateTimeAttr != null)
            {
                format = dateTimeAttr.NamedArgumentInfo
                                .OfType<StringArgumentInfo>()
                                .FirstOrDefault(a => a.Name.ToLower() == "format")?
                                .Value ?? "yyyy-MM-ddTHH:mm:ssZ";

                if (string.IsNullOrWhiteSpace(format))
                    format = dateTimeAttr.ConstructorArgumentInfo
                                .OfType<StringArgumentInfo>()
                                .FirstOrDefault(a => a.Name.ToLower() == "format")?
                                .Value ?? "yyyy-MM-ddTHH:mm:ssZ";
            }

            return $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}?.ToString(\"{format}\"));";
        }
    }

    internal class BooleanPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Boolean;
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}.ToString().ToLowerInvariant());";
    }

    internal class DefaultPropertyHandlerStringBuilder : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) => true;
        
        public override string GetStatement(PropertyInfo prop) =>
            $"{queryParamBuilderNamespase}.AppendParam(sb, \"{prop.Name}\", obj.{prop.OriginalName}?.ToString());";
    }
}
