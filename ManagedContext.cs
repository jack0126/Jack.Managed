using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Jack.Managed.Util;

namespace Jack.Managed
{
    public static class ManagedContext
    {
        private static ManagedContextStub context;
        public static void Initializer<T>()
        {
            var bootType = typeof(T);
            var attr = (AutoManagedComponentsAttribute)bootType.GetCustomAttribute(InternalCache.AutoManagedComponentsAttributeType, false);

            lock (typeof(ManagedContextStub))
            {
                if (context != null)
                {
                    return;
                }

                if (attr == null)
                {
                    throw new ManagedException($"managed components is invalid: {bootType.FullName}");
                }
                context = new ManagedContextStub(attr.Environment);
            }

            try
            {
                IEnumerable<Type> componentTypes = bootType.Assembly.GetTypes();
                var ns = attr.Namespace;
                if (ns != null && !string.IsNullOrEmpty(ns = ns.Trim()))
                {
                    componentTypes = componentTypes.Where(t => t.FullName.StartsWith(ns));
                }

                context.ComponentsScan(componentTypes);
            }
            catch (ManagedException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ManagedException(e.Message, e);
            }

            //组件集加载完成，执行初始化程序
            StaticComponentInitializer.GetStaticComponentInitializer(bootType)?.Invoke(bootType, null);

        }
    }
}
