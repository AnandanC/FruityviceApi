namespace Fruityvice.Base.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  App Services
    /// </summary>
    public interface IAppServices
    {
        /// <summary>
        /// Gets or sets the implementation type
        /// </summary>
        Type ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the service types
        /// </summary>
        public List<Type> ServiceTypes { get; set; }
    }
}