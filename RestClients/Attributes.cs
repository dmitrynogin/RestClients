using Microsoft.SqlServer.Server;
using RestClients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClients
{ 
    public abstract class UriAttribute : Attribute
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

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class ActionAttribute : UriAttribute
    {
        public ActionAttribute(string uri = "")
            : base(uri)
        {
        }

        public abstract string Verb { get; }
        public abstract bool AllowPayload { get; }
    }

    public class GetAttribute : ActionAttribute
    {
        public GetAttribute(string uri = "")
            : base(uri)
        {
        }

        public override string Verb => "Get";
        public override bool AllowPayload => false;
    }

    public class PostAttribute : ActionAttribute
    {
        public PostAttribute(string uri = "")
            : base(uri)
        {
        }

        public override string Verb => "Post";
        public override bool AllowPayload => true;
    }

    public class PutAttribute : ActionAttribute
    {
        public PutAttribute(string uri = "")
            : base(uri)
        {
        }

        public override string Verb => "Put";
        public override bool AllowPayload => true;
    }

    public class DeleteAttribute : ActionAttribute
    {
        public DeleteAttribute(string uri = "")
            : base(uri)
        {
        }

        public override string Verb => "Delete";
        public override bool AllowPayload => false;
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

        public bool IsRequest(MethodInfo method)
        {
            var key = Guid.NewGuid().ToString();
            return string.Format(Format, method
                .GetParameters()
                .Select(p => p.IsIncomming() ? key : "")
                .ToArray())
                .Contains(key);               
        }

        public bool IsResponse(MethodInfo method)
        {
            var key = Guid.NewGuid().ToString();
            return string.Format(Format, method
                .GetParameters()
                .Select(p => p.IsOutgoing() ? key : "")
                .ToArray())
                .Contains(key);
        }
    }
}
