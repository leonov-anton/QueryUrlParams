using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace QueryUrlParamsGenerator.Models
{
    public sealed record AttributeInfo(
        string TypeName,
        ImmutableArray<AttributeArgumentInfo> ConstructorArgumentInfo,
        ImmutableArray<AttributeArgumentInfo> NamedArgumentInfo);
}
