using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace QueryUrlParamsGenerator.Models
{
    internal sealed record Result<T>(T Value, ImmutableArray<Diagnostic> Diagnostics) 
        where T : IEquatable<T>?
    {
    }
}
