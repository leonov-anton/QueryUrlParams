using System;
using System.Collections.Generic;
using System.Text;

namespace QueryUrlParams.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class QueryParameterIgnoreAttribute : Attribute
    {
        public QueryParameterIgnoreAttribute()
        {
        }
    }
}
