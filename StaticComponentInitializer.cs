using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jack.Managed
{
    public static class StaticComponentInitializer
    {
        /// <summary>
        /// 获取静态组件的初始化程序，没有则返回 null
        /// </summary>
        /// <param name="staticComponent"></param>
        /// <returns></returns>
        public static MethodInfo GetStaticComponentInitializer(Type staticComponent)
        {
            return staticComponent.GetMethod("StaticComponentInitializer", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null);
        }

    }
}
