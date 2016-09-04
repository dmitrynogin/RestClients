using Microsoft.CSharp;
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

namespace RestClients
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

            return results.CompiledAssembly.GetType($"{Namespace}.{Class}");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"using {GetType().Namespace};");            
            sb.AppendLine();

            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    class {Class}: RestClient, {Type.TypeName()}");
            sb.AppendLine("    {");

            sb.AppendLine($"        public {Class}({typeof(HttpMessageHandler).TypeName()} handler)");
            sb.AppendLine($"            : base({Attribute().Error.TypeOf()}, handler)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");

            foreach (var method in Methods)
            {
                sb.AppendLine($"        public System.Threading.Tasks.Task<{method.ReturnType()}> {method.Name}({method.Arguments()})");
                sb.AppendLine("        {");
                sb.AppendLine($"            var url = new UriFormat(\"{Attribute().Uri}\", \"{Attribute(method).Uri}\"{method.Parameters()});");

                if (Attribute(method) is GetAttribute)
                    sb.AppendLine($"            return GetAsync<{method.ReturnType()}>(url);");
                if (Attribute(method) is PostAttribute)
                    sb.AppendLine($"            return PostAsync<{method.ReturnType()}>(url{method.Body()});");
                if (Attribute(method) is PutAttribute)
                    sb.AppendLine($"            return PutAsync<{method.ReturnType()}>(url{method.Body()});");
                if (Attribute(method) is DeleteAttribute)
                    sb.AppendLine($"            return DeleteAsync<{method.ReturnType()}>(url);");

                sb.AppendLine("        }");
            }

            sb.AppendLine($"        public string CSharp {{ get {{  return new ClientBuilder<{Type}>(\"{Namespace}\").ToString(); }} }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        string Class => Type.Name.TrimStart('I');
        IEnumerable<MethodInfo> Methods => Type.GetMethods();
        SiteAttribute Attribute() => Type.Attribute<SiteAttribute>();
        Attributes Attribute(MethodInfo method) => method.Attribute<Attributes>();
    }

    static class Helpers
    {
        public static string ReturnType(this MethodInfo method) => method.ReturnType.GetGenericArguments().First().TypeName();
        public static string Arguments(this MethodInfo method) => string.Join(",", method.GetParameters().Select(p => $@"{p.ParameterType.TypeName()} {p.Name}"));
        public static string Parameters(this MethodInfo method) => 
            string.Join("", method.GetParameters().Where(p => !p.IsDefined(typeof(BodyAttribute))).Select(p => ", " + p.Name));
        public static string Body(this MethodInfo method) =>
            string.Join("", method.GetParameters().Where(p => p.IsDefined(typeof(BodyAttribute))).Select(p => ", " + p.Name));

        public static string TypeName(this Type type) => new CSharpCodeProvider().GetTypeOutput(new CodeTypeReference(type));
        public static string TypeOf(this Type type) => type == null ? "null" : $"typeof({new CSharpCodeProvider().GetTypeOutput(new CodeTypeReference(type))})";
    }
}
