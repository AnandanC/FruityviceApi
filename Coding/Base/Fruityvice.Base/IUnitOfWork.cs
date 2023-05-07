namespace Fruityvice.Base
{
    /// <summary>
    /// Represents a transactional job.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Should Commit Or Rollback Transaction or not
        /// </summary>
        /// <param name="transaction">NHibernate Transaction</param>
        /// <returns>Returns true if it is able to commit otherwise false.</returns>
        bool ShouldCommitOrRollbackTransaction(NHibernate.ITransaction transaction);
    }
}
