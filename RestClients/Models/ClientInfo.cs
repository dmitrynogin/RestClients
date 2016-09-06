using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestClients.Models
{
    public class ClientInfo<T>
    {
        public ClientInfo()
            : this("API" + Guid.NewGuid().ToString().Replace("-", ""))
        {
        }

        public ClientInfo(string @namespace)
        {
            Type = typeof(T);
            Namespace = @namespace;            
        }

        Type Type { get; }
        public string Namespace { get; }
        public string ClassTypeName => Type.Name.TrimStart('I');
        public string BaseTypeName => typeof(RestClient).TypeName();
        public string InterfaceTypeName => Type.TypeName();
        public string MessageHandlerTypeName => typeof(HttpMessageHandler).TypeName();
        public string ErrorType => Attribute.Error.TypeOf();
        public string BuilderTypeName => typeof(ClientBuilder<T>).TypeName();
        public string UrlTypeName => typeof(Url).TypeName();
        public string HeaderTypeName => typeof(Header).TypeName();
        public string SiteUri => Attribute.Uri;
        public IEnumerable<ActionInfo<T>> Actions => Type
            .GetMethods()
            .Select(m => new ActionInfo<T>(this, m));

        SiteAttribute Attribute => Type.Attribute<SiteAttribute>();
    }
}
