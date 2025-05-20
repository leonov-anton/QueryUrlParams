using Microsoft.CodeAnalysis;
using QueryUrlParamsGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace QueryUrlParamsGenerator.Extensions
{
    public static class AttributeDataExtensions
    {
        public static string? GetConstructorStringArgument(this AttributeData attributeData, string parameterName)
        {
            var parameters = attributeData.AttributeConstructor?.Parameters;

            if (parameters is null)
                return null;

            for (int i = 0; i < parameters.Value.Length; i++)
            {
                if (parameters.Value[i].Name.ToLower() == parameterName.ToLower() &&
                    attributeData.ConstructorArguments.Length > i &&
                    attributeData.ConstructorArguments[i].Value is string value)
                    return value;
            }

            return null;
        }

        public static bool? GetConstructorBoolArgument(this AttributeData attributeData, string parameterName)
        {
            var parameters = attributeData.AttributeConstructor?.Parameters;
            if (parameters is null)
                return null;

            for (int i = 0; i < parameters.Value.Length; i++)
            {
                if (parameters.Value[i].Name.ToLower() == parameterName.ToLower() &&
                    attributeData.ConstructorArguments.Length > i &&
                    attributeData.ConstructorArguments[i].Value is bool value)
                    return value;
            }

            return null;
        }

        public static T? GetNamedArgument<T>(this AttributeData attributeData, string name)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                if (namedArg.Key.ToLower() == name.ToLower() &&
                    namedArg.Value.Value is T value)
                    return value;
            }

            return default;
        }

        public static T? GetNamedVauetTypeArgument<T>(this AttributeData attributeData, string name) where T : struct
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                if (namedArg.Key.ToLower() == name.ToLower() &&
                    namedArg.Value.Value is T value)
                    return value;
            }

            return null;
        }

        public static ImmutableArray<AttributeArgumentInfo> GetAttributeConstructureArgs(this AttributeData attributeData)
        {
            var result = ImmutableArray.CreateBuilder<AttributeArgumentInfo>();

            var parameters = attributeData.AttributeConstructor?.Parameters;

            if (parameters is null)
                return result.DrainToImmutable();

            for (int i = 0; i < parameters.Value.Length; i++)
            {
                switch (attributeData.ConstructorArguments[i].Value)
                {
                    case string str:
                        result.Add(new StringArgumentInfo(parameters.Value[i].Name, str));
                        break;
                    case bool b:
                        result.Add(new BoolArgumentInfo(parameters.Value[i].Name, b));
                        break;
                    case int iVal:
                        result.Add(new IntArgumentInfo(parameters.Value[i].Name, iVal));
                        break;
                    case double dVal:
                        result.Add(new DoubleArgumentInfo(parameters.Value[i].Name, dVal));
                        break;
                    default:
                        result.Add(new AttributeArgumentInfo(parameters.Value[i].Name));
                        break;
                }
            }

            return result.DrainToImmutable();
        }

        public static ImmutableArray<AttributeArgumentInfo> GetAttributeNamedArgs(this AttributeData attributeData)
        {
            var result = ImmutableArray.CreateBuilder<AttributeArgumentInfo>();

            foreach (var namedArg in attributeData.NamedArguments)
            {
                switch (namedArg.Value.Value)
                {
                    case string str:
                        result.Add(new StringArgumentInfo(namedArg.Key, str));
                        break;
                    case bool b:
                        result.Add(new BoolArgumentInfo(namedArg.Key, b));
                        break;
                    case int iVal:
                        result.Add(new IntArgumentInfo(namedArg.Key, iVal));
                        break;
                    case double dVal:
                        result.Add(new DoubleArgumentInfo(namedArg.Key, dVal));
                        break;
                    default:
                        result.Add(new AttributeArgumentInfo(namedArg.Key));
                        break;
                }
            }

            return result.DrainToImmutable();
        }
    }
}
