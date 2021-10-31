using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class NotComponentAttribute : Attribute
    {
    }
}
