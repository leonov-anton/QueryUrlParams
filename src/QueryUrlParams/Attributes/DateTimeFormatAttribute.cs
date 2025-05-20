using System;
using System.Collections.Generic;
using System.Text;

namespace QueryUrlParams.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeFormatAttribute : Attribute
    {
        public string Format { get; }
        public DateTimeFormatAttribute(string format)
        {
            Format = format;
        }
    }
}
