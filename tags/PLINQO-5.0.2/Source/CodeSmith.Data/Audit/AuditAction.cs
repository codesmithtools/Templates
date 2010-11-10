namespace CodeSmith.Data.Audit
{
    /// <summary>
    /// A list of entity actions for the audit log.
    /// </summary>
    public enum AuditAction
    {
        /// <summary>
        /// The entity was inserted.
        /// </summary>
        Insert = 1,
        /// <summary>
        /// The entity was updated.
        /// </summary>
        Update = 2,
        /// <summary>
        /// The entity was deleted.
        /// </summary>
        Delete = 3
    }
}