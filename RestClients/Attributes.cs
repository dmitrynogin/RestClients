using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClients
{
    public abstract class Attributes : System.Attribute
    {
        protected Attributes(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; }
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class SiteAttribute : Attributes
    {
        public SiteAttribute(string uri)
            : base(uri)
        {
        }

        public Type Error { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : Attributes
    {
        public GetAttribute(string uri = "")
            : base(uri)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : Attributes
    {
        public PostAttribute(string uri = "")
            : base(uri)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class BodyAttribute : System.Attribute
    {
    }
}
