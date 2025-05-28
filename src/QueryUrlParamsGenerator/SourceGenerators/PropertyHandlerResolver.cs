using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using QueryUrlParamsGenerator.Models;
using QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers;
using QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryUrlParamsGenerator.SourceGenerators
{
    internal class PropertyHandlerResolver
    {
        private readonly List<IPropertyHandler> _handlers;

        public PropertyHandlerResolver()
        {
            _handlers = new List<IPropertyHandler>
            {
                new InnerUrlParametersDtoPropertyHandlerStringBuilder(),
                new DictionaryPropertyHandlerStringBuilder(),
                new EnumerablePropertyHandlerStringBuilder(),
                new StringPropertyHandlerStringBuilder(),
                new DoublePropertyHandlerStringBuilder(),
                new IntPropertyHandlerStringBuilder(),
                new DateTimePropertyHandlerStringBuilder(),
                new BooleanPropertyHandlerStringBuilder(),
                new DecimalPropertyHandlerStringBuilder(),
                new EnumPropertyHandlerStringBuilder(),
                new DefaultPropertyHandlerStringBuilder()
            };
        }

        public StatementSyntax[] GetStatement(PropertyInfo prop)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(prop));
            return handler?.GetStatements(prop)
               ?? throw new InvalidOperationException($"Unsupported property type of property - {prop.Name}");
        }
    }
}
