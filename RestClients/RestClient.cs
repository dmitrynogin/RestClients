using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestClients.Models;
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
            Types.GetOrAdd(typeof(T), t => new ClientBuilder<T>().Emit());

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

        protected HttpClient Client { get; }
        protected Type Error { get; }

        protected async Task<T> SendAsync<T>(HttpMethod method, Uri url, IEnumerable<Header> headers = null, object data = null) =>
            await ParseJson<T>(await SendAsync(method, url, headers, data));

        protected async Task<HttpResponseMessage> SendAsync(HttpMethod method, Uri url, IEnumerable<Header> headers = null, object data = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (headers != null)
                foreach (var header in headers)
                    header.TryWrite(request);
            if (data != null)
                request.Content = new JsonContent(data);

            var response = await Client.SendAsync(request);
            if (headers != null)
                foreach (var header in headers)
                    header.TryRead(response);

            return response.ThrowIfError(Error);
        }

        async Task<T> ParseJson<T>(HttpResponseMessage response) =>
            JsonConvert.DeserializeObject<T>(
                await response.ThrowIfError(Error).Content.ReadAsStringAsync(), Settings);

        class JsonContent : StringContent
        {
            public JsonContent(object data)
                : base(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            {
            }
        }
    }
}