namespace Fruityvice.Base
{
    /// <summary>
    /// Defines interface for base entity type.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public interface IEntity<TPrimaryKey> : IAuditEntity, ISoftDelete, IVersion<TPrimaryKey>
    {
        /// <summary>
        /// Gets Primary key of the entity.
        /// </summary>
        //////[JsonIgnore]
        TPrimaryKey? Id { get; }
    }
}
