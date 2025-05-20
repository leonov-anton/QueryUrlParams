using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace QueryUrlParamsGenerator.Models
{
    public record AttributeArgumentInfo(string Name);
    
    internal sealed record StringArgumentInfo(string Name, string Value) : AttributeArgumentInfo(Name);

    internal sealed record BoolArgumentInfo(string Name, bool Value) : AttributeArgumentInfo(Name);

    internal sealed record IntArgumentInfo(string Name, int Value) : AttributeArgumentInfo(Name);

    internal sealed record DoubleArgumentInfo(string Name, double Value) : AttributeArgumentInfo(Name);
}
