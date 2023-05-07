namespace Fruityvice.Base.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  App Services
    /// </summary>
    public class AppServices : IAppServices
    {
        /// <summary>
        /// Gets or sets the implementation type
        /// </summary>
        public Type ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the service types
        /// </summary>
        public List<Type> ServiceTypes { get; set; }
    }
}
