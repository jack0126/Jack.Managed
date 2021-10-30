using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jack.Managed
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class VariableAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Format { get; set; }
        public VariableAttribute(string name)
        {
            Name = name;
        }
    }
}
