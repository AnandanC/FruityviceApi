namespace Fruityvice.Base
{
    using System;

    /// <summary>
    /// This attribute is used to indicate that declaring method is transactional (atomic).
    /// A method that has this attribute is intercepted, a transaction starts before call the method.
    /// At the end of method call, transaction is committed if there is no exception, otherwise it's rolled back.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute
    {
    }
}