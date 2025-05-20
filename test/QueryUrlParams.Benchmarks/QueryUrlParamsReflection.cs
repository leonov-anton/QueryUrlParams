using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryUrlParams.Benchmarks
{
    internal static class QueryUrlParamsReflection
    {
        public static string ToQueryUrl(object obj, string baseUrl, bool snakeCase)
        {
            if (obj == null)
                return string.Empty;

            var properties = obj.GetType().GetProperties();
            var queryParams = new List<string>();
            
            foreach (var property in properties)
            {
                var name = snakeCase ? ToSnakeCase(property.Name) : property.Name;
                var value = property.GetValue(obj);
                
                if (value != null)
                {
                    if (value is Dictionary<string, object>)
                    {
                        foreach (var kvp in (Dictionary<string, object>)value)
                        {
                            if (kvp.Key is null || kvp.Value is null) continue;
                            queryParams.Add($"{kvp.Key}={Uri.EscapeDataString(ConvertToString(kvp.Value))}");
                        }
                    }
                    else if (value is IEnumerable<object> enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            queryParams.Add($"{property.Name}={ConvertToString(item)}");
                        }
                    }
                    else
                    {
                        queryParams.Add($"{property.Name}={ConvertToString(value)}");
                    }
                }
            }
            
            return queryParams.Any() ? $"{baseUrl}?{string.Join("&", queryParams)}" : baseUrl;
        }

        private static string ConvertToString(object value)
        {
            return value switch
            {
                DateTime dt => dt.ToString("o"), // ISO 8601
                bool b => b.ToString().ToLowerInvariant(),
                _ => value.ToString()
            };
        }

        private static string ToSnakeCase(string input) =>
            Regex.Replace(input, "(?<=.)([A-Z]+)", "_$1").ToLower();
    }
}
