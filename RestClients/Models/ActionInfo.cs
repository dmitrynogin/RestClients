using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClients.Models
{
    public class ActionInfo<T>
    {
        public ActionInfo(ClientInfo<T> client, MethodInfo method)
        {
            Client = client;
            Method = method;
            Validate();            
        }

        void Validate()
        {
            if (!typeof(Task).IsAssignableFrom(Method.ReturnType))
                throw new NotImplementedException($"Async API action {Method.Name} return type should be Task or Task<T>.");

            var bodies = Method.GetParameters().Where(p => p.IsDefined(typeof(BodyAttribute))).Count();
            if (bodies > 0 && !Action.AllowPayload)
                throw new NotImplementedException($"API action {Method.Name} for {Action.Verb.ToUpperInvariant()} HTTP verb can not define payload.");
            if (bodies > 1)
                throw new NotImplementedException($"API action {Method.Name} for {Action.Verb.ToUpperInvariant()} HTTP verb redefines payload twice or more.");
            if(Method.GetParameters().FirstOrDefault(p => p.IsDefined(typeof(BodyAttribute)))?.ParameterType?.IsByRef == true)
                throw new NotImplementedException($"API action {Method.Name} for {Action.Verb.ToUpperInvariant()} HTTP verb can not be 'out' or 'ref' parameter.");
        }

        public string ReturnTypeName => Method.ReturnType.TypeName();
        public string Name => Method.Name;
        public string ActionUri => Action.Uri;

        public string Arguments => string.Join(", ", Method
            .GetParameters()
            .Select(p => $"{p.Decorator()}{p.ParameterType.TypeName()} {p.Name}"));

        public IEnumerable<string> Initializations => Method
            .GetParameters()
            .Where(p => p.IsOut && p.ParameterType.IsByRef)
            .Select(p => $"{p.Name} = default({p.ParameterType.TypeName()});");

        public string RequestHeaderTypeName => typeof(RequestHeader).TypeName();
        public string ResponseHeaderTypeName => typeof(ResponseHeader).TypeName();
        public IEnumerable<string> RequestHeaders => Headers.Where(h => h.IsRequest(Method)).Select(h => h.Format);
        public IEnumerable<string> ResponseHeaders => Headers.Where(h => h.IsResponse(Method)).Select(h => h.Format);
        public string[] HeaderAssignments => Method
            .GetParameters()
            .Select((p, i) => new { p, i })
            .Where(x => x.p.IsOutgoing())
            .Select(x => $"header.TryGet({x.i}, ref {x.p.Name});")
            .ToArray();

        public string UrlParameters => string.Join("", Method
            .GetParameters()            
            .Select(p => ", " + p.Name));

        public string Call => $"SendAsync{CallGenericParameter}({CallParameters})";
        public bool HasResult => Method.ReturnType.IsGenericType;

        ClientInfo<T> Client { get; } // ? Do we need it here
        MethodInfo Method { get; }
        ActionAttribute Action => Method.Attribute<ActionAttribute>();
        IEnumerable<HeaderAttribute> Headers => Method.Attributes<HeaderAttribute>();        
        string ResultTypeName => Method.ReturnType.GenericTypeArguments.First().TypeName();
        string CallGenericParameter => HasResult ? $"<{ResultTypeName}>" : "";
        string CallParameters => $"HttpMethod.{Action.Verb}, url, headers" + (BodyParameter == null ? "" : $", {BodyParameter}");
        string BodyParameter => Method
            .GetParameters()
            .FirstOrDefault(p => p.IsDefined(typeof(BodyAttribute)))
            ?.Name;
    }
}
