using Microsoft.CodeAnalysis.CSharp.Syntax;
using QueryUrlParamsGenerator.Models;

namespace QueryUrlParamsGenerator.SourceGenerators.PropertyHandlers.Base
{
    internal interface IPropertyHandler
    {
        bool CanHandle(PropertyInfo prop);
        StatementSyntax[] GetStatements(PropertyInfo prop);
    }
}
