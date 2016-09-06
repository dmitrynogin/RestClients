using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestClients.Models
{
    public abstract class Header
    {
        protected Header(string format)
        {
            Format = format;
        }
        
        public abstract bool TryRead(HttpResponseMessage message);
        public abstract bool TryWrite(HttpRequestMessage message);
        public abstract bool TryGet<T>(int index, ref T value);

        protected string Format { get; }
    }

    public class RequestHeader : Header
    {
        public RequestHeader(string format, params object[] args) 
            : base(format)
        {
            Args = args;
        }

        public override bool TryRead(HttpResponseMessage message)
        {
            return false;
        }

        public override bool TryWrite(HttpRequestMessage message)
        {
            var text = string.Format(Format, Args);
            var name = text.Split(':').FirstOrDefault() ?? "";
            var value = string.Join(":", text.Split(':').Skip(1));
            message.Headers.TryAddWithoutValidation(name, value);
            return true;
        }

        public override bool TryGet<T>(int index, ref T value)
        {
            return false;
        }

        protected object[] Args { get; }
    }

    public class ResponseHeader : Header
    {
        public ResponseHeader(string format)
            : base(format)
        {
        }

        public override bool TryRead(HttpResponseMessage message)
        {
            var pattern = string.Format(Format, Enumerable.Range(0, 200).Select(i => $"(?'{i}'.*)").ToArray());
            foreach (var header in message.Headers.Select(h => $"{h.Key}: {string.Join("; ", h.Value)}"))
            {
                var groups = Regex.Match(header.ToString(), pattern).Groups;
                foreach (var i in Regex
                    .Matches(Format, @"{\d+}")
                    .OfType<Match>()
                    .Select(m => m.Groups[0].Value.Trim('{', '}')))
                    if(groups[i].Success)
                        Parameters[int.Parse(i)] = groups[i].Value;
            }

            return true;
        }

        public override bool TryWrite(HttpRequestMessage message)
        {
            return false;
        }

        public override bool TryGet<T>(int index, ref T value)
        {
            value = default(T);
            string text;
            if(!Parameters.TryGetValue(index, out text))
                return false;

            return text.TryTo(out value);
        }

        Dictionary<int, string> Parameters { get; } = new Dictionary<int, string>();
    }

    public static class StringHelpers
    {
        public static bool TryTo<T>(this string text, out T value)
        {
            value = default(T);
            if (text == null)
                return true;
                
            var converter = TypeDescriptor
                .GetConverter(typeof(T));

            if (!converter.CanConvertFrom(typeof(string)))
                return false;

            value = (T)converter.ConvertFromInvariantString(text);
            return true;
        }
    }
}
