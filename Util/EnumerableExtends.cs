using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed.Util
{
    public static class EnumerableExtends
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach(var item in enumerable)
            {
                action(item);
            }
        }
    }
}
