using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutowiredAttribute : Attribute
    {
        public string Name { get; private set; }
        public AutowiredAttribute()
        {
        }
        public AutowiredAttribute(string name)
        {
            Name = name;
        }
    }
}
