using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClients.Models
{
    static class CSharp
    {
        public static string TypeName(this Type type) => 
            new CSharpCodeProvider().GetTypeOutput(new CodeTypeReference(type)).TrimEnd('&');

        public static string TypeOf(this Type type) => 
            type == null ? "null" : $"typeof({new CSharpCodeProvider().GetTypeOutput(new CodeTypeReference(type))})";

        public static string Decorator(this ParameterInfo parameter) =>
            parameter.ParameterType.IsByRef ?
                (parameter.IsOut ? "out " : "ref ") : 
                "";

        public static bool IsIncomming(this ParameterInfo parameter) =>
            parameter.ParameterType.IsByRef ?
                (parameter.IsOut ? false : true) :
                true;

        public static bool IsOutgoing(this ParameterInfo parameter) =>
            parameter.ParameterType.IsByRef ?
                (parameter.IsOut ? true : true) :
                false;
    }
}
