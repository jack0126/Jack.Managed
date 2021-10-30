using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed.Util
{
    interface IContextComponentManager
    {
        bool Add(Type type, string name, object instance);
        object Find(Type type, string name);
        void ForEach(Action<string, object> action);
    }
}
