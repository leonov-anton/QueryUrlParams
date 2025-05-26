using System;
using System.Collections.Generic;
using System.Text;

namespace QueryUrlParams.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EnumAsStringAttribute : Attribute
    {
        public EnumAsStringAttribute() { }
    }
}
