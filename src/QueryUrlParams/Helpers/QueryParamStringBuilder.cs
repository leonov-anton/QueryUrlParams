using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace QueryUrlParams.Helpers
{
    public static class QueryParamStringBuilder
    {
        public static void AppendParam<T>(StringBuilder sb, string key, T value)
        {
            switch (value)
            {
                case null:
                    return;

                case string s:
                    AppendParam(sb, key, s);
                    break;

                case bool b:
                    AppendParam(sb, key, b ? "true" : "false");
                    break;

                case IFormattable formattable:
                    AppendParam(sb, key, formattable.ToString(null, CultureInfo.InvariantCulture));
                    break;

                default:
                    AppendParam(sb, key, value.ToString());
                    break;
            }
        }

        public static void AppendParam(StringBuilder sb, string key, DateTime? value, string format = "yyyy-MM-ddTHH:mm:ssZ")
        {
            if (value == null) return;
            AppendParam(sb, key, value.Value.ToString(format, CultureInfo.InvariantCulture));
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
                if (value == null) continue;
                AppendParam(sb, key, value);
            }
        }

        public static void AppendParams<TKey, TValue>(StringBuilder sb, string key, IDictionary<TKey, TValue> dict)
        {
            if (dict == null || !dict.Keys.Any()) return;
            
            foreach (var kvp in dict)
            {
                if (kvp.Key == null || kvp.Value == null) continue;
                AppendParam(sb, $"{kvp.Key}", kvp.Value);
            }
        }

        private static void AppendParam(StringBuilder sb, string key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (sb.Length > 0)
                sb.Append("&");

            sb.Append(key);
            sb.Append("=");
            sb.Append(Uri.EscapeDataString(value));
        }
    }
}
