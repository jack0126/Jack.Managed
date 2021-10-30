using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LinkComponentAttribute : Attribute
    {
        public Type TargetType { get; private set; }
        public string TargetName { get; private set; }
        public bool Inheritable { get; set; }
        public LinkComponentAttribute(Type targetType) : this(targetType, null)
        {
        }
        public LinkComponentAttribute(Type targetType, string targetName)
        {
        }
    }
}
