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
                new InnerUrlParametersDtoPropertyHandler(),
                new DictionaryPropertyHandler(),
                new EnumerablePropertyHandler(),
                new StringPropertyHandler(),
                new DoublePropertyHandler(),
                new IntPropertyHandler(),
                new DateTimePropertyHandler(),
                new BooleanPropertyHandler(),
                new DefaultPropertyHandler()
            };
        }

        public string GetStatement(PropertyInfo prop)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(prop));
            return handler?.GetStatement(prop)
               ?? throw new InvalidOperationException($"Unsupported property type of property - {prop.Name}");
        }
    }
}
