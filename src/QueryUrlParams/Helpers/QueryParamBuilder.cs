using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace QueryUrlParams.Helpers
{
    public static class QueryParamBuilder
    {
        public static IEnumerable<string> FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict, string keyPrefix)
        {
            if (dict == null || !dict.Keys.Any()) return Enumerable.Empty<string>();

            var parameters = new List<string>();

            foreach (var kvp in dict)
            {
                if (kvp.Key == null || kvp.Value == null) continue;
                
                parameters.Add($"{Uri.EscapeDataString(kvp.Key.ToString())}={Uri.EscapeDataString(kvp.Value.ToString())}");
            }

            return parameters;
        }

        public static IEnumerable<string> FromEnumerable<T>(IEnumerable<T> enumerable, string keyPrefix)
        {
            if (enumerable == null || !enumerable.Any()) return Enumerable.Empty<string>();

            var parameters = new List<string>();
            
            foreach (var item in enumerable)
            {
                if (item == null) continue;

                parameters.Add($"{keyPrefix}={Uri.EscapeDataString(item.ToString())}");
            }

            return parameters;
        }

        public static string FromString(string value, string keyPrefix)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            
            return $"{keyPrefix}={Uri.EscapeDataString(value)}";
        }

        public static string FromDouble(double? value, string keyPrefix)
        {
            if (value == null) return string.Empty;

            return $"{keyPrefix}={Uri.EscapeDataString(value?.ToString(CultureInfo.InvariantCulture)!)}";
        }

        public static string FromInt(int? value, string keyPrefix)
        {
            if (value == null) return string.Empty;

            return $"{keyPrefix}={Uri.EscapeDataString(value?.ToString(CultureInfo.InvariantCulture)!)}";
        }

        public static string FromDateTime(DateTime? value, string keyPrefix, string format)
        {
            if (value == null) return string.Empty;

            if (string.IsNullOrWhiteSpace(format))
            {
                format = "yyyy-MM-ddTHH:mm:ssZ";
            }

            return $"{keyPrefix}={Uri.EscapeDataString(value?.ToString(format))}";
        }

        public static string FromDateTimeOffset(DateTimeOffset? value, string keyPrefix)
        {
            if (value == null) return string.Empty;
            return $"{keyPrefix}={Uri.EscapeDataString(value?.ToString("yyyy-MM-ddTHH:mm:ssZ"))}";
        }

        public static string FromBool(bool? value, string keyPrefix)
        {
            if (value == null) return string.Empty;
            return $"{keyPrefix}={Uri.EscapeDataString(value.ToString()!.ToLowerInvariant())}";
        }

        public static string FromGuid(Guid? value, string keyPrefix)
        {
            if (value == null) return string.Empty;
            return $"{keyPrefix}={Uri.EscapeDataString(value.ToString()!)}";
        }

        public static string FromObject(object value, string keyPrefix)
        {
            if (value == null) return string.Empty;
            return $"{keyPrefix}={Uri.EscapeDataString(value.ToString()!)}";
        }
    }
}
