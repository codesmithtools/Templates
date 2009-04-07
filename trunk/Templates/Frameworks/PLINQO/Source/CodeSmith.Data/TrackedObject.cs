namespace CodeSmith.Data
{
    /// <summary>
    /// A class representing a tracked object.
    /// </summary>
    /// <typeparam name="T">The type of the tracked object.</typeparam>
    public class TrackedObject<T>
    {
        /// <summary>
        /// Gets the current tracked object.
        /// </summary>
        /// <value>The current.</value>
        public T Current { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
        public bool IsChanged { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew { get; internal set; }

        /// <summary>
        /// Gets the original tracked object.
        /// </summary>
        /// <value>The original.</value>
        public T Original { get; internal set; }
    }

    /// <summary>
    /// A class representing a tracked object.
    /// </summary>
    public class TrackedObject : TrackedObject<object>
    {}
}