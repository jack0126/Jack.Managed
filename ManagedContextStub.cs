using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Jack.Managed.Util;

namespace Jack.Managed
{
    class ManagedContextStub
    {
        private readonly EnvironmentFile EnvironmentVariables;
        private readonly IContextComponentManager componentManager = new DefaultComponentManager();
        
        internal ManagedContextStub(string environment)
        {
            if (!string.IsNullOrEmpty(environment) && File.Exists(environment))
            {
                EnvironmentVariables = EnvironmentFile.Open(environment, Encoding.UTF8, true);
                CheckBackEnvironment(environment);
            }
        }

        #region check back of environment
        private void CheckBackEnvironment(string environment)
        {
            var env = environment;
            var envNew = env + ".new$_";
            var envOld = env + ".backup$_";
            if (File.Exists(envNew) && !File.Exists(env))
            {
                if (File.Exists(envOld))
                {
                    new FileInfo(envOld).MoveTo(env);
                }
                throw new ManagedEnvironmentException();
            }

            if (EnvironmentVariables.variables.Values.Count > 0)
            {
                EnvironmentVariables.SaveTo(envNew, Encoding.UTF8);
                File.Delete(envOld);
                new FileInfo(env).MoveTo(envOld);
                new FileInfo(envNew).MoveTo(env);
            }
        }
        #endregion

        #region components scanning
        internal void ComponentsScan(IEnumerable<Type> componentTypes)
        {
            var _start = DateTime.Now.Ticks;

            var StaticComponentAttributeType = typeof(StaticComponentAttribute);
            var LinkComponentAttributeType = typeof(LinkComponentAttribute);
            var ComponentAttributeType = typeof(ComponentAttribute);
            var ConfigurationAttributeType = typeof(ConfigurationAttribute);
            
            var staticComponents = componentTypes.Where(type => type.IsDefined(StaticComponentAttributeType, false));
            var linkComponents = new List<Tuple<LinkComponentAttribute, object>>(1024);

            foreach(var type in componentTypes)
            {
                if (type.IsDefined(ComponentAttributeType, false) && !type.IsAbstract)
                {
                    #region find and load Component 
                    var attr = (ComponentAttribute)type.GetCustomAttribute(ComponentAttributeType, false);
                    if (!componentManager.Add(type, attr.Name, Activator.CreateInstance(type)))
                    {
                        throw new ManagedException($"duplicate resource: {type.FullName}.");
                    }
                    #endregion
                }
                else if (type.IsDefined(LinkComponentAttributeType, true) && !type.IsAbstract)
                {
                    #region find and load LinkComponent
                    var attr = (LinkComponentAttribute)type.GetCustomAttribute(LinkComponentAttributeType, true);
                    if (attr.Inheritable || type.IsDefined(LinkComponentAttributeType, false))
                    {
                        linkComponents.Add(new Tuple<LinkComponentAttribute, object>(attr, Activator.CreateInstance(type)));
                    }
                    #endregion
                }
                else if (type.IsDefined(ConfigurationAttributeType, false))
                {
                    #region find and load Configuration
                    LoadConfiguration(type, linkComponents);
                    #endregion
                }
            }
            
            #region injection dependency
            staticComponents.ForEach(sc => Injection(sc));
            componentManager.ForEach((n, c) => Injection(c));
            linkComponents.ForEach(tup => Injection(tup.Item2));
            #endregion

            #region after dependency injection is completed, the notifier is calling
            staticComponents.ForEach(sc => StaticComponentInitializer.GetStaticComponentInitializer(sc)?.Invoke(sc, null));
            componentManager.ForEach((n, c) => (c as IComponentInitializer)?.ComponentInitializer());
            linkComponents.ForEach(tup => (tup.Item2 as IComponentInitializer)?.ComponentInitializer());
            #endregion

            #region inversion of control
            linkComponents.ForEach(tup =>
            {
                if (componentManager.Find(tup.Item1.TargetType, tup.Item1.TargetName) is ILinkableTarget target)
                {
                    target.Linkup(tup.Item2);
                }
            });
            #endregion

            var _end = DateTime.Now.Ticks;
            var _count = 0;
            componentManager.ForEach((n, c) => _count++);
            Console.WriteLine($"Jack.Managed: scan completed, components-count = {_count}, spends-time = {_end - _start}.");
        }
        #endregion

        private void LoadConfiguration(Type type, List<Tuple<LinkComponentAttribute, object>> linkComponents)
        {
            #region inject environment variables into configuration classes
            type.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(prop => prop.IsDefined(InternalCache.VariableAttributeType, false))
                .ForEach(prop => InjectionVariable(type, prop));
            #endregion

            var componentMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.ReturnType != InternalCache.VoidType && 
                (m.IsDefined(InternalCache.ComponentAttributeType, false) || m.IsDefined(InternalCache.LinkComponentAttributeType, false)));

