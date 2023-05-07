namespace Fruityvice.Base
{
    /// <summary>
    /// Version Interface
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public interface IVersion<TPrimaryKey>
    {
        /// <summary>
        /// Gets Version key of the entity.
        /// </summary>
        TPrimaryKey? Version { get; }
    }
}
