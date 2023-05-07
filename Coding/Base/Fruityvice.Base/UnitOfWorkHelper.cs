namespace Fruityvice.Base
{
    using System;

    /// <summary>
    /// Used to find the Repository class attribute and Unit of work attributes 
    /// </summary>
    public static class UnitOfWorkHelper
    {
        /// <summary>
        /// Find weather it is repository method or not.
        /// </summary>
        /// <param name="methodInfo">Method information</param>
        /// <returns>Returns true if it is repository method otherwise false.</returns>
        public static bool IsRepositoryMethod(System.Reflection.MethodInfo methodInfo)
        {
            return IsRepositoryClass(methodInfo.DeclaringType);
        }

        /// <summary>
        /// Find weather it is repository class or not.
        /// </summary>
        /// <param name="type">Type of Repository Class</param>
        /// <returns>>Returns true if it is repository class otherwise false. </returns>
        public static bool IsRepositoryClass(Type type)
        {
            return typeof(IRepository).IsAssignableFrom(type);
        }

        /// <summary>
        /// Find weather it has Unit of work attribute.
        /// </summary>
        /// <param name="methodInfo">Method information</param>
        /// <returns>Returns true if it has Unit of work attribute otherwise false.</returns>
        public static bool HasUnitOfWorkAttribute(System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
        }
    }
}