using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestClients
{
    public abstract class RestClient 
    {
        static ConcurrentDictionary<Type, Type> Types { get; } = new ConcurrentDictionary<Type, Type>();

        public static T Create<T>() =>
            Create<T>(null);
        
        public static T Create<T>(HttpMessageHandler handler) =>
            (T)Activator.CreateInstance(Emit<T>(), handler);

        public static Type Emit<T>() =>
            Types.GetOrAdd(typeof(T), new ClientBuilder<T>().Emit());

        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new[] { new StringEnumConverter() }
        };

        protected RestClient(Type error, HttpMessageHandler handler)
        {
            Error = error;
            Client = handler == null ? new HttpClient() : new HttpClient(handler);
            Client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async Task<T> GetAsync<T>(Uri url) =>
            await ParseJson<T>(await Client.GetAsync(url));

        protected async Task<T> PostAsync<T>(Uri url, object data = null) =>
            await ParseJson<T>(await Client.PostAsync(url, JsonContent.From(data)));

        protected HttpClient Client { get; }
        protected Type Error { get; }

        async Task<T> ParseJson<T>(HttpResponseMessage response) =>
            JsonConvert.DeserializeObject<T>(
                await response.ThrowIfError(Error).Content.ReadAsStringAsync(), Settings);

        class JsonContent : StringContent
        {
            public static JsonContent From(object data) =>
                data == null ? null : new JsonContent(data);

            public JsonContent(object data)
                : base(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            {
            }
        }
    }
}
