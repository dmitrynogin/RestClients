using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClients
{
    public class UriFormat
    {
        public static implicit operator Uri(UriFormat format) => new Uri(format.Value);

        public UriFormat(string baseUri, string relativeUri, params object[] parameters)
        {
            var format = new Uri(
                new Uri(baseUri), relativeUri)
                .ToString();

            Value = string.Format(format, parameters);
        }

        string Value { get; }
    }
}
