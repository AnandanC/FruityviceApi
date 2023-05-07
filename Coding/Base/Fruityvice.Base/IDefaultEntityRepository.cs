namespace Fruityvice.Base
{
    /// <summary>
    /// Entity Repository Interface for entities with long type primary key
    /// </summary>
    /// <typeparam name="TEntity">NHibernate Entity class</typeparam>
    public interface IDefaultEntityRepository<TEntity> : Base.IRepository<TEntity, long> where TEntity : class
    {
    }
}
