using QueryUrlParamsGenerator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers.Base
{
    internal abstract class PropertyHandlerBase : IPropertyHandler
    {
        protected const string queryParamBuilderNamespase = "global::QueryUrlParams.Helpers.QueryParamStringBuilder";

        public abstract bool CanHandle(PropertyInfo prop);
        public abstract string GetStatement(PropertyInfo prop);
    }
}
