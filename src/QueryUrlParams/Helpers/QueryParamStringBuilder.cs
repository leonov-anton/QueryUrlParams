using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace QueryUrlParams.Helpers
{
    public static class QueryParamStringBuilder
    {
        public static void AppendParam(StringBuilder sb, string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (sb.Length > 0)
                sb.Append("&");

            sb.Append(key);
            sb.Append("=");
            sb.Append(Uri.EscapeDataString(value));
        }

        public static void AppendParam(StringBuilder sb, string key, double? value)
        {
            if (value == null) return;
            AppendParam(sb, key, value?.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public static void AppendParam(StringBuilder sb, string key, int? value)
        {
            if (value == null) return;
            AppendParam(sb, key, value?.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public static void AppendParam(StringBuilder sb, string key, decimal? value)
        {
            if (value == null) return;
            AppendParam(sb, key, value?.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public static void AppendParam(StringBuilder sb, string key, bool? value)
        {
            if (value == null) return;
            AppendParam(sb, key, value?.ToString().ToLowerInvariant());
        }

        public static void AppendParam<TEnum>(StringBuilder sb, string key, TEnum? value, bool isString = false) where TEnum : struct, Enum
        {
            if (value == null) return;

            if (isString)
                AppendParam(sb, key, value.ToString());
            else  
                AppendParam(sb, key, Convert.ToInt64(value).ToString());
        }

        public static void AppendParams<T>(StringBuilder sb, string key, IEnumerable<T> values)
        {
            if (values == null || !values.Any()) return;

            foreach (var value in values)
            {
                AppendParam(sb, key, value?.ToString());
            }
        }

        public static void AppendParams<TKey, TValue>(StringBuilder sb, string key, IDictionary<TKey, TValue> dict)
        {
            if (dict == null || !dict.Keys.Any()) return;
            
            foreach (var kvp in dict)
            {
                if (kvp.Key == null || kvp.Value == null) continue;
                AppendParam(sb, $"{kvp.Key}", kvp.Value?.ToString());
            }
        }
    }
}
