namespace Fruityvice.Base
{
    /// <summary>
    /// Gets Entity Name Retrieval Type
    /// </summary>
    public enum EntityNameMode
    {
        /// <summary>
        /// Gets NHibernate Entity Name
        /// </summary>
        Default,

        /// <summary>
        /// Gets NHibernate Entity Name as "Table"
        /// </summary>
        Table,

        /// <summary>
        /// Gets NHibernate Entity Name as "View"
        /// </summary>
        View
    }
}
