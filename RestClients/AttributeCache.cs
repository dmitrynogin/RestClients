using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClients
{
    public static class AttributeCache
    {
        public static T Attribute<T>(this MemberInfo member)
            where T : System.Attribute
        {
            var attribute = member.Attributes<T>().SingleOrDefault();
            if (attribute == null)
                throw new NotImplementedException($"Single instance of {nameof(T)} is required.");

            return attribute;
        }

        public static IEnumerable<T> Attributes<T>(this MemberInfo member)
            where T : System.Attribute => Cache
                .GetOrAdd(member, t => t.GetCustomAttributes(true).OfType<UriAttribute>().ToArray())
                .OfType<T>();

        static ConcurrentDictionary<MemberInfo, System.Attribute[]> Cache { get; } =
            new ConcurrentDictionary<MemberInfo, System.Attribute[]>();
    }
}
