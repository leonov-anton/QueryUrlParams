using System;
using System.Collections.Generic;
using System.Text;

namespace QueryUrlParams.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class QueryParameterNameAttribute : Attribute
    {
        public string Name { get; }
        public QueryParameterNameAttribute(string name) => 
            Name = name;
    }
}
