namespace Fruityvice.Base
{
    using System;
    using System.Linq;

    /// <summary>
    /// ORM Utility functionality
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// Get all mapping types of an assembly from specified type assembly
        /// </summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <returns>Returns all mapping types of an assembly</returns>
        public static System.Collections.Generic.List<Type> GetAllMappings<T>() where T : class
        {
            return System.Reflection.Assembly.GetAssembly(typeof(T)).GetAllMappings();
        }

        /// <summary>
        /// Get all mapping types of an assembly 
        /// </summary>
        /// <param name="assembly">Type of assembly</param>
        /// <returns>Returns all mapping types of an assembly</returns>
        public static System.Collections.Generic.List<Type> GetAllMappings(this System.Reflection.Assembly assembly)
        {
            if (assembly == null)
            {
                throw new NullReferenceException(nameof(assembly));
            }

            try
            {
                return assembly.GetExportedTypes().GetAllMappings();
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                ////The class "Foo" derives from a class "Foo", but "Foo" is defined in another assembly. Here’s a non-exhaustive list of reasons why loading "Foo" might fail:
                ////    The assembly containing "Foo" does not exist on disk.
                ////    The current user does not have permission to load the assembly containing "Foo".
                ////    The assembly containing "Foo" is corrupted and not a valid assembly.

                return e.Types.Where(t => t != null).ToArray().GetAllMappings();
            }
        }

        /// <summary>
        /// Get all mapping types from assemblies
        /// </summary>
        /// <param name="assemblies">Type of assembly</param>
        /// <returns>Returns all mapping types of an assembly</returns>
        public static System.Collections.Generic.List<Type> GetAllMappings(this System.Collections.Generic.List<System.Reflection.Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new NullReferenceException(nameof(assemblies));
            }

            try
            {
                return assemblies.SelectMany(x => x.GetExportedTypes().GetAllMappings()).ToList();
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                ////The class "Foo" derives from a class "Foo", but "Foo" is defined in another assembly. Here’s a non-exhaustive list of reasons why loading "Foo" might fail:
                ////    The assembly containing "Foo" does not exist on disk.
                ////    The current user does not have permission to load the assembly containing "Foo".
                ////    The assembly containing "Foo" is corrupted and not a valid assembly.

                return e.Types.Where(t => t != null).ToArray().GetAllMappings();
            }
        }

        /// <summary>
        /// Get all mapping types of an assembly from system types
        /// </summary>
        /// <param name="types">System types</param>
        /// <returns>Returns all mapping types of an assembly</returns>
        private static System.Collections.Generic.List<Type> GetAllMappings(this System.Type[] types)
        {
            if (types == null)
            {
                throw new NullReferenceException(nameof(types));
            }

            return types.Where(x => x.IsClass && !x.IsAbstract && x.BaseType != null && !string.IsNullOrWhiteSpace(x.BaseType.FullName) &&
                  (x.BaseType.FullName.StartsWith(typeof(NHibernate.Mapping.ByCode.Conformist.ClassMapping<>).FullName) ||
                   x.BaseType.FullName.StartsWith(typeof(NHibernate.Mapping.ByCode.Conformist.JoinedSubclassMapping<>).FullName) ||
                   x.BaseType.FullName.StartsWith(typeof(NHibernate.Mapping.ByCode.Conformist.UnionSubclassMapping<>).FullName) ||
                   x.BaseType.FullName.StartsWith(typeof(NHibernate.Mapping.ByCode.Conformist.SubclassMapping<>).FullName) ||
                   x.BaseType.FullName.StartsWith(typeof(NHibernate.Mapping.ByCode.Conformist.ComponentMapping<>).FullName) ||
                   x.BaseType.FullName.StartsWith(typeof(DefaultEntityClassMap<>).FullName))).ToList();
        }
    }
}
