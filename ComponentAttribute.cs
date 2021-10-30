using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ComponentAttribute : Attribute
    {
        public string Name { get; private set; }
        public ComponentAttribute()
        {
        }
        public ComponentAttribute(string name)
        {
            Name = name;
        }
    }
}
