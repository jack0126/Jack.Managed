using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoManagedComponentsAttribute : Attribute
    {
        public string Namespace { get; set; }
        public string Environment { get; set; }
    }
}
