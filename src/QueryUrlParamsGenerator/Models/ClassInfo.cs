using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QueryUrlParamsGenerator.Models
{
    public sealed record ClassInfo(
            string Name,
            string Namespace,
            string BaseUrl,
            ImmutableArray<PropertyInfo?> Properties);
}
