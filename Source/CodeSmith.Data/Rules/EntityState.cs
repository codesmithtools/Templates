using System;

namespace CodeSmith.Data.Rules
{
    /// <summary>
    /// The state to the tracked entity
    /// </summary>
    [Flags]
    public enum EntityState
    {
        /// <summary>The entity is new.</summary>
        New = 0,
        /// <summary>The entity is changed.</summary>
        Changed = 1,
        /// <summary>The entity is deleted.</summary>
        Deleted = 2,
        /// <summary>The entity is new or changed.</summary>
        Dirty = New | Changed,
        /// <summary>All the entity states.</summary>
        All = New | Changed | Deleted
    }
}