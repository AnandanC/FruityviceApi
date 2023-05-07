namespace Fruityvice.Base
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NHibernate.Linq;

    /// <summary>
    /// This interface must be implemented by all repositories to ensure UnitOfWork to work.
    /// Implement by generic version instead of this one.
    /// </summary>
    public interface IRepository
    {
    }

    /// <summary>
    /// This interface is implemented by all repositories to ensure implementation of fixed methods.
    /// </summary>
    /// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : class
    {
        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Returns Entity</returns>
        TEntity Get(TPrimaryKey id);

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="lockMode">Entity lock mode</param>
        /// <returns>Returns Entity</returns>
        TEntity Get(TPrimaryKey id, NHibernate.LockMode lockMode);

        /// <summary>
        /// Gets Entity by Primary Key of Entity
        /// </summary>
        /// <param name="id">Primary Key of Entity</param>
        /// <returns>Returns Entity</returns>
        Task<TEntity> GetAsync(TPrimaryKey id);

        /// <summary>
        /// Used to get a Queryable that is used to retrieve object from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Used to get a Queryable that is used to retrieve object from entire table.
        /// </summary>
        /// <param name="filter">Delegate function to filter entities</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        NHibernate.IQueryOver<TEntity, TEntity> GetAll(System.Linq.Expressions.Expression<Func<TEntity>> filter);

        /// <summary>
        /// Used to get a Queryable that is used to retrieve object from entire table.
        /// </summary>
        /// <param name="filter">Delegate function to filter entities</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalPages">Total Pages</param>
        /// <returns>ICriteria to be used to select entities from database</returns>
        NHibernate.ICriteria GetAll(System.Linq.Expressions.Expression<Func<TEntity>> filter, int pageNumber, int pageSize, out int totalPages);

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        /// <returns>Returns task</returns>
        Task InsertAsync(TEntity entity);

        /// <summary>
        /// Bulk Inserts, Before bulk insert, you must do the cache.use_second_level_cache = false or 
        /// explicitly set the CacheMode to disable interaction with the second-level cache. otherwise
        /// This would fall over with an OutOfMemoryException somewhere around the 50,000 record. 
        /// That's because NHibernate caches all the newly inserted entity instances in the session-level cache.
        /// </summary>
        /// <param name="entities">List of entity</param>
        void BulkInsert(System.Collections.Generic.IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        /// <returns>Returns task</returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Deletes an entity by its id without actually loading it.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="isSoftDelete">It's updates IsDeleted property to "Y" and DeleteDate property to system current date and time </param>
        /// <returns>Returns true if successfully deleted otherwise false</returns>
        bool Delete(TPrimaryKey id, bool isSoftDelete = true);

        /// <summary>
        /// Deletes an entity by its id without actually loading it.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="isSoftDelete">It's updates IsDeleted property to "Y" and DeleteDate property to system current date and time </param>
        /// <returns>Returns task</returns>
        Task DeleteAsync(TPrimaryKey id, bool isSoftDelete = true);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity Object</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity Object</param>
        /// <returns>Returns task</returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Queryable deletes an entity.
        /// Example : session.Query{Product}().Where(x => x.IsDiscontinued == true).Delete();
        /// </summary>
        /// <param name="queryable">Strongly Typed queryable entity</param>
        //void Delete(IQueryable<TEntity> queryable);

        /// <summary>
        /// Add/Update Entity into NHibernate Context
        /// </summary>
        /// <param name="entity">NHibernate Entity</param>
        /// <returns>Returns Added/Updated Entity</returns>
        TEntity SaveOrUpdate(TEntity entity);

        /// <summary>
        /// Add/Update Entity into NHibernate Context
        /// </summary>
        /// <param name="entity">NHibernate Entity</param>
        /// <returns>Returns Added/Updated Entity</returns>
        Task SaveOrUpdateAsync(TEntity entity);

        /// <summary>
        /// Validates entity based on attribute specified.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        /// <returns>Returns true if it is valid otherwise false</returns>
        bool IsValid(TEntity entity);

        /// <summary>
        /// Inserts, updates and deletes query to bulk changes.
        /// </summary>
        /// <param name="queryString">bulk changes query</param>
        /// <returns>Returns NHibernate.IQuery</returns>
        NHibernate.IQuery CreateQuery(string queryString);

        /// <summary>
        /// It is HQL for doing bulk changes: inserts, updates and deletes.
        /// Versioned keyword is NHibernate to do the right thing: update the version on each affected entity, of the entity is versioned.
        /// Use SetParameter, SetParameterList method to parameters
        /// </summary>
        /// <param name="query">An object-oriented representation of a NHibernate query.</param>
        /// <returns>The number of entities inserted or updated or deleted.</returns>
        long ExecuteUpdate(NHibernate.IQuery query);

        /// <summary>
        /// Create a new instance of NHibernate.ISQLQuery for the given SQL query string.
        /// </summary>
        /// <param name="queryString">a query expressed in SQL</param>
        /// <returns>Returns NHibernate.ISQLQuery</returns>
        NHibernate.ISQLQuery CreateSQLQuery(string queryString);

        /// <summary>
        /// It is HQL for doing bulk changes: inserts, updates and deletes.
        /// Versioned keyword is NHibernate to do the right thing: update the version on each affected entity, of the entity is versioned.
        /// Use SetParameter, SetParameterList method to parameters
        /// </summary>
        /// <param name="query">An object-oriented representation of a NHibernate query.</param>
        /// <returns>The number of entities inserted or updated or deleted.</returns>
        long ExecuteUpdate(NHibernate.ISQLQuery query);

        /// <summary>
        /// Create a new Criteria instance, for the given entity name.
        /// </summary>
        /// <returns>Returns NHibernate.ICriteria</returns>
        NHibernate.ICriteria CreateCriteria();

        /// <summary>
        /// Create a new instance of Query for the given collection and filter string
        /// </summary>
        /// <param name="collection">A persistent collection</param>
        /// <param name="queryString">A hibernate query</param>
        /// <returns>Returns a query</returns>
        NHibernate.IQuery CreateFilter(object collection, string queryString);

        /// <summary>
        /// Create a new instance of Query for the given collection and filter string
        /// </summary>
        /// <param name="collection">A persistent collection</param>
        /// <param name="queryString">A hibernate query</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns>Returns a query</returns>
        Task<NHibernate.IQuery> CreateFilterAsync(object collection, string queryString, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Return the identifier of an entity instance cached by the ISession
        /// </summary>
        /// <param name="entity">a persistent instance</param>
        /// <returns>Returns the identifier, Throws an exception if the instance is transient or associated with a different ISession</returns>
        TPrimaryKey GetIdentifier(TEntity entity);

        /// <summary>
        ///     Copy the state of the given object onto the persistent object with the same identifier.
        ///     If there is no persistent instance currently associated with the session, it
        ///     will be loaded. Return the persistent instance. If the given instance is unsaved,
        ///     save a copy of and return it as a newly persistent instance. The given instance
        ///     does not become associated with the session. This operation cascades to associated
        ///     instances if the association is mapped with cascade="merge". The semantics of
        ///     this method are defined by JSR-220.
        /// </summary>
        /// <param name="entity">a detached instance with state to be copied</param>
        /// <returns> an updated persistent instance</returns>
        TEntity Merge(TEntity entity);

        /// <summary>
        /// Enable the named filter for this current session.
        /// </summary>
        /// <param name="filterName">The name of the filter to be enabled.</param>
        /// <returns>The Filter instance representing the enabled filter.</returns>
        NHibernate.IFilter EnableFilter(string filterName);

        /// <summary>
        /// Disable the named filter for the current session.
        /// </summary>
        /// <param name="filterName">The name of the filter to be disabled.</param>
        void DisableFilter(string filterName);

        /// <summary>
        /// Retrieve a currently enabled filter by name.
        /// </summary>
        /// <param name="filterName">The name of the filter to be retrieved.</param>
        /// <returns>The Filter instance representing the enabled filter.</returns>
        NHibernate.IFilter GetEnabledFilter(string filterName);

        /// <summary>
        /// Obtain the definition of a filter by name.
        /// </summary>
        /// <param name="filterName">The name of the filter for which to obtain the definition.</param>
        /// <returns>Returns Filter Definition</returns>
        NHibernate.Engine.FilterDefinition GetFilterDefinition(string filterName);

        /// <summary>
        /// Converted default filter condition to parameters
        /// </summary>
        /// <param name="filter">
        ///     Type definition of Filter. Filter defines the user's view into enabled dynamic
        ///     filters, allowing them to set filter parameter values.
        /// </param>
        /// <returns>Returns true if successfully converted default filter condition to parameters</returns>
        bool EnableFilterWithDefaultFilterCondition(NHibernate.IFilter filter);

        /// <summary>
        /// Sets Result Transformer
        /// </summary>
        /// <param name="query">Query to set result transformer</param>
        /// <param name="resultTransformer">Result transformer </param>
        /// <returns>Returns query</returns>
        NHibernate.IQuery SetResultTransformer(NHibernate.IQuery query, NHibernate.Transform.IResultTransformer resultTransformer);

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="queryable">LINQ queryable</param>
        /// <returns>Returns query string</returns>
        string ToSql(IQueryable queryable);

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="query">NHibernate query</param>
        /// <returns>Returns query string</returns>
        string ToSql(NHibernate.IQuery query);

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="queryOver">NHibernate Query Over</param>
        /// <returns>Returns query string</returns>
        string ToSql(NHibernate.IQueryOver queryOver);

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="criteria">NHibernate Criteria</param>
        /// <returns>Returns query string</returns>
        string ToSql(NHibernate.ICriteria criteria);

        /// <summary>
        /// Re-read the state of the given instance from the underlying database.
        /// Note: It is inadvisable to use this to implement long-running sessions that span many
        ///     business tasks. This method is, however, useful in certain special circumstances.
        ///     For example, Where a database trigger alters the object state upon insert or
        ///     update After executing direct SQL (eg. a mass update) in the same session After
        ///     inserting a Blob or Clob
        /// </summary>
        /// <param name="obj">A persistent instance</param>
        void Refresh(object obj);

        /// <summary>
        /// Re-read the state of the given instance from the underlying database.
        /// Note: It is inadvisable to use this to implement long-running sessions that span many
        ///     business tasks. This method is, however, useful in certain special circumstances.
        ///     For example, Where a database trigger alters the object state upon insert or
        ///     update After executing direct SQL (eg. a mass update) in the same session After
        ///     inserting a Blob or Clob
        /// </summary>
        /// <param name="obj">A persistent instance</param>
        /// <param name="lockMode">the lock mode to use</param>
        void Refresh(object obj, NHibernate.LockMode lockMode);

        /// <summary>
        ///  Re-read the state of the given instance from the underlying database.
        ///  Note : 
        ///     It is inadvisable to use this to implement long-running sessions that span many
        ///     business tasks. This method is, however, useful in certain special circumstances.
        ///     For example, Where a database trigger alters the object state upon insert or
        ///     update After executing direct SQL (eg. a mass update) in the same session After
        ///     inserting a Blob or Clob
        /// </summary>
        /// <param name="obj">A persistent instance</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns>Returns Task</returns>
        Task RefreshAsync(object obj, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        ///  Re-read the state of the given instance from the underlying database.
        ///  Note : 
        ///     It is inadvisable to use this to implement long-running sessions that span many
        ///     business tasks. This method is, however, useful in certain special circumstances.
        ///     For example, Where a database trigger alters the object state upon insert or
        ///     update After executing direct SQL (eg. a mass update) in the same session After
        ///     inserting a Blob or Clob
        /// </summary>
        /// <param name="obj">A persistent instance</param>
        /// <param name="lockMode">the lock mode to use</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns>Returns Task</returns>
        Task RefreshAsync(object obj, NHibernate.LockMode lockMode, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Refreshing Manually Created Entities Does Not Bring Lazy Properties of Base Types.
        /// </summary>
        /// <param name="entity">a persistent instance</param>
        void InitializeLazyProperties(TEntity entity);

        /// <summary>
        /// Log the exception
        /// </summary>
        /// <param name="exception">Exception details</param>
        /// <returns>Returns true if success</returns>
        bool LogException(Exception exception);
    }
}
