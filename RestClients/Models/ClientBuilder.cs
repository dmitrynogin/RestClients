using Microsoft.CSharp;
using RazorEngine;
using RestClients.Properties;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;
using RestClients.Models;
using RazorEngine.Configuration;
using RazorEngine.Text;

namespace RestClients.Models
{
    public class ClientBuilder<T> 
    {
        Type Type { get; }
        string Namespace { get; }
        
        public ClientBuilder()
            : this("API" + Guid.NewGuid().ToString().Replace("-", ""))
        {
        }

        public ClientBuilder(string @namespace)
        {
            Type = typeof(T);
            Namespace = @namespace;
        }

        public Type Emit()
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false };
            parameters.ReferencedAssemblies.Add(typeof(Uri).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(HttpMessageHandler).Assembly.Location);
            parameters.ReferencedAssemblies.Add(GetType().Assembly.Location);
            parameters.ReferencedAssemblies.Add(Type.Assembly.Location);

            var results = provider.CompileAssemblyFromSource(parameters, ToString());
            if (results.Errors.Count > 0)
                throw new NotImplementedException(string.Join("\n", from e in results.Errors.OfType<CompilerError>()
                                                                    select e.ErrorText));

            return results.CompiledAssembly.GetType($"{Client.Namespace}.{Client.ClassTypeName}");
        }

        public override string ToString()
        {
            var config = new TemplateServiceConfiguration();
            config.DisableTempFileLocking = true;
            config.EncodedStringFactory = new RawStringFactory();
            config.CachingProvider = new DefaultCachingProvider(t => { });
            var service = RazorEngineService.Create(config);
            return service.RunCompile(Resources.RestClient, "key", null, Client);
        }

        ClientInfo<T> Client { get; } = new ClientInfo<T>();
    }
}
