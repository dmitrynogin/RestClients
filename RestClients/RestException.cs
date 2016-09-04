using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestClients
{
    public class RestException : Exception
    {
        public RestException(HttpResponseMessage response)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; }
        public override string ToString() => 
            Response.Content.ReadAsStringAsync().Result;
    }

    public class RestException<T> : RestException
    {
        public RestException(HttpResponseMessage response)
            : base(response)
        {
        }

        public T Error
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(ToString());
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }

    static class ResponseGuard
    {
        public static HttpResponseMessage ThrowIfError(this HttpResponseMessage response, Type error)
        {
            if (response.IsSuccessStatusCode)
                return response;

            if (error == null)
                throw new RestException(response);

            var type = typeof(RestException<>).MakeGenericType(error);
            throw (RestException)Activator.CreateInstance(type, response);
        }
    }
}
