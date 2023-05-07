namespace Fruityvice.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using global::NHibernate;

    /// <summary>
    /// Implements Unit of work for NHibernate.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region local Contstants - CONSTANTS

        #endregion

        #region local variables - Declaration Order: public/protected/private
        ////All variables must be private

        /// <summary>
        /// NHibernate Session Closed Message
        /// </summary>
        private const string NHSessionClosedMessage = "NHibernate Session is closed!";

        /// <summary>
        /// Gets current instance of the UnitOfWork.
        /// It gets the right instance that is related to current thread.
        /// </summary>
        [ThreadStatic]
        private static UnitOfWork activeUnitOfWork;

        /// <summary>
        /// Reference to the session factory.
        /// </summary>
        private readonly ISessionFactory activeSessionFactory;

        #endregion

        #region Constructors/Finalizer

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="session">NHibernate Session</param>
        public UnitOfWork(ISession session)
        {
            //// Added for ASP.Net Core
            this.Session = session;
            this.activeSessionFactory = this.Session != null ? this.Session.SessionFactory : null;

            if (!this.OpenSessionIfClosed())
            {
                throw new Exception(NHSessionClosedMessage);
            }
        }
        #endregion

        #region public properties/enum
        /// <summary>
        /// Gets or sets current instance of the UnitOfWork.
        /// It gets the right instance that is related to current thread.
        /// </summary>
        public static UnitOfWork Current
        {
            get { return activeUnitOfWork; }
            set { activeUnitOfWork = value; }
        }

        /// <summary>
        /// Gets NHibernate session object to perform queries.
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// Gets the application to define units of work, while maintaining abstraction from the underlying transaction implementation
        /// </summary>
        public ITransaction Transaction { get; private set; }
        #endregion
        #region public methods

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        public void BeginTransaction()
        {
            ////this.Session = this.activeSessionFactory.OpenSession();
            if (!this.OpenSessionIfClosed())
            {
                throw new Exception(NHSessionClosedMessage);
            }

            this.Transaction = this.Session.BeginTransaction();
        }

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        public void Commit()
        {
            try
            {
                if (this.Transaction == null)
                {
                    throw new NullReferenceException(nameof(this.Transaction));
                }

                if (!this.Transaction.WasCommitted)
                {
                    this.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        public void Rollback()
        {
            try
            {
                if (this.Transaction == null)
                {
                    throw new NullReferenceException(nameof(this.Transaction));
                }

                if (!this.Transaction.WasRolledBack)
                {
                    this.Transaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                this.CloseSessionAndOpenNewSession();
            }
        }

        /// <summary>
        /// Should Commit Or Rollback Transaction or not
        /// </summary>
        /// <param name="transaction">NHibernate Transaction</param>
        /// <returns>Returns true if it is able to commit otherwise false.</returns>
        public bool ShouldCommitOrRollbackTransaction(NHibernate.ITransaction transaction)
        {
            return transaction != null && transaction.IsActive && !transaction.WasRolledBack;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Opens NHibernate Session
        /// </summary>
        /// <returns>Returns true if NHibernate Session is opened, otherwise false</returns>
        private bool OpenSessionIfClosed()
        {
            if (this.Session != null && !this.Session.IsOpen)
            {
                this.Session = this.Session.SessionFactory.OpenSession();
            }
            else if (this.Session == null && this.activeSessionFactory != null)
            {
                this.Session = this.activeSessionFactory.OpenSession();
            }

            ////this.EnableBasicFilter();
            return this.Session.IsOpen;
        }

        /// <summary>
        /// Close Session And Open New Session
        /// </summary>
        private void CloseSessionAndOpenNewSession()
        {
            if (this.Session != null && this.Session.IsOpen)
            {
                this.Session.Close();
            }

            this.OpenSessionIfClosed();
        }

        /// <summary>
        /// Enable the named filter for this current session.
        /// </summary>
        /// <param name="filterName">The name of the filter to be enabled.</param>
        /// <returns>Returns true if successfully converted default filter condition to parameters</returns>
        private bool EnableFilter(string filterName)
        {
            IFilter filter = this.Session.EnableFilter(filterName);
            return this.EnableFilterWithDefaultFilterCondition(filter);
        }

        /// <summary>
        /// Converted default filter condition to parameters
        /// </summary>
        /// <param name="filter">
        ///     Type definition of Filter. Filter defines the user's view into enabled dynamic
        ///     filters, allowing them to set filter parameter values.
        /// </param>
        /// <returns>Returns true if successfully converted default filter condition to parameters</returns>
        private bool EnableFilterWithDefaultFilterCondition(IFilter filter)
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
                catch (Exception ex)
                {
                    throw;
                }
            }

            return true;
        }
        #endregion
    }
}
