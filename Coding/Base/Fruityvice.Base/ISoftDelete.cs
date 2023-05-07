namespace Fruityvice.Base
{
    /// <summary>
    /// Soft Delete Interface
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// Gets or sets a value indicating whether entity is deleted or not, True/Yes/Y/1 means deleted, False/No/N/0 means not deleted
        /// </summary>
        bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets entity is deleted DateTime
        /// </summary>
        System.DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Gets or sets name of user who is deleted.
        /// </summary>
        string? DeletedBy { get; set; }
    }
}
