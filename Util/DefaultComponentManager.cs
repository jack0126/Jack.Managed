using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed.Util
{
    class DefaultComponentManager : IContextComponentManager
    {
        private readonly Dictionary<string, List<NamedObject>> mapping = new Dictionary<string, List<NamedObject>>(4096);

        public bool Add(Type type, string name, object instance)
        {
            var namedObject = new NamedObject(name ?? string.Empty, instance);
            var types = new List<Type>(type.GetInterfaces());
            var objectType = InternalCache.ObjectType;

            while (type != objectType)
            {
                types.Add(type);
                type = type.BaseType;
            }

            var keys = types.Select(t => t.FullName).Where(n => !n.StartsWith("System."));

            foreach (var key in keys)
            {
                if (mapping.ContainsKey(key))
                {
                    var list = mapping[key];
                    if (list.Find(e => namedObject.name == e.name) == null)
                    {
                        list.Add(namedObject);
                    }
                }
                else
                {
                    mapping.Add(key, new List<NamedObject>(2) { namedObject });
                }
            }
            return true;
        }

        public object Find(Type type, string name)
        {
            var key = type.FullName;
            if (!mapping.ContainsKey(key))
            {
                return null;
            }

            var list = mapping[key];
            if (string.IsNullOrEmpty(name))
            {
                return list[0].instance;
            }
            return list.Find(e => name == e.name)?.instance;
        }

        public void ForEach(Action<string, object> action)
        {
            foreach(var list in mapping.Values)
            {
                list.ForEach(e => action(e.name, e.instance));
            }
        }

        private class NamedObject
        {
            internal readonly string name;
            internal readonly object instance;
            internal NamedObject(string name, object instance)
            {
                this.name = name;
                this.instance = instance;
            }
        }
    }
}
