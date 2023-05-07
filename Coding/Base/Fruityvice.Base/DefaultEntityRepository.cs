namespace Fruityvice.Base
{
    /// <summary>
    /// Base Repository class for entities with long type primary key
    /// </summary>
    /// <typeparam name="TEntity">NHibernate Entity class</typeparam>
    public class DefaultEntityRepository<TEntity> : Base.Repository<TEntity, long>, IDefaultEntityRepository<TEntity> where TEntity : class
    {
    }
}
