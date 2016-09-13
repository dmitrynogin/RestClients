using RestClients.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClients.Models
{
    public class Url
    {
        public static implicit operator Uri(Url format) => new Uri(format.Value);
        
        public Url(string baseUri, string relativeUri, params object[] parameters)
        {
            var format = new Uri(
                new Uri(baseUri), relativeUri)
                .ToString();

            Value = format.Format(parameters);
        }

        string Value { get; }
        public override string ToString() => Value;
    }
}
