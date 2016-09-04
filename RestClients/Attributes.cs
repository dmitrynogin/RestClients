using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClients
{
    public abstract class UriAttribute : System.Attribute
    {
        protected UriAttribute(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; }
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class SiteAttribute : UriAttribute
    {
        public SiteAttribute(string uri)
            : base(uri)
        {
        }

        public Type Error { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : UriAttribute
    {
        public GetAttribute(string uri = "")
            : base(uri)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : UriAttribute
    {
        public PostAttribute(string uri = "")
            : base(uri)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : UriAttribute
    {
        public PutAttribute(string uri = "")
            : base(uri)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteAttribute : UriAttribute
    {
        public DeleteAttribute(string uri = "")
            : base(uri)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class BodyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HeaderAttribute : Attribute
    {
        public HeaderAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; }
    }
}
