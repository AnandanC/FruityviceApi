namespace Fruityvice.Base
{
    using System;

    /// <summary>
    /// Default Entity Class Mapping
    /// </summary>
    /// <typeparam name="TEntity">Class of entity</typeparam>
    public abstract class DefaultEntityClassMap<TEntity> : NHibernate.Mapping.ByCode.Conformist.ClassMapping<TEntity> where TEntity : DefaultEntity
    {
        #region Constructors/Finalizer
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEntityClassMap{TEntity}"/> class.
        /// </summary>
        /// <param name="tableName">Name of database table</param>
        /// <param name="idColumnName">Name of database table's primary id column</param>
        public DefaultEntityClassMap(string tableName, string idColumnName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (string.IsNullOrEmpty(idColumnName))
            {
                throw new ArgumentNullException(nameof(idColumnName));
            }

            this.Table(tableName);
            this.Id(x => x.Id, map => { map.Column(idColumnName); map.Generator(NHibernate.Mapping.ByCode.Generators.Increment); });

            if (this as IVersion<long> != null)
            {
                this.Version(x => x.Version, x => x.Type(NHibernate.NHibernateUtil.Int64));
                this.OptimisticLock(NHibernate.Mapping.ByCode.OptimisticLockMode.Version);
            }

            if (this as ISoftDelete != null)
            {
                this.Property(x => x.IsDeleted, map => { map.Column("IsDeleted"); map.Scale(1); map.Type(new NHibernate.Type.YesNoType()); });
            }

            if (this as IAuditEntity != null)
            {
                this.Property(x => x.CreatedBy, map => { map.Column("EnteredBy"); map.Scale(64); });
                this.Property(x => x.CreatedDate, map => { map.Column("EnteredDate"); });
                this.Property(x => x.LastUpdatedBy, map => { map.Column("UpdatedBy"); map.Scale(64); });
                this.Property(x => x.LastUpdatedDate, map => { map.Column("UpdatedDate"); });
            }
        }

        #endregion
    }
}
