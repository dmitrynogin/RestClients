using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestClients.Formatting
{
    static class Formatter
    {
        public static string Format(this string format, params object[] args) =>
            string.Format(format, args);
        
        public static string Format(this string format, ValueSource source)
        {
            foreach (var name in Regex.Matches(format, "{[^}]+}")
                .OfType<Match>()
                .Select(m => m.Groups[0].Value.Trim('{', '}')))
                if (source[name] != null)
                    format = format.Replace("{" + name + "}", source[name]);

            return format;
        }            
    }
}
