using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace QueryUrlParamsGenerator.Models
{
    public record PropertyInfo(
            string Name,
            string OriginalName,
            ITypeSymbol Type,
            ImmutableArray<AttributeInfo> AttributeInfos);
}