            foreach (var method in componentMethods)
            {
                object component;
                #region call configuration method of Component or LineComponent
                try
                {
                    var parameters = method.GetParameters();
                    var args = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var param = parameters[i];
                        var variableAttr = (VariableAttribute)param.GetCustomAttribute(InternalCache.VariableAttributeType, false);
                        if (variableAttr == null)
                        {
                            throw new ManagedException($"parameter must be VariableAttribute: {param.Name} at {type.FullName}.{method.Name}()");
                        }
                        var variable = GetVariable(variableAttr.Name);
                        if (variable == null)
                        {
                            throw new ManagedException($"variable not found: '{variableAttr.Name}' at {type.FullName}.{method.Name}().");
                        }
                        args[i] = TypeMatchUtils.VariableMatching(param.ParameterType, variable, variableAttr.Format);
                        if (args[i] == null)
                        {
                            throw new ManagedException($"unsupported variable type: '{param.ParameterType}' at {type.FullName}.{method.Name}().");
                        }
                    }
                    component = method.Invoke(type, args);
                }
                catch (Exception e)
                {
                    throw new ManagedException($"configuration error: '{e.Message}' at {type.FullName}.{method.Name}()", e);
                }
                #endregion

                if (component == null)
                {
                    throw new ManagedException($"component can't be null: {type.FullName}.{method.Name}().");
                }

                var componentType = component.GetType();
                if (componentType.IsValueType)
                {
                    throw new ManagedException($"component must be a class object: {type.FullName}.{method.Name}().");
                }

                var attr = (ComponentAttribute)method.GetCustomAttribute(InternalCache.ComponentAttributeType, false);
                if (attr != null)
                {
                    if (!componentManager.Add(componentType, attr.Name, component))
                    {
                        throw new ManagedException($"duplicate resource: at {type.FullName}.{method.Name}().");
                    }
                }
                else
                {
                    var a = (LinkComponentAttribute)method.GetCustomAttribute(InternalCache.LinkComponentAttributeType, false);
                    linkComponents.Add(new Tuple<LinkComponentAttribute, object>(a, component));
                }
            }
        }

        #region dependency injection
        private void Injection(object instance)
        {
            IEnumerable<PropertyInfo> props;
            if (instance is Type type)
            {
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            }
            else
            {
                props = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            
            foreach (var prop in props)
            {
                if (prop.IsDefined(InternalCache.AutowiredAttributeType, false))
                {
                    InjectionResource(instance, prop);
                }
                else if (prop.IsDefined(InternalCache.VariableAttributeType, false))
                {
                    InjectionVariable(instance, prop);
                }
            }
        }

        private void InjectionResource(object instance, PropertyInfo prop)
        {
            var attr = (AutowiredAttribute)prop.GetCustomAttribute(InternalCache.AutowiredAttributeType, false);
            var setter = prop.GetSetMethod();

            if (setter == null)
            {
                var type = (instance is Type) ? (Type)instance : instance.GetType();
                throw new ManagedException($"set method not found: {type.FullName}.{prop.Name}.");
            }

            var component = componentManager.Find(prop.PropertyType, attr.Name);
            if (component == null)
            {
                var type = (instance is Type) ? (Type)instance : instance.GetType();
                throw new ManagedException($"resource not found: {type.FullName}.{prop.Name}.");
            }
            setter.Invoke(instance, BindingFlags.SetProperty, null, new object[] { component }, null);
        }

        private void InjectionVariable(object instance, PropertyInfo prop)
        {
            var attr = (VariableAttribute)prop.GetCustomAttribute(InternalCache.VariableAttributeType, false);
            var setter = prop.GetSetMethod();

            if (setter == null)
            {
                var type = (instance is Type) ? (Type)instance : instance.GetType();
                throw new ManagedException($"set method not found: {type.FullName}.{prop.Name}.");
            }

            var variable = GetVariable(attr.Name);
            if (variable == null)
            {
                var type = (instance is Type) ? (Type)instance : instance.GetType();
                throw new ManagedException($"variable not found: '{attr.Name}' at {type.FullName}.{prop.Name}.");
            }

            var propType = prop.PropertyType;
            object[] args = { TypeMatchUtils.VariableMatching(propType, variable, attr.Format) };

            if (args[0] == null)
            {
                var type = (instance is Type) ? (Type)instance : instance.GetType();
                throw new ManagedException($"unsupported variable type: '{propType.FullName}' at {type.FullName}.{prop.Name}.");
            }
            setter.Invoke(instance, BindingFlags.SetProperty, null, args, null);
        }
        #endregion
        
        private TextVariable GetVariable(string name)
        {
            if (EnvironmentVariables.variables.ContainsKey(name))
            {
                return EnvironmentVariables.variables[name];
            }
            return null;
        }
    }
}
