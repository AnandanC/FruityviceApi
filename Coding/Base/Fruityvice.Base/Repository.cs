namespace Fruityvice.Base
{
    using global::NHibernate;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for all repositories those uses NHibernate.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        #region local Contstants - CONSTANTS
        /// <summary>
        /// NHibernate Session Closed Message
        /// </summary>
        private const string NHSessionClosedMessage = "NHibernate Session is closed!";
        #endregion

        #region local variables - Declaration Order: public/protected/private
        ////All variables must be private

        /// <summary>
        /// Entity Name Retrieval Type
        /// </summary>
        private EntityNameMode mode = EntityNameMode.Default;

        #endregion

        #region protected properties/enum
        #endregion

        #region private properties/enum
        /// <summary>
        /// Gets the NHibernate session object to perform database operations.
        /// </summary>
        protected ISession Session { get; set; }
        #endregion

        #region public methods

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Returns Entity</returns>
        public TEntity Get(TPrimaryKey id)
        {
            return this.Session.Get<TEntity>(id);
        }

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="lockMode">Entity lock mode</param>
        /// <returns>Returns Entity</returns>
        public TEntity Get(TPrimaryKey id, LockMode lockMode)
        {
            return this.Session.Get<TEntity>(id, lockMode);
        }

        /// <summary>
        /// Gets Entity by Primary Key of Entity
        /// </summary>
        /// <param name="id">Primary Key of Entity</param>
        /// <returns>Returns Entity</returns>
        public async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return (TEntity)await this.Session.GetAsync<TEntity>(id);
        }

        /// <summary>
        /// Used to get a Queryable that is used to retrieve object from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public IQueryable<TEntity> GetAll()
        {
            return this.Session.Query<TEntity>();
        }

        /// <summary>
        /// Used to get a Queryable that is used to retrieve object from entire table.
        /// </summary>
        /// <param name="filter">Delegate function to filter entities</param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public IQueryOver<TEntity, TEntity> GetAll(System.Linq.Expressions.Expression<Func<TEntity>> filter)
        {
            return filter != null ? this.Session.QueryOver<TEntity>(filter) : this.Session.QueryOver<TEntity>();
        }

        /// <summary>
        /// Used to get a Queryable that is used to retrieve object from entire table.
        /// </summary>
        /// <param name="filter">Delegate function to filter entities</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalPages">Total Pages</param>
        /// <returns>ICriteria to be used to select entities from database</returns>
        public ICriteria GetAll(System.Linq.Expressions.Expression<Func<TEntity>> filter, int pageNumber, int pageSize, out int totalPages)
        {
            IQueryOver<TEntity, TEntity> result = this.GetAll(filter);
            totalPages = Convert.ToInt32(Math.Ceiling(result.RowCount() / (double)pageSize));

            return this.Session.CreateCriteria(typeof(TEntity))
                .SetFirstResult((pageNumber - 1) * pageSize)
                .SetMaxResults(pageSize);
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        public void Insert(TEntity entity)
        {
            this.Session.Save(entity);
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        /// <returns>Returns task</returns>
        public Task InsertAsync(TEntity entity)
        {
            return this.Session.SaveAsync(entity);
        }

        /// <summary>
        /// Bulk Inserts, Before bulk insert, you must do the cache.use_second_level_cache = false or 
        /// explicitly set the CacheMode to disable interaction with the second-level cache. otherwise
        /// This would fall over with an OutOfMemoryException somewhere around the 50,000 record. 
        /// That's because NHibernate caches all the newly inserted entity instances in the session-level cache.
        /// </summary>
        /// <param name="entities">List of entity</param>
        public void BulkInsert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                return;
            }

            CacheMode cacheMode = this.Session.CacheMode;
            this.Session.CacheMode = CacheMode.Ignore; ////We can use CacheMode.IGNORE to stop interaction between the session and second-level cache.

            int batchSize = this.Session.GetSessionImplementation().Batcher.BatchSize;
            batchSize = batchSize <= 0 ? 1 : batchSize;

            int i = 0;
            foreach (TEntity entity in entities)
            {
                this.Insert(entity);
                if (i % batchSize == 0)
                {
                    // flush a batch of inserts and release memory:
                    this.Session.Flush();
                    this.Session.Clear();
                }

                i++;
            }

            this.Session.CacheMode = cacheMode;
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        public void Update(TEntity entity)
        {
            this.Session.Update(entity);
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        /// <returns>Returns task</returns>
        public Task UpdateAsync(TEntity entity)
        {
            return this.Session.UpdateAsync(entity);
        }

        /// <summary>
        /// Deletes an entity by its id without actually loading it.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="isSoftDelete">It's updates IsDeleted property to "Y" and DeleteDate property to system current date and time </param>
        /// <returns>Returns true if successfully deleted otherwise false</returns>
        public bool Delete(TPrimaryKey id, bool isSoftDelete = true)
        {
            if (isSoftDelete)
            {
                this.Session.Delete(this.Get(id));
                return true;
            }

            var metadata = this.Session.SessionFactory.GetClassMetadata(typeof(TEntity));
            var hql = string.Format("delete {0} where {1} = :{2}", metadata.EntityName, metadata.IdentifierPropertyName, metadata.IdentifierPropertyName);
            var results = this.Session.CreateQuery(hql).SetParameter(metadata.IdentifierPropertyName, id).ExecuteUpdate();

            return results == 1;
        }

        /// <summary>
        /// Deletes an entity by its id without actually loading it.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="isSoftDelete">It's updates IsDeleted property to "Y" and DeleteDate property to system current date and time </param>
        /// <returns>Returns task</returns>
        public Task DeleteAsync(TPrimaryKey id, bool isSoftDelete = true)
        {
            ////return this.Session.DeleteAsync(this.Get(id));
            return Task.Run(() => this.Delete(id, isSoftDelete));
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity Object</param>
        public void Delete(TEntity entity)
        {
            this.Session.Delete(entity);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity Object</param>
        /// <returns>Returns task</returns>
        public Task DeleteAsync(TEntity entity)
        {
            return this.Session.DeleteAsync(entity);
        }

        /// <summary>
        /// Add/Update Entity into NHibernate Context
        /// </summary>
        /// <param name="entity">NHibernate Entity</param>
        /// <returns>Returns Added/Updated Entity</returns>
        public TEntity SaveOrUpdate(TEntity entity)
        {
            this.Session.SaveOrUpdate(this.GetEntityName(), entity);
            return entity;
        }

        /// <summary>
        /// Add/Update Entity into NHibernate Context
        /// </summary>
        /// <param name="entity">NHibernate Entity</param>
        /// <returns>Returns Added/Updated Entity</returns>
        public Task SaveOrUpdateAsync(TEntity entity)
        {
            return this.Session.SaveOrUpdateAsync(entity);
        }

        /// <summary>
        /// Validates entity based on attribute specified.
        /// </summary>
        /// <param name="entity">Mapped Entity</param>
        /// <returns>Returns true if it is valid otherwise false</returns>
        public bool IsValid(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            ////JSON Schema validation goes here.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts, updates and deletes query to bulk changes.
        /// </summary>
        /// <param name="queryString">bulk changes query</param>
        /// <returns>Returns NHibernate.IQuery</returns>
        public virtual IQuery CreateQuery(string queryString)
        {
            return this.Session.CreateQuery(queryString);
        }

        /// <summary>
        /// It is HQL for doing bulk changes: inserts, updates and deletes.
        /// Versioned keyword is NHibernate to do the right thing: update the version on each affected entity, of the entity is versioned.
        /// Use SetParameter, SetParameterList method to parameters
        /// </summary>
        /// <param name="query">An object-oriented representation of a NHibernate query.</param>
        /// <returns>The number of entities inserted or updated or deleted.</returns>
        public virtual long ExecuteUpdate(IQuery query)
        {
            return query.ExecuteUpdate();
        }

        /// <summary>
        /// Create a new instance of NHibernate.ISQLQuery for the given SQL query string.
        /// </summary>
        /// <param name="queryString">a query expressed in SQL</param>
        /// <returns>Returns NHibernate.ISQLQuery</returns>
        public virtual ISQLQuery CreateSQLQuery(string queryString)
        {
            return this.Session.CreateSQLQuery(queryString);
        }

        /// <summary>
        /// It is HQL for doing bulk changes: inserts, updates and deletes.
        /// Versioned keyword is NHibernate to do the right thing: update the version on each affected entity, of the entity is versioned.
        /// Use SetParameter, SetParameterList method to parameters
        /// </summary>
        /// <param name="query">An object-oriented representation of a NHibernate query.</param>
        /// <returns>The number of entities inserted or updated or deleted.</returns>
        public virtual long ExecuteUpdate(ISQLQuery query)
        {
            return query.ExecuteUpdate();
        }

        /// <summary>
        /// Create a new Criteria instance, for the given entity name.
        /// </summary>
        /// <returns>Returns NHibernate.ICriteria</returns>
        public NHibernate.ICriteria CreateCriteria()
        {
            return this.Session.CreateCriteria(this.GetEntityName());
        }

        /// <summary>
        /// Create a new instance of Query for the given collection and filter string.
        /// A nice mechanism to filter and order a collection without actually loading all of its items
        /// </summary>
        /// <param name="collection">A persistent collection</param>
        /// <param name="queryString">A hibernate query</param>
        /// <returns>Returns a query</returns>
        public NHibernate.IQuery CreateFilter(object collection, string queryString)
        {
            return this.Session.CreateFilter(collection, queryString);
        }

        /// <summary>
        /// Create a new instance of Query for the given collection and filter string
        /// </summary>
        /// <param name="collection">A persistent collection</param>
        /// <param name="queryString">A hibernate query</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns>Returns a query</returns>
        public Task<NHibernate.IQuery> CreateFilterAsync(object collection, string queryString, System.Threading.CancellationToken cancellationToken)
        {
            return this.Session.CreateFilterAsync(collection, queryString, cancellationToken);
        }

        /// <summary>
        /// Return the identifier of an entity instance cached by the ISession
        /// </summary>
        /// <param name="entity">a persistent instance</param>
        /// <returns>Returns the identifier, Throws an exception if the instance is transient or associated with a different ISession</returns>
        public TPrimaryKey GetIdentifier(TEntity entity)
        {
            return (TPrimaryKey)this.Session.GetIdentifier(entity as Base.Entity<TPrimaryKey>);
        }

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
        public TEntity Merge(TEntity entity)
        {
            return this.Session.Merge<TEntity>(this.GetEntityName(), entity);
        }

        /// <summary>
        /// Enable the named filter for this current session.
        /// </summary>
        /// <param name="filterName">The name of the filter to be enabled.</param>
        /// <returns>The Filter instance representing the enabled filter.</returns>
        public IFilter EnableFilter(string filterName)
        {
            return this.Session.EnableFilter(filterName);
        }

        /// <summary>
        /// Disable the named filter for the current session.
        /// </summary>
        /// <param name="filterName">The name of the filter to be disabled.</param>
        public void DisableFilter(string filterName)
        {
            this.Session.DisableFilter(filterName);
        }

        /// <summary>
        /// Retrieve a currently enabled filter by name.
        /// </summary>
        /// <param name="filterName">The name of the filter to be retrieved.</param>
        /// <returns>The Filter instance representing the enabled filter.</returns>
        public IFilter GetEnabledFilter(string filterName)
        {
            return this.Session.GetEnabledFilter(filterName);
        }

        /// <summary>
        /// Obtain the definition of a filter by name.
        /// </summary>
        /// <param name="filterName">The name of the filter for which to obtain the definition.</param>
        /// <returns>Returns Filter Definition</returns>
        public NHibernate.Engine.FilterDefinition GetFilterDefinition(string filterName)
        {
            return this.Session.SessionFactory.GetFilterDefinition(filterName);
        }

        /// <summary>
        /// Converted default filter condition to parameters
        /// </summary>
        /// <param name="filter">
        ///     Type definition of Filter. Filter defines the user's view into enabled dynamic
        ///     filters, allowing them to set filter parameter values.
        /// </param>
        /// <returns>Returns true if successfully converted default filter condition to parameters</returns>
        public bool EnableFilterWithDefaultFilterCondition(IFilter filter)
        {
            if (filter != null && filter.FilterDefinition != null)
            {
                NHibernate.Engine.FilterDefinition filterDefinition = filter.FilterDefinition;
                ICollection<string> values = filterDefinition.DefaultFilterCondition.Split(new string[] { "," }, StringSplitOptions.None).ToList();

                foreach (var parameterType in filterDefinition.ParameterTypes)
                {
                    if (parameterType.Value.GetType().FullName == typeof(NHibernate.Type.StringType).FullName)
                    {
                        filter.SetParameterList<string>(parameterType.Key, values);
                    }
                    else if (parameterType.Value.GetType().FullName == typeof(NHibernate.Type.DateType).FullName)
                    {
                        string dateFormat = "yyyy/MMM/dd";
                        DateTime dt;
                        if (System.DateTime.TryParseExact(string.Join(string.Empty, values), dateFormat, null, System.Globalization.DateTimeStyles.None, out dt))
                        {
                            ICollection<DateTime> dateTimeValues = new List<DateTime>() { dt };
                            filter.SetParameterList<DateTime>(parameterType.Key, dateTimeValues);
                        }
                    }
                }

                try
                {
                    filter.Validate();
                    return ((NHibernate.Impl.FilterImpl)filter).Parameters.Count > 0;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return true;
        }

        /// <summary>
        /// NHibernate Transformers:
        ///     1.AliasToBeanResultTransformer
        ///     2.AliasToBeanConstructorResultTransformer
        ///     3.AliasToEntityMapResultTransformer
        ///     4.AliasedTupleSubsetResultTransformer
        ///     5.CacheableResultTransformer
        ///     6.DistinctRootEntityResultTransformer(Default LINQ Transformers)
        ///     7.PassThroughResultTransformer
        ///     8.RootEntityResultTransformer
        ///     9.ToListResultTransformer
        /// Customized Result Transformer
        ///     1.ExpressionsResultTransformer - It's allows us to select which indexes, in the database record, map to which properties in some entity. 
        /// </summary>
        /// <param name="query">Query to set result transformer</param>
        /// <param name="resultTransformer">Result transformer </param>
        /// <returns>Returns query</returns>
        public IQuery SetResultTransformer(IQuery query, NHibernate.Transform.IResultTransformer resultTransformer)
        {
            return query.SetResultTransformer(resultTransformer);
        }

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="queryable">LINQ queryable</param>
        /// <returns>Returns query string</returns>
        public string ToSql(IQueryable queryable)
        {
            ////var sessionProperty = typeof(NHibernate.Linq.DefaultQueryProvider).GetProperty("Session", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ////var session = sessionProperty.GetValue(queryable.Provider, null) as ISession;
            var sessionImpl = this.Session.GetSessionImplementation();
            var factory = sessionImpl.Factory;
            var linqExpression = new NHibernate.Linq.NhLinqExpression(queryable.Expression, factory);
            var translatorFactory = new NHibernate.Hql.Ast.ANTLR.ASTQueryTranslatorFactory();
            var translator = translatorFactory.CreateQueryTranslators(linqExpression, null, false, sessionImpl.EnabledFilters, factory).First();

            ////in case you want the parameters as well
            var parameters = linqExpression.ParameterValuesByName.ToDictionary(x => x.Key, x => x.Value);

            ////var parameteFormat = "@p16 = NULL[Type: String(0:0:0)]";
            var parameteFormat = "@{0} = {1} [Type: {2} ({3})]";
            var parameterSql = string.Empty;
            foreach (var kvp in parameters)
            {
                if (!string.IsNullOrWhiteSpace(parameterSql))
                {
                    parameterSql += ", ";
                }

                dynamic value = kvp.Value.Item1;
                string valueType = value.GetType().Name;
                var defaultValueOfType = string.Empty; //// value.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(value.GetType()).Invoke(this, null);

                parameterSql += string.Format(parameteFormat, kvp.Key, Convert.ToString(value), valueType, defaultValueOfType);
            }

            return translator.SQLString + "\r\n" + parameterSql;
        }

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="query">LINQ queryable</param>
        /// <returns>Returns query string</returns>
        public string ToSql(IQuery query)
        {
            return query.QueryString;
        }

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="queryOver">NHibernate Query Over</param>
        /// <returns>Returns query string</returns>
        public string ToSql(IQueryOver queryOver)
        {
            var criteria = queryOver.UnderlyingCriteria;
            return criteria.ToString();
        }

        /// <summary>
        /// We have a look at the generated SQL before it is actually executed.
        /// </summary>
        /// <param name="criteria">NHibernate criteria</param>
        /// <returns>Returns query string</returns>
        public string ToSql(ICriteria criteria)
        {
            var criteriaImpl = criteria as NHibernate.Impl.CriteriaImpl;
            var sessionImpl = criteriaImpl.Session;
            var factory = sessionImpl.Factory;
            var implementors = factory.GetImplementors(criteriaImpl.EntityOrClassName);
            var loader = new NHibernate.Loader.Criteria.CriteriaLoader(factory.GetEntityPersister(implementors[0]) as NHibernate.Persister.Entity.IOuterJoinLoadable, factory, criteriaImpl, implementors[0], sessionImpl.EnabledFilters);

            return loader.SqlString.ToString();
        }

        /// <summary>
        /// Re-read the state of the given instance from the underlying database.
        /// Note: It is inadvisable to use this to implement long-running sessions that span many
        ///     business tasks. This method is, however, useful in certain special circumstances.
        ///     For example, Where a database trigger alters the object state upon insert or
        ///     update After executing direct SQL (eg. a mass update) in the same session After
        ///     inserting a Blob or Clob
        /// </summary>
        /// <param name="obj">A persistent instance</param>
        public void Refresh(object obj)
        {
            this.Session.Refresh(obj);
            this.Session.Connection.CreateCommand().CreateParameter();
        }

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
        public void Refresh(object obj, LockMode lockMode)
        {
            this.Session.Refresh(obj);
        }

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
        public Task RefreshAsync(object obj, System.Threading.CancellationToken cancellationToken)
        {
            return this.Session.RefreshAsync(obj, cancellationToken);
        }

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
        public Task RefreshAsync(object obj, LockMode lockMode, System.Threading.CancellationToken cancellationToken)
        {
            return this.Session.RefreshAsync(obj, lockMode, cancellationToken);
        }

        /// <summary>
        /// Refreshing Manually Created Entities Does Not Bring Lazy Properties of Base Types.
        /// </summary>
        /// <param name="entity">a persistent instance</param>
        public void InitializeLazyProperties(TEntity entity)
        {
            bool useProxyWhenPossible = true;

            var metadata = this.Session.SessionFactory.GetClassMetadata(entity.GetType());

            if (metadata.HasProxy)
            {
                ////if entity is a proxy, use the default mechanism
                NHibernateUtil.Initialize(entity);
            }
            else
            {
                var propertiesToLoad = new System.Collections.Generic.List<string>();

                for (var i = 0; i < metadata.PropertyNames.Length; ++i)
                {
                    if (metadata.GetPropertyValue(entity, metadata.PropertyNames[i]) == null)
                    {
                        if ((metadata.PropertyTypes[i].IsEntityType == false) || (useProxyWhenPossible == false))
                        {
                            ////either load the value
                            propertiesToLoad.Add(metadata.PropertyNames[i]);
                        }
                        else
                        {
                            ////or the id of the associated entity
                            propertiesToLoad.Add(string.Concat(metadata.PropertyNames[i], "." + metadata.IdentifierPropertyName));
                        }
                    }
                }

                var hql = new System.Text.StringBuilder();
                hql.Append("select ");
                hql.Append(string.Join(", ", propertiesToLoad));
                hql.AppendFormat(" from {0} where {1} = :{2}", entity.GetType(), metadata.IdentifierPropertyName, metadata.IdentifierPropertyName);

                var query = this.Session.CreateQuery(hql.ToString());
                query.SetParameter(metadata.IdentifierPropertyName, metadata.GetIdentifier(entity));

                var result = query.UniqueResult();
                var values = result as object[] ?? new object[] { result };

                for (var i = 0; i < propertiesToLoad.Count; ++i)
                {
                    var parts = propertiesToLoad[i].Split('.');
                    var value = values[i];
                    var propertyName = parts.First();

                    if (parts.Length > 0)
                    {
                        var propertyIndex = Array.IndexOf(metadata.PropertyNames, propertyName);
                        var propertyType = metadata.PropertyTypes[propertyIndex].ReturnedClass;

                        ////create a proxy
                        value = this.Session.Load(propertyType, values[i]);
                    }

                    metadata.SetPropertyValue(entity, propertyName, value);
                }
            }
        }

        /// <summary>
        /// Log the exception
        /// </summary>
        /// <param name="exception">Exception details</param>
        /// <returns>Returns true if success</returns>
        public bool LogException(Exception exception)
        {
            NHibernate.Exceptions.ISQLExceptionConverter sqlExceptionConverter = this.Session.GetSessionImplementation().Factory.SQLExceptionConverter;
            sqlExceptionConverter.Convert(new NHibernate.Exceptions.AdoExceptionContextInfo()
            {
                SqlException = exception,
                EntityName = this.GetEntityName()
            });

            return true;
        }

        #endregion

        #region private methods
        /// <summary>
        /// Gets Name of NHibernate Entity
        /// </summary>
        /// <returns>Returns Name of NHibernate Entity</returns>
        private string GetEntityName()
        {
            switch (this.mode)
            {
                case EntityNameMode.Table: return "Table";
                case EntityNameMode.View: return "View";
                default: return typeof(TEntity).FullName;
            }
        }

        #endregion
    }
}