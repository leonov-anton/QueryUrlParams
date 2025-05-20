using System;

namespace QueryUrlParams.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class GenerateQueryUrlAttribute : Attribute
    {
        public string BaseUrl { get; set; }

        public bool SnakeCaseNameConvert { get; set; } = true;

        public GenerateQueryUrlAttribute()
        {
            BaseUrl = string.Empty;
        }

        public GenerateQueryUrlAttribute(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public GenerateQueryUrlAttribute(string baseUrl, bool snakeCaseNameConvert)
        {
            BaseUrl = baseUrl;
            SnakeCaseNameConvert = snakeCaseNameConvert;
        }

        public GenerateQueryUrlAttribute(bool snakeCaseNameConvert)
        {
            BaseUrl = string.Empty;
            SnakeCaseNameConvert = snakeCaseNameConvert;
        }
    }
}
