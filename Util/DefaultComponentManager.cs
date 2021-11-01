using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed.Util
{
    class DefaultComponentManager : IContextComponentManager
    {
        private readonly Dictionary<string, List<NamedObject>> mapping = new Dictionary<string, List<NamedObject>>(8192);
        private readonly List<NamedObject> components = new List<NamedObject>(1024);
        private readonly List<string> keysCache = new List<string>(32);

        public bool Add(Type type, string name, object instance)
        {
            if (Find(type, name) != null)
            {
                return false;
            }

            var keys = keysCache;
            keys.Clear();
            keys.Add(type.FullName);
            
            keys.AddRange(type.GetInterfaces().Select(e => e.FullName).Where(n => !n.StartsWith("System.")));

            var baseType = type.BaseType;
            var objectType = InternalCache.ObjectType;

            while (baseType != objectType)
            {
                var fullName = baseType.FullName;
                if (fullName.StartsWith("System."))
                {
                    break;
                }
                keys.Add(fullName);
                baseType = baseType.BaseType;
            }

            var namedObject = new NamedObject(name ?? string.Empty, instance);
            components.Add(namedObject);

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
                    mapping.Add(key, new List<NamedObject>(1) { namedObject });
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
            name = name ?? string.Empty;
            return list.Find(e => name == e.name)?.instance;
        }

        public void ForEach(Action<string, object> action)
        {
            foreach(var item in components)
            {
                action(item.name, item.instance);
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
