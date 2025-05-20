using QueryUrlParamsGenerator.Models;

namespace QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers.Base
{
    internal interface IPropertyHandler
    {
        bool CanHandle(PropertyInfo prop);
        string GetStatement(PropertyInfo prop);
    }
}
