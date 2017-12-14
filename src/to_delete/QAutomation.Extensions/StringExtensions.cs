using System.Collections.Generic;
using System.Linq;

namespace QAutomation.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string source)
        {
            return !string.IsNullOrEmpty(source) && !string.IsNullOrWhiteSpace(source);
        }

        public static string AsFormat(this string source, params object[] args)
        {
            return string.Format(source, args);
        }

        public static string AsFormat(this string source, IEnumerable<KeyValuePair<string, object>> args)
        {
            return source.AsFormat(args.ToArray());
        }

        public static string AsFormat(this string source, IDictionary<string, object> args)
        {
            return source.AsFormat(args.ToArray());
        }

        public static string AsFormat(this string source, params KeyValuePair<string, object>[] args)
        {
            return args.Aggregate(source,
                (current, keyValuePair) => current.Replace("{" + keyValuePair.Key + "}", keyValuePair.Value.ToString()));
        }
    }
}