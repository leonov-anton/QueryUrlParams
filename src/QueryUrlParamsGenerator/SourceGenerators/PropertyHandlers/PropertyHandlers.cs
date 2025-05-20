using Microsoft.CodeAnalysis;
using QueryUrlParamsGenerator.Models;
using QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers.Base;
using System;
using System.Linq;

namespace QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers
{
    internal class InnerUrlParametersDtoPropertyHandler : PropertyHandlerBase
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

            return $"ifparameters.AddRange(global::{namespaceName}.{classSymbol.Name}Extensions.GetObjectUrlParams(obj.{prop.OriginalName}));";
        }
    }

    internal class DictionaryPropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.MetadataName == "Dictionary`2";

        public override string GetStatement(PropertyInfo prop) =>
            $"parameters.AddRange({queryParamBuilderNamespase}.FromDictionary(obj.{prop.OriginalName}, \"{prop.Name}\"));";
    }

    internal class EnumerablePropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.AllInterfaces.Any(i =>
                i.OriginalDefinition.SpecialType == SpecialType.System_Collections_IEnumerable
                && prop.Type.SpecialType != SpecialType.System_String);

        public override string GetStatement(PropertyInfo prop) =>
        $"parameters.AddRange({queryParamBuilderNamespase}.FromEnumerable(obj.{prop.OriginalName}, \"{prop.Name}\"));";
    }

    internal class StringPropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_String;
        public override string GetStatement(PropertyInfo prop) =>
            $"parameters.Add({queryParamBuilderNamespase}.FromString(obj.{prop.OriginalName}, \"{prop.Name}\"));";
    }

    internal class DoublePropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Double;
        public override string GetStatement(PropertyInfo prop) =>
            $"parameters.Add({queryParamBuilderNamespase}.FromDouble(obj.{prop.OriginalName}, \"{prop.Name}\"));";
    }

    internal class IntPropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Int32;
        public override string GetStatement(PropertyInfo prop) =>
            $"parameters.Add({queryParamBuilderNamespase}.FromInt(obj.{prop.OriginalName}, \"{prop.Name}\"));";
    }

    internal class DateTimePropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_DateTime;
        
        public override string GetStatement(PropertyInfo prop)
        {
            string format = string.Empty;

            var dateTimeAttr = prop.AttributeInfos.FirstOrDefault(a => a.TypeName == "DateTimeFormatAttribute");
            if (dateTimeAttr != null)
            {
                format = dateTimeAttr.NamedArgumentInfo
                                .OfType<StringArgumentInfo>()
                                .FirstOrDefault(a => a.Name.ToLower() == "format")?
                                .Value ?? string.Empty;

                if (string.IsNullOrWhiteSpace(format))
                    format = dateTimeAttr.ConstructorArgumentInfo
                                .OfType<StringArgumentInfo>()
                                .FirstOrDefault(a => a.Name.ToLower() == "format")?
                                .Value ?? string.Empty;
            }

            return $"parameters.Add({queryParamBuilderNamespase}.FromDateTime(obj.{prop.OriginalName}, \"{prop.Name}\", \"{format}\"));";
        }            
    }

    internal class BooleanPropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) =>
            prop.Type.SpecialType == SpecialType.System_Boolean;
        public override string GetStatement(PropertyInfo prop) =>
            $"parameters.Add({queryParamBuilderNamespase}.FromBool(obj.{prop.OriginalName}, \"{prop.Name}\"));";
    }

    internal class DefaultPropertyHandler : PropertyHandlerBase
    {
        public override bool CanHandle(PropertyInfo prop) => true;
        
        public override string GetStatement(PropertyInfo prop) =>
            $"parameters.Add({queryParamBuilderNamespase}.FromObject(obj.{prop.OriginalName}.ToString(), \"{prop.Name}\"));";
    }
}
