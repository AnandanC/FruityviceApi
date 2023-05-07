namespace Fruityvice.Base
{
    using System;

    /// <summary>
    /// Defines interface for base entity type.
    /// </summary>
    public interface IAuditEntity
    {
        /// <summary>
        /// Gets or sets name of user who is initially created.
        /// </summary>
        string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets entered date which is initially created.
        /// </summary>
        DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets name of user who is last updated.
        /// </summary>
        string? LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets entered date which is last updated.
        /// </summary>
        DateTime? LastUpdatedDate { get; set; }
    }
}
