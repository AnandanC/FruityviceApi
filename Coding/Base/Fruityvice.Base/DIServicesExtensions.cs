namespace Fruityvice.Base.DependencyInjection
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///  Dependency Injection Services Extensions
    /// </summary>
    public static class DIServicesExtensions
    {
        /// <summary>
        /// Get All Service based on type
        /// </summary>
        /// <typeparam name="T">T is any class</typeparam>
        /// <returns>Returns list of services</returns>
        public static List<AppServices> GetAllService<T>() where T : class
        {
            return GetAllService(new List<System.Reflection.Assembly>() { System.Reflection.Assembly.GetAssembly(typeof(T)) });
        }

        /// <summary>
        /// Get All Service based on type
        /// </summary>
        /// <param name="implementationTypes">Implementation Types are system types</param>
        /// <returns>Returns list of services</returns>
        /// <exception cref="System.NullReferenceException">Throws NullReferenceException if Implementation Types is null</exception>
        public static List<AppServices> GetAllServices(List<System.Type> implementationTypes)
        {
            if (implementationTypes == null)
            {
                throw new System.NullReferenceException(nameof(implementationTypes));
            }

            List<AppServices> result = new List<AppServices>();
            foreach (System.Type implementationType in implementationTypes)
            {
                var serviceTypes = implementationType.GetInterfaces().ToList();
                serviceTypes = serviceTypes.Where(x => x.Name.EndsWith(implementationType.Name) && x.Namespace == implementationType.Namespace).ToList();

                if (serviceTypes.Count() > 0)
                {
                    result.Add(new AppServices()
                    {
                        ImplementationType = implementationType,
                        ServiceTypes = new List<System.Type>(serviceTypes)
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Get All Service based on type
        /// </summary>
        /// <param name="assemblies">list of assembly</param>
        /// <returns>Returns list of services</returns>
        /// <exception cref="System.NullReferenceException">Throws NullReferenceException if Implementation Types is null</exception>
        public static List<AppServices> GetAllService(List<System.Reflection.Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new System.NullReferenceException(nameof(assemblies));
            }

            try
            {
                List<System.Type> implementationTypes = assemblies.SelectMany(x => x.GetExportedTypes()).Where(x => x.IsClass && !x.IsAbstract && x.BaseType != null && !string.IsNullOrWhiteSpace(x.BaseType.FullName)).ToList();

                return GetAllServices(implementationTypes);
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                ////The class "Foo" derives from a class "Foo", but "Foo" is defined in another assembly. Here’s a non-exhaustive list of reasons why loading "Foo" might fail:
                ////    The assembly containing "Foo" does not exist on disk.
                ////    The current user does not have permission to load the assembly containing "Foo".
                ////    The assembly containing "Foo" is corrupted and not a valid assembly.

                return GetAllServices(e.Types.Where(t => t != null).ToList());
            }
        }
    }
}
