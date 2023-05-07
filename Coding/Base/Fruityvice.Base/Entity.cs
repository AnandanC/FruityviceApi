namespace Fruityvice.Base
{
    using System;

    /// <summary>
    /// Base class for all Entity types.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {

        #region Constructors/Finalizer
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TPrimaryKey}"/> class.
        /// </summary>
        public Entity()
        {
        }
        #endregion

        #region public properties/enum
        /// <summary>
        /// Gets or sets Unique identifier for this entity.
        /// </summary>
        //////[JsonIgnore]
        public virtual TPrimaryKey? Id { get; protected set; }

        /// <summary>
        /// Gets or sets Version key of the entity.
        /// </summary>
        //////[JsonIgnore]
        public virtual TPrimaryKey? Version { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether entity is deleted or not, True/Yes/Y/1 means deleted, False/No/N/0 means not deleted
        /// </summary>
        public virtual bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets entity is deleted DateTime
        /// </summary>
        public virtual System.DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Gets or sets entity is deleted DateTime
        /// </summary>
        public virtual string? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets name of user who is initially created.
        /// </summary>
        public virtual string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets entered date which is initially created.
        /// </summary>
        public virtual DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets name of user who is last updated.
        /// </summary>
        public virtual string? LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets entered date which is last updated.
        /// </summary>
        public virtual DateTime? LastUpdatedDate { get; set; }

        #endregion
    }
}
